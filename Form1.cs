using System.IO.Ports;
using System.Text;
using System.Text.RegularExpressions;

namespace AT_command_test
{
    public partial class Form1 : Form
    {
        private SerialPort? _serialPort;

        // Thread-safe buffer for automated test flows
        private readonly object _rxLock = new();
        private string _rxBuffer = string.Empty;
        private TaskCompletionSource<bool>? _rxSignal;
        private CancellationTokenSource? _testCts;
        private volatile bool _suppressTerminalRxLog = false;
        private bool _isSyncingUrlAndLocalFile = false;

        public enum LogType
        {
            Info,
            Tx,
            Rx,
            Error
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitBaudRates();
            ScanComPorts();
            AutoDetectLocal1MFile();
            InitializeQuickButtons();
        }

        private void InitializeQuickButtons()
        {
            ConfigureQuickButton(btnQuickAT, "AT", "AT");
            ConfigureQuickButton(btnQuickATI, "ATI", "ATI");
            ConfigureQuickButton(btnQuickATI7, "ATI7 (Ver)", "ATI7");
            ConfigureQuickButton(btnQuickCPIN, "AT+CPIN?", "AT+CPIN?");
            ConfigureQuickButton(btnQuickCSQ, "AT+CSQ", "AT+CSQ");
            ConfigureQuickButton(btnQuickCREG, "AT+CREG?", "AT+CREG?");
            ConfigureQuickButton(btnQuickCEREG, "AT+CEREG?", "AT+CEREG?");
            ConfigureQuickButton(btnQuickCOPS, "AT+COPS?", "AT+COPS?");
            ConfigureQuickButton(btnQuickCGATT, "AT+CGATT?", "AT+CGATT?");
            ConfigureQuickButton(btnQuickGTFLIST, "📁 AT+GTFLIST", "AT+GTFLIST");

            btnQuickGTFLIST.BackColor = Color.FromArgb(0, 121, 107);
            btnQuickGTFLIST.ForeColor = Color.White;
            btnQuickGTFLIST.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        }

        private void ConfigureQuickButton(Button? btn, string text, string command)
        {
            if (btn == null) return;
            btn.Text = text;
            btn.Tag = command;
            btn.AutoSize = true;
            btn.Height = 27;
            btn.Margin = new Padding(3, 1, 3, 1);
            btn.UseVisualStyleBackColor = (btn != btnQuickGTFLIST);
            btn.Click -= QuickCommand_Click;
            btn.Click += QuickCommand_Click;
        }

        private void AutoDetectLocal1MFile()
        {
            string candidate = ResolveLocalFilePath("1M.txt");
            if (File.Exists(candidate))
            {
                txtLocalFile.Text = candidate;
                AppendLog($"Found local reference file: {candidate}", LogType.Info);
            }
        }

        private string ResolveLocalFilePath(string filename)
        {
            if (File.Exists(filename))
                return Path.GetFullPath(filename);

            string baseDirFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);
            if (File.Exists(baseDirFile))
                return baseDirFile;

            string cwdFile = Path.Combine(Directory.GetCurrentDirectory(), filename);
            if (File.Exists(cwdFile))
                return cwdFile;

            return filename;
        }

        /// <summary>
        /// Initialize standard baud rates list and select 115200 by default.
        /// </summary>
        private void InitBaudRates()
        {
            int[] baudRates = { 9600, 19200, 38400, 57600, 115200, 230400, 460800, 921600 };
            cboBaudRate.Items.Clear();

            foreach (var baud in baudRates)
            {
                cboBaudRate.Items.Add(baud);
            }

            // Set default to 115200
            cboBaudRate.SelectedItem = 115200;
            if (cboBaudRate.SelectedIndex == -1 && cboBaudRate.Items.Count > 0)
            {
                cboBaudRate.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Scan and populate available COM ports on the system.
        /// </summary>
        private void ScanComPorts()
        {
            string currentSelection = cboPort.SelectedItem?.ToString() ?? string.Empty;
            cboPort.Items.Clear();

            string[] ports = SerialPort.GetPortNames();
            if (ports.Length > 0)
            {
                var sortedPorts = ports.OrderBy(p =>
                {
                    if (p.StartsWith("COM", StringComparison.OrdinalIgnoreCase) &&
                        int.TryParse(p.Substring(3), out int num))
                    {
                        return num;
                    }
                    return 9999;
                }).ToArray();

                cboPort.Items.AddRange(sortedPorts);

                if (!string.IsNullOrEmpty(currentSelection) && cboPort.Items.Contains(currentSelection))
                {
                    cboPort.SelectedItem = currentSelection;
                }
                else
                {
                    cboPort.SelectedIndex = 0;
                }
                AppendLog($"Found {ports.Length} available COM port(s).", LogType.Info);
            }
            else
            {
                AppendLog("No COM ports found on this system.", LogType.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            ScanComPorts();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                DisconnectPort();
            }
            else
            {
                ConnectPort();
            }
        }

        /// <summary>
        /// Connect to selected COM port with default Baud rate 115200.
        /// </summary>
        private void ConnectPort()
        {
            if (cboPort.SelectedItem == null)
            {
                MessageBox.Show("Please select a COM port before connecting!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string portName = cboPort.SelectedItem.ToString()!;
            int baudRate = 115200;
            if (cboBaudRate.SelectedItem != null && int.TryParse(cboBaudRate.SelectedItem.ToString(), out int parsedBaud))
            {
                baudRate = parsedBaud;
            }

            try
            {
                _serialPort = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One)
                {
                    ReadTimeout = 1500,
                    WriteTimeout = 1500,
                    DtrEnable = true, // Required for many cellular/AT modems
                    RtsEnable = true,
                    Encoding = Encoding.Latin1,
                    ReadBufferSize = 1024 * 1024, // 1MB buffer to prevent UART overflow on large chunks
                    WriteBufferSize = 64 * 1024
                };

                _serialPort.DataReceived += SerialPort_DataReceived;
                _serialPort.Open();

                UpdateConnectionState(true, portName, baudRate);
                AppendLog($"Successfully connected to {portName} @ {baudRate} bps.", LogType.Info);
            }
            catch (Exception ex)
            {
                DisconnectPort();
                MessageBox.Show($"Unable to open {portName}!\nDetails: {ex.Message}", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                AppendLog($"Connection error ({portName}): {ex.Message}", LogType.Error);
            }
        }

        /// <summary>
        /// Disconnect and release the COM port.
        /// </summary>
        private void DisconnectPort()
        {
            try
            {
                _testCts?.Cancel();

                if (_serialPort != null)
                {
                    _serialPort.DataReceived -= SerialPort_DataReceived;
                    if (_serialPort.IsOpen)
                    {
                        _serialPort.Close();
                    }
                    _serialPort.Dispose();
                    _serialPort = null;
                }
            }
            catch (Exception ex)
            {
                AppendLog($"Error while disconnecting: {ex.Message}", LogType.Error);
            }
            finally
            {
                UpdateConnectionState(false);
                AppendLog("COM port disconnected.", LogType.Info);
            }
        }

        private void UpdateConnectionState(bool isConnected, string port = "", int baud = 115200)
        {
            cboPort.Enabled = !isConnected;
            btnRefresh.Enabled = !isConnected;
            cboBaudRate.Enabled = !isConnected;

            if (isConnected)
            {
                btnConnect.Text = "Disconnect";
                btnConnect.BackColor = Color.FromArgb(198, 40, 40); // Red
                lblStatus.Text = $"● Connected: {port} @ {baud} bps";
                lblStatus.ForeColor = Color.FromArgb(46, 125, 50); // Green
            }
            else
            {
                btnConnect.Text = "Connect";
                btnConnect.BackColor = Color.FromArgb(46, 125, 50); // Green
                lblStatus.Text = "● Disconnected";
                lblStatus.ForeColor = Color.Red;
            }
        }

        /// <summary>
        /// Serial data received event handler.
        /// </summary>
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (_serialPort != null && _serialPort.IsOpen)
                {
                    string incomingData = _serialPort.ReadExisting();
                    if (!string.IsNullOrEmpty(incomingData))
                    {
                        lock (_rxLock)
                        {
                            _rxBuffer += incomingData;
                            _rxSignal?.TrySetResult(true);
                        }

                        if (!_suppressTerminalRxLog)
                        {
                            BeginInvoke(new Action(() =>
                            {
                                AppendLog(incomingData, LogType.Rx);
                            }));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                BeginInvoke(new Action(() =>
                {
                    AppendLog($"Data receive error: {ex.Message}", LogType.Error);
                }));
            }
        }

        /// <summary>
        /// Send AT command over serial port.
        /// </summary>
        private void SendCommand(string command)
        {
            if (_serialPort == null || !_serialPort.IsOpen)
            {
                MessageBox.Show("Please connect to a COM port before sending commands!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(command))
            {
                return;
            }

            try
            {
                string payload = command;
                if (chkCRLF.Checked)
                {
                    payload += "\r\n";
                }

                _serialPort.Write(payload);
                AppendLog(command, LogType.Tx);
            }
            catch (Exception ex)
            {
                AppendLog($"Transmission error: {ex.Message}", LogType.Error);
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string cmd = txtCommand.Text.Trim();
            if (!string.IsNullOrEmpty(cmd))
            {
                SendCommand(cmd);
                txtCommand.Focus();
                txtCommand.SelectAll();
            }
        }

        private void txtCommand_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                btnSend.PerformClick();
            }
        }

        private void QuickCommand_Click(object? sender, EventArgs e)
        {
            if (sender is Button btn && btn.Tag is string cmd)
            {
                txtCommand.Text = cmd;
                SendCommand(cmd);
            }
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            rtbLog.Clear();
        }

        #region Local Reference File & Browse

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog
            {
                Title = "Select Reference 1M.txt File",
                Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                txtLocalFile.Text = ofd.FileName;
                btnCheckLocal.PerformClick();
            }
        }

        private async void btnCheckLocal_Click(object sender, EventArgs e)
        {
            string path = ResolveLocalFilePath(txtLocalFile.Text.Trim());
            if (!File.Exists(path))
            {
                MessageBox.Show($"File not found:\n{path}\nPlease verify the path or click Browse.", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Streaming calculation - O(1) RAM even for large files
                var report = await Crc16Helper.AnalyzeFileStreamAsync(path);

                AppendLog("--------------------------------------------------", LogType.Info);
                AppendLog($"[LOCAL FILE AUDIT] File: {Path.GetFileName(path)}", LogType.Info);
                AppendLog($"  Path       : {path}", LogType.Info);
                AppendLog($"  Size       : {report.ByteCount:N0} bytes ({report.ByteCount / 1024.0:F1} KB)", LogType.Info);
                AppendLog($"  CRC16-CCITT: 0x{report.Crc16Ccitt:X4}", LogType.Info);
                AppendLog($"  CRC16-Modb : 0x{report.Crc16Modbus:X4}", LogType.Info);
                AppendLog($"  CRC16-XMOD : 0x{report.Crc16Xmodem:X4}", LogType.Info);
                AppendLog($"  MD5 Hash   : {report.Md5}", LogType.Info);
                AppendLog("--------------------------------------------------", LogType.Info);

                lblCrcVerdict.Text = $"Local CRC16-CCITT: 0x{report.Crc16Ccitt:X4} | Modbus: 0x{report.Crc16Modbus:X4}";
                lblCrcVerdict.ForeColor = Color.FromArgb(21, 101, 192); // Blue
            }
            catch (Exception ex)
            {
                AppendLog($"Error analyzing local file: {ex.Message}", LogType.Error);
            }
        }

        private void txtHttpUrl_TextChanged(object? sender, EventArgs e)
        {
            if (_isSyncingUrlAndLocalFile) return;

            string url = txtHttpUrl.Text.Trim();
            if (string.IsNullOrEmpty(url)) return;

            string fileName = "";
            try
            {
                if (Uri.TryCreate(url, UriKind.Absolute, out Uri? uri))
                {
                    fileName = Path.GetFileName(uri.LocalPath);
                }
                else
                {
                    fileName = Path.GetFileName(url);
                }
            }
            catch
            {
                fileName = Path.GetFileName(url);
            }

            if (!string.IsNullOrWhiteSpace(fileName))
            {
                UpdateModuleFileName(fileName);

                string currentLocal = txtLocalFile.Text.Trim();
                string newLocal = fileName;
                try
                {
                    string? currentDir = Path.GetDirectoryName(currentLocal);
                    if (!string.IsNullOrEmpty(currentDir) && Directory.Exists(currentDir))
                    {
                        newLocal = Path.Combine(currentDir, fileName);
                    }
                }
                catch
                {
                    // Ignore path formatting issues during typing
                }

                if (!string.Equals(newLocal, currentLocal, StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        _isSyncingUrlAndLocalFile = true;
                        txtLocalFile.Text = newLocal;
                    }
                    finally
                    {
                        _isSyncingUrlAndLocalFile = false;
                    }
                }
            }
        }

        private void txtLocalFile_TextChanged(object? sender, EventArgs e)
        {
            toolTip1?.SetToolTip(txtLocalFile, txtLocalFile.Text);

            if (_isSyncingUrlAndLocalFile) return;

            string localText = txtLocalFile.Text.Trim();
            if (string.IsNullOrEmpty(localText)) return;

            string localFileName = "";
            try
            {
                localFileName = Path.GetFileName(localText);
            }
            catch
            {
                // Ignore path formatting issues during typing
            }

            if (!string.IsNullOrWhiteSpace(localFileName))
            {
                UpdateModuleFileName(localFileName);

                string currentUrl = txtHttpUrl.Text.Trim();
                if (!string.IsNullOrEmpty(currentUrl))
                {
                    string newUrl = ReplaceUrlFileName(currentUrl, localFileName);
                    if (!string.Equals(newUrl, currentUrl, StringComparison.OrdinalIgnoreCase))
                    {
                        try
                        {
                            _isSyncingUrlAndLocalFile = true;
                            txtHttpUrl.Text = newUrl;
                        }
                        finally
                        {
                            _isSyncingUrlAndLocalFile = false;
                        }
                    }
                }
            }
        }

        private void txtDownloadFile_TextChanged(object? sender, EventArgs e)
        {
            if (_isSyncingUrlAndLocalFile) return;

            string modText = txtDownloadFile.Text.Trim();
            if (string.IsNullOrEmpty(modText)) return;

            string modFileName = "";
            int lastSlash = modText.LastIndexOf('/');
            modFileName = lastSlash >= 0 ? modText.Substring(lastSlash + 1) : modText;

            if (!string.IsNullOrWhiteSpace(modFileName))
            {
                try
                {
                    _isSyncingUrlAndLocalFile = true;

                    // Sync to Server URL
                    string currentUrl = txtHttpUrl.Text.Trim();
                    if (!string.IsNullOrEmpty(currentUrl))
                    {
                        string newUrl = ReplaceUrlFileName(currentUrl, modFileName);
                        if (!string.Equals(newUrl, currentUrl, StringComparison.OrdinalIgnoreCase))
                        {
                            txtHttpUrl.Text = newUrl;
                        }
                    }

                    // Sync to Local Ref
                    string currentLocal = txtLocalFile.Text.Trim();
                    string newLocal = modFileName;
                    try
                    {
                        string? currentDir = Path.GetDirectoryName(currentLocal);
                        if (!string.IsNullOrEmpty(currentDir) && Directory.Exists(currentDir))
                        {
                            newLocal = Path.Combine(currentDir, modFileName);
                        }
                    }
                    catch
                    {
                        // Ignore path formatting issues
                    }

                    if (!string.Equals(newLocal, currentLocal, StringComparison.OrdinalIgnoreCase))
                    {
                        txtLocalFile.Text = newLocal;
                    }
                }
                finally
                {
                    _isSyncingUrlAndLocalFile = false;
                }
            }
        }

        private void UpdateModuleFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return;

            string currentModFile = txtDownloadFile.Text.Trim();
            string newModFile;
            int lastSlash = currentModFile.LastIndexOf('/');
            if (lastSlash >= 0)
            {
                newModFile = currentModFile.Substring(0, lastSlash + 1) + fileName;
            }
            else
            {
                newModFile = $"/mnt/userdata/{fileName}";
            }

            if (!string.Equals(newModFile, currentModFile, StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    _isSyncingUrlAndLocalFile = true;
                    txtDownloadFile.Text = newModFile;
                }
                finally
                {
                    _isSyncingUrlAndLocalFile = false;
                }
            }
        }

        private static string ReplaceUrlFileName(string url, string newFileName)
        {
            try
            {
                if (Uri.TryCreate(url, UriKind.Absolute, out Uri? uri))
                {
                    string path = uri.AbsolutePath;
                    int lastSlash = path.LastIndexOf('/');
                    string basePath = lastSlash >= 0 ? path.Substring(0, lastSlash + 1) : "/";

                    var builder = new UriBuilder(uri)
                    {
                        Path = basePath + newFileName
                    };
                    return builder.Uri.ToString();
                }
            }
            catch
            {
                // Fallback for non-standard URI
            }

            int slashIdx = url.LastIndexOf('/');
            if (slashIdx >= 0)
            {
                return url.Substring(0, slashIdx + 1) + newFileName;
            }
            return url;
        }

        #endregion

        #region 1MB Download & CRC16 Verification Flow

        private async void btnRun1MTest_Click(object sender, EventArgs e)
        {
            if (_testCts != null)
            {
                // Currently running -> Cancel test
                _testCts.Cancel();
                return;
            }

            if (_serialPort == null || !_serialPort.IsOpen)
            {
                MessageBox.Show("Please connect to a COM port before running the test!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string initialLocalPath = ResolveLocalFilePath(txtLocalFile.Text.Trim());
            string? localDir = Path.GetDirectoryName(initialLocalPath);
            if (string.IsNullOrEmpty(localDir) || !Directory.Exists(localDir))
            {
                localDir = AppDomain.CurrentDomain.BaseDirectory;
            }

            string baseUrl = txtHttpUrl.Text.Trim();
            if (string.IsNullOrEmpty(baseUrl))
            {
                MessageBox.Show("Please enter the HTTP Server URL!", "Missing URL", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int timeoutSec = 60;
            if (int.TryParse(txtTimeout.Text.Trim(), out int parsedTimeout) && parsedTimeout > 0)
            {
                timeoutSec = parsedTimeout;
            }

            int totalLoops = 1;
            if (int.TryParse(txtLoopCount.Text.Trim(), out int parsedLoops) && parsedLoops > 0)
            {
                totalLoops = parsedLoops;
            }

            int chunkSizeBytes = GetConfiguredChunkSizeBytes();
            int chunkKb = chunkSizeBytes / 1024;

            _testCts = new CancellationTokenSource();
            btnRun1MTest.Text = totalLoops > 1 ? $"⏹ Stop Test (0/{totalLoops})" : "⏹ Stop 1M Test";
            btnRun1MTest.BackColor = Color.FromArgb(198, 40, 40); // Red
            lblDownloadState.Text = totalLoops > 1 
                ? $"State: Starting {totalLoops} alternating loops..." 
                : $"State: Starting download (Timeout: {timeoutSec}s)...";
            lblDownloadState.ForeColor = Color.FromArgb(21, 101, 192); // Blue
            prg1M.Value = 0;
            lblProgress.Text = "Starting test...";

            AppendLog("==================================================", LogType.Info);
            AppendLog(totalLoops > 1 
                ? $">>> STARTING MULTI-LOOP HTTP TEST ({totalLoops} LOOPS ALTERNATING 1M & 1M_OVER) <<<" 
                : ">>> STARTING 1MB HTTP DOWNLOAD & CRC16 TEST <<<", LogType.Info);
            AppendLog($"  Base URL    : {baseUrl}", LogType.Info);
            AppendLog($"  Total Loops : {totalLoops}", LogType.Info);
            AppendLog($"  Wait Timeout: {timeoutSec}s", LogType.Info);
            AppendLog($"  Read Chunk  : {chunkKb} KB ({chunkSizeBytes:N0} bytes)", LogType.Info);
            AppendLog($"  Local Dir   : {localDir}", LogType.Info);
            AppendLog("==================================================", LogType.Info);

            int passCount = 0;
            int failCount = 0;
            var failedLoops = new List<int>();

            try
            {
                // Step 0: Activate network PDP context once before starting loops
                AppendLog("--- [Setup] Activating PDP Context (at+mipcall=1) ---", LogType.Info);
                var rPdp = await SendAndAwaitResponseAsync("at+mipcall=1", new[] { "OK" }, 12000, _testCts.Token, postDelayMs: 2000);
                if (!rPdp.success && !rPdp.response.Contains("+MIPCALL:"))
                {
                    throw new Exception("at+mipcall=1 failed to activate network context.");
                }
                await Task.Delay(1000, _testCts.Token);

                for (int loop = 1; loop <= totalLoops; loop++)
                {
                    _testCts.Token.ThrowIfCancellationRequested();

                    // Determine alternating target file:
                    // If totalLoops == 1: use currently chosen file
                    // If totalLoops > 1: odd loop -> 1M.txt, even loop -> 1M_Over.txt
                    string targetFileName;
                    if (totalLoops == 1)
                    {
                        targetFileName = Path.GetFileName(initialLocalPath);
                        if (string.IsNullOrWhiteSpace(targetFileName)) targetFileName = "1M.txt";
                    }
                    else
                    {
                        targetFileName = (loop % 2 == 1) ? "1M.txt" : "1M_Over.txt";
                    }

                    string currentLocalPath = Path.Combine(localDir, targetFileName);
                    if (!File.Exists(currentLocalPath))
                    {
                        currentLocalPath = ResolveLocalFilePath(targetFileName);
                        if (!File.Exists(currentLocalPath))
                        {
                            throw new FileNotFoundException($"Reference file not found for loop {loop}: {targetFileName}");
                        }
                    }

                    string currentUrl = ReplaceUrlFileName(baseUrl, targetFileName);
                    string currentModuleFile = $"/mnt/userdata/{targetFileName}";

                    // Synchronize UI
                    try
                    {
                        _isSyncingUrlAndLocalFile = true;
                        txtHttpUrl.Text = currentUrl;
                        txtDownloadFile.Text = currentModuleFile;
                        txtLocalFile.Text = currentLocalPath;
                    }
                    finally
                    {
                        _isSyncingUrlAndLocalFile = false;
                    }

                    btnRun1MTest.Text = totalLoops > 1 ? $"⏹ Stop Test ({loop}/{totalLoops})" : "⏹ Stop 1M Test";
                    lblDownloadState.Text = totalLoops > 1 
                        ? $"State: [Loop {loop}/{totalLoops}] Downloading {targetFileName}..." 
                        : $"State: Downloading {targetFileName} from server...";
                    lblDownloadState.ForeColor = Color.FromArgb(21, 101, 192);

                    AppendLog("--------------------------------------------------", LogType.Info);
                    AppendLog($">>> [LOOP {loop}/{totalLoops}] TARGET FILE: '{targetFileName}' <<<", LogType.Info);
                    AppendLog($"  URL        : {currentUrl}", LogType.Info);
                    AppendLog($"  Module File: {currentModuleFile}", LogType.Info);
                    AppendLog($"  Local File : {currentLocalPath}", LogType.Info);

                    // 1. Calculate Local Reference Checksums
                    var localReport = await Crc16Helper.AnalyzeFileStreamAsync(currentLocalPath, _testCts.Token);
                    AppendLog($"[LOCAL {targetFileName}] Size: {localReport.ByteCount:N0} bytes | CRC16-CCITT: 0x{localReport.Crc16Ccitt:X4} | Modbus: 0x{localReport.Crc16Modbus:X4}", LogType.Info);

                    // 2. Download from Module and Verify
                    bool isMatch = await Execute1MDownloadAndVerifyAsync(
                        currentUrl, 
                        currentLocalPath, 
                        currentModuleFile, 
                        localReport, 
                        timeoutSec, 
                        chunkSizeBytes, 
                        totalLoops <= 1, 
                        loop, 
                        totalLoops, 
                        _testCts.Token);

                    if (isMatch)
                    {
                        passCount++;
                        AppendLog($">>> [LOOP {loop}/{totalLoops} RESULT: PASS] CRC16 MATCH (0x{localReport.Crc16Ccitt:X4}) <<<", LogType.Info);
                    }
                    else
                    {
                        failCount++;
                        failedLoops.Add(loop);
                        AppendLog($">>> [LOOP {loop}/{totalLoops} RESULT: FAIL] CRC16 MISMATCH detected! <<<", LogType.Error);
                    }

                    lblCrcVerdict.Text = totalLoops > 1 
                        ? $"[Loop {loop}/{totalLoops}] {(isMatch ? "PASS" : "FAIL")} | Total Pass: {passCount}, Fail: {failCount}" 
                        : (isMatch ? $"[PASS] CRC16 MATCH: 0x{localReport.Crc16Ccitt:X4} (100% Identical)" : $"[FAIL] MISMATCH: 0x{localReport.Crc16Ccitt:X4}");
                    lblCrcVerdict.ForeColor = (failCount == 0) ? Color.FromArgb(46, 125, 50) : Color.FromArgb(198, 40, 40);

                    if (loop < totalLoops)
                    {
                        AppendLog($"Cooldown: waiting 1.5s before next loop ({loop + 1}/{totalLoops})...", LogType.Info);
                        await Task.Delay(1500, _testCts.Token);
                    }
                }

                // Summary for multi-loop test
                if (totalLoops > 1)
                {
                    AppendLog("==================================================", LogType.Info);
                    AppendLog(">>> MULTI-LOOP TEST SUMMARY <<<", LogType.Info);
                    AppendLog($"  Total Loops  : {totalLoops}", LogType.Info);
                    AppendLog($"  Passed       : {passCount} ({(passCount * 100.0 / totalLoops):F1}%)", LogType.Info);
                    AppendLog($"  Failed       : {failCount} ({(failCount * 100.0 / totalLoops):F1}%)", (failCount > 0) ? LogType.Error : LogType.Info);
                    if (failedLoops.Count > 0)
                    {
                        AppendLog($"  Failed Loops : {string.Join(", ", failedLoops)}", LogType.Error);
                    }
                    AppendLog("==================================================", LogType.Info);

                    string summaryMsg = $"Completed {totalLoops} alternating loops!\n\nPassed: {passCount} / {totalLoops} ({(passCount * 100.0 / totalLoops):F1}%)\nFailed: {failCount} / {totalLoops}";
                    if (failedLoops.Count > 0)
                    {
                        summaryMsg += $"\nFailed loops: {string.Join(", ", failedLoops)}";
                    }
                    MessageBox.Show(summaryMsg, failCount == 0 ? "Test Completed: ALL PASSED" : "Test Completed: WITH FAILURES", MessageBoxButtons.OK, failCount == 0 ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
                }
            }
            catch (OperationCanceledException)
            {
                AppendLog($">>> [STOPPED] Test cancelled by user at loop (Pass: {passCount}, Fail: {failCount}).", LogType.Error);
                lblDownloadState.Text = $"State: Test Cancelled (Completed: {passCount + failCount}/{totalLoops})";
                lblDownloadState.ForeColor = Color.Red;
            }
            catch (Exception ex)
            {
                AppendLog($">>> [FAILED] Test Error: {ex.Message}", LogType.Error);
                lblDownloadState.Text = $"State: FAILED ({ex.Message})";
                lblDownloadState.ForeColor = Color.Red;
            }
            finally
            {
                // Always close IP PDP context regardless of pass or fail
                if (_serialPort != null && _serialPort.IsOpen)
                {
                    try
                    {
                        AppendLog("--- Closing PDP context (at+mipcall=0,1) ---", LogType.Info);
                        await SendAndAwaitResponseAsync("at+mipcall=0,1", new[] { "OK" }, 6000, CancellationToken.None, postDelayMs: 500);
                    }
                    catch (Exception ex)
                    {
                        AppendLog($"Notice closing MIPCALL: {ex.Message}", LogType.Info);
                    }
                }

                _testCts?.Dispose();
                _testCts = null;
                btnRun1MTest.Text = "🚀 Download & Verify CRC16";
                btnRun1MTest.BackColor = Color.FromArgb(230, 81, 0);
            }
        }

        private async Task<bool> Execute1MDownloadAndVerifyAsync(
            string url, 
            string localPath, 
            string moduleFilePath,
            Crc16Helper.ChecksumReport localReport, 
            int waitTimeoutSec, 
            int maxChunkSize,
            bool showMessageBox,
            int currentLoop,
            int totalLoops,
            CancellationToken ct)
        {
            // Determine local save path in the exact same directory as local reference file
            string? localDir = Path.GetDirectoryName(localPath);
            if (string.IsNullOrEmpty(localDir))
            {
                localDir = AppDomain.CurrentDomain.BaseDirectory;
            }

            // Derive downloaded file name from local/URL (e.g. 1M.txt -> 1M_download.txt, 1M_Over.txt -> 1M_Over_download.txt)
            string sourceFileName = Path.GetFileName(localPath);
            if (string.IsNullOrWhiteSpace(sourceFileName))
            {
                sourceFileName = Path.GetFileName(url);
            }

            string baseName = Path.GetFileNameWithoutExtension(sourceFileName);
            string ext = Path.GetExtension(sourceFileName);
            if (string.IsNullOrEmpty(baseName))
            {
                baseName = "1M";
                ext = ".txt";
            }

            string saveDownloadedPath = Path.Combine(localDir, $"{baseName}_download{ext}");

            // Step 1: Ensure PDP context is ready
            AppendLog("--- [1/7] Checking PDP Context ---", LogType.Info);
            lblDownloadState.Text = $"State: [Loop {currentLoop}/{totalLoops}] Checking network PDP...";
            lblProgress.Text = "Checking PDP...";
            prg1M.Value = 5;
            var r1 = await SendAndAwaitResponseAsync("at+mipcall=1", new[] { "OK" }, 6000, ct, postDelayMs: 200);
            if (!r1.success && !r1.response.Contains("+MIPCALL:"))
            {
                // Already active or error, proceed
            }
            await Task.Delay(200, ct);

            // Step 2: Set HTTP URL
            AppendLog($"--- [2/7] Setting HTTP URL ({url}) ---", LogType.Info);
            lblDownloadState.Text = $"State: [Loop {currentLoop}/{totalLoops}] Configuring HTTP URL...";
            lblProgress.Text = "Configuring URL...";
            prg1M.Value = 10;
            var r2 = await SendAndAwaitResponseAsync($"AT+HTTPSET=\"URL\",\"{url}\"", new[] { "OK" }, 6000, ct);
            if (!r2.success) throw new Exception("AT+HTTPSET URL failed.");
            await Task.Delay(400, ct);

            // Step 3: Clean Module Flash & Set Destination
            AppendLog($"--- [3/7] Setting Download Destination ({moduleFilePath}) ---", LogType.Info);
            lblDownloadState.Text = $"State: [Loop {currentLoop}/{totalLoops}] Cleaning flash & setting destination...";
            lblProgress.Text = "Cleaning flash...";
            prg1M.Value = 15;

            // Delete existing file on flash to guarantee clean sectors and avoid dirty overwrite
            await SendAndAwaitResponseAsync($"AT+GTFDELETE=\"{moduleFilePath}\"", new[] { "OK", "ERROR" }, 3000, ct, postDelayMs: 200);

            var r3 = await SendAndAwaitResponseAsync($"AT+HTTPSET=\"DOWNLOADFILE\",\"{moduleFilePath}\"", new[] { "OK" }, 6000, ct);
            if (!r3.success) throw new Exception("AT+HTTPSET DOWNLOADFILE failed.");
            await Task.Delay(400, ct);

            // Step 4: Execute HTTP GET Download
            // Explicitly use AT+HTTPACT=0,60 (60 is the maximum timeout parameter allowed on Fibocom L716)
            string actCmd = "AT+HTTPACT=0,60";

            AppendLog($"--- [4/7] Triggering HTTP Download ({actCmd}) ---", LogType.Info);
            AppendLog($"Waiting up to {waitTimeoutSec}s for download to complete (+HTTPRES:)...", LogType.Info);
            lblDownloadState.Text = $"State: [Loop {currentLoop}/{totalLoops}] Downloading {sourceFileName} (timeout: {waitTimeoutSec}s)...";
            lblProgress.Text = $"Waiting +HTTPRES:...";
            prg1M.Value = 25;

            // Clear buffer right before trigger
            lock (_rxLock)
            {
                _rxBuffer = string.Empty;
                _rxSignal = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            }

            // Send AT+HTTPACT=0,60 and wait for initial OK
            var rAct = await SendAndAwaitResponseAsync(actCmd, new[] { "OK" }, 8000, ct, postDelayMs: 200);
            if (!rAct.success)
            {
                // Fallback attempt with AT+HTTPACT=0
                AppendLog("Retrying with AT+HTTPACT=0...", LogType.Info);
                rAct = await SendAndAwaitResponseAsync("AT+HTTPACT=0", new[] { "OK" }, 8000, ct, postDelayMs: 200);
                if (!rAct.success)
                {
                    throw new Exception("Module rejected AT+HTTPACT with ERROR. Please ensure PDP context (at+mipcall=1) is active.");
                }
            }

            // Wait up to waitTimeoutSec for +HTTPRES: response
            var (resSuccess, resData) = await AwaitUrcAsync("+HTTPRES:", waitTimeoutSec, ct);
            if (!resSuccess)
            {
                throw new Exception($"HTTP download timed out after waiting {waitTimeoutSec}s for +HTTPRES: response from server.");
            }

            AppendLog("Download to module storage completed (+HTTPRES: received)!", LogType.Info);
            AppendLog("Waiting 3s for module to finish writing data to flash storage...", LogType.Info);
            lblDownloadState.Text = $"State: [Loop {currentLoop}/{totalLoops}] Flushing flash storage (waiting 3s)...";
            lblProgress.Text = "Flushing flash (3s)...";
            prg1M.Value = 40;
            await Task.Delay(3000, ct);

            // Step 5: Verify file existence and size via AT+GTFLIST
            AppendLog("--- [5/7] Verifying File Existence in Module Storage ---", LogType.Info);
            lblDownloadState.Text = $"State: [Loop {currentLoop}/{totalLoops}] Verifying downloaded file in module storage...";
            lblProgress.Text = "Verifying file...";
            var r6 = await SendAndAwaitResponseAsync("AT+GTFLIST", new[] { "OK" }, 6000, ct);
            string targetFileName = Path.GetFileName(moduleFilePath);
            if (!r6.success || !r6.response.Contains(targetFileName, StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception($"File '{targetFileName}' was not found in module file list after download.");
            }
            await Task.Delay(1000, ct);

            // Step 6: Open File and query size
            AppendLog($"--- [6/7] Opening Downloaded File on Module ({moduleFilePath}) ---", LogType.Info);
            lblProgress.Text = $"[Loop {currentLoop}/{totalLoops}] Opening file descriptor...";
            var r7 = await SendAndAwaitResponseAsync($"AT+GTFOPEN=\"{moduleFilePath}\",0", new[] { "OK" }, 6000, ct);
            if (!r7.success) throw new Exception($"AT+GTFOPEN failed to open {moduleFilePath}.");

            string fileId = "41";
            var match = Regex.Match(r7.response, @"\+GTFOPEN:\s*(\d+)");
            if (match.Success)
            {
                fileId = match.Groups[1].Value;
            }
            AppendLog($"Opened File Descriptor: {fileId}", LogType.Info);
            await Task.Delay(500, ct);

            // Query size
            var rSize = await SendAndAwaitResponseAsync($"AT+GTFSIZE={fileId}", new[] { "OK" }, 6000, ct);
            int moduleFileSize = 1048576; // Default 1MB
            var sizeMatch = Regex.Match(rSize.response, @"\+GTFSIZE:\s*(\d+)");
            if (sizeMatch.Success && int.TryParse(sizeMatch.Groups[1].Value, out int parsedSize))
            {
                moduleFileSize = parsedSize;
            }
            AppendLog($"Module File Size: {moduleFileSize:N0} bytes (Expected: {localReport.ByteCount:N0} bytes)", LogType.Info);

            // Step 7: Read Module File Data & Calculate CRC16 (configurable chunk size)
            int chunkKb = maxChunkSize / 1024;
            AppendLog($"--- [7/7] Reading Module File Data & Calculating CRC16 ({moduleFileSize:N0} bytes) ---", LogType.Info);

            int totalChunks = (int)Math.Ceiling((double)moduleFileSize / maxChunkSize);
            AppendLog($"Reading {moduleFileSize:N0} bytes in {totalChunks} chunks of {chunkKb}KB ({maxChunkSize:N0} bytes) each.", LogType.Info);

            lblProgress.Text = $"[Loop {currentLoop}/{totalLoops}] Reading: 0 / {moduleFileSize:N0} bytes (0%)";

            using var rollingCalc = new Crc16Helper.RollingChecksumCalculator();
            int bytesRead = 0;

            // Open stream to save the downloaded file in the exact same directory as the uploaded local reference file
            using (var fileStream = new FileStream(saveDownloadedPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, useAsync: true))
            {
                _suppressTerminalRxLog = true;
                try
                {
                    int chunkIndex = 0;
                    while (bytesRead < moduleFileSize)
                    {
                        ct.ThrowIfCancellationRequested();
                        chunkIndex++;
                        int toRead = Math.Min(maxChunkSize, moduleFileSize - bytesRead);

                        lblProgress.Text = $"[Loop {currentLoop}/{totalLoops}] Chunk {chunkIndex}/{totalChunks} ({toRead:N0} bytes)...";
                        var (chunkSuccess, chunkData) = await ReadFileChunkAsync(fileId, toRead, ct);
                        if (!chunkSuccess || chunkData.Length == 0)
                        {
                            AppendLog($"Warning: Read returned 0 bytes or failed at offset {bytesRead}.", LogType.Error);
                            break;
                        }

                        // 1. Incrementally update CRC16 & MD5 on-the-fly
                        rollingCalc.AppendChunk(chunkData);

                        // 2. Stream chunk directly to local disk file
                        await fileStream.WriteAsync(chunkData, 0, chunkData.Length, ct);

                        bytesRead += chunkData.Length;

                        int progressPercent = 40 + (int)((bytesRead / (double)moduleFileSize) * 55); // scale 40% to 95%
                        prg1M.Value = Math.Min(progressPercent, 95);
                        lblDownloadState.Text = $"State: [Loop {currentLoop}/{totalLoops}] Reading chunk {chunkIndex}/{totalChunks} ({toRead:N0} bytes)...";
                        lblProgress.Text = $"{bytesRead:N0} / {moduleFileSize:N0} bytes ({(bytesRead * 100) / moduleFileSize}%)";

                        int logInterval = Math.Min(64, Math.Max(1, totalChunks / 10));
                        if (chunkIndex % logInterval == 0 || bytesRead >= moduleFileSize || chunkIndex == 1)
                        {
                            AppendLog($"[Chunk {chunkIndex}/{totalChunks}] Read {bytesRead:N0} / {moduleFileSize:N0} bytes ({(bytesRead * 100) / moduleFileSize}%)...", LogType.Info);
                        }

                        await Task.Delay(10, ct);
                    }
                }
                finally
                {
                    _suppressTerminalRxLog = false;
                }

                await fileStream.FlushAsync(ct);
            }

            // Close File
            await SendAndAwaitResponseAsync($"at+GTFCLOSE={fileId}", new[] { "OK" }, 5000, ct);

            // Finalize rolling report without storing the entire file in RAM
            var moduleReport = rollingCalc.FinalizeReport();

            prg1M.Value = 100;
            lblProgress.Text = $"[Loop {currentLoop}/{totalLoops}] Done: {moduleReport.ByteCount:N0} bytes.";
            AppendLog($"[FILE SAVED] Downloaded file saved to disk: {saveDownloadedPath}", LogType.Info);

            // Log Complete Comparison
            AppendLog("==================================================", LogType.Info);
            AppendLog($">>> CRC16 INTEGRITY REPORT (LOOP {currentLoop}/{totalLoops}) <<<", LogType.Info);
            AppendLog($"  Local Ref File   : {localReport.ByteCount:N0} bytes", LogType.Info);
            AppendLog($"  Downloaded File  : {moduleReport.ByteCount:N0} bytes -> {saveDownloadedPath}", LogType.Info);
            AppendLog($"  Local CRC16-CCITT  : 0x{localReport.Crc16Ccitt:X4}", LogType.Info);
            AppendLog($"  Module CRC16-CCITT : 0x{moduleReport.Crc16Ccitt:X4}", LogType.Info);
            AppendLog($"  Local CRC16-Modbus : 0x{localReport.Crc16Modbus:X4}", LogType.Info);
            AppendLog($"  Module CRC16-Modbus: 0x{moduleReport.Crc16Modbus:X4}", LogType.Info);
            AppendLog($"  Local MD5 Hash     : {localReport.Md5}", LogType.Info);
            AppendLog($"  Module MD5 Hash    : {moduleReport.Md5}", LogType.Info);

            bool isMatch = (localReport.Crc16Ccitt == moduleReport.Crc16Ccitt) &&
                           (localReport.ByteCount == moduleReport.ByteCount);

            if (isMatch)
            {
                AppendLog($">>> [PASS] CRC16 MATCH (0x{moduleReport.Crc16Ccitt:X4}): FILES ARE 100% IDENTICAL! <<<", LogType.Info);
                if (showMessageBox)
                {
                    lblCrcVerdict.Text = $"[PASS] CRC16 MATCH: 0x{moduleReport.Crc16Ccitt:X4} (100% Identical)";
                    lblCrcVerdict.ForeColor = Color.FromArgb(46, 125, 50); // Dark Green
                    MessageBox.Show($"Verification PASSED!\n\nCRC16-CCITT: 0x{moduleReport.Crc16Ccitt:X4}\nCRC16-Modbus: 0x{moduleReport.Crc16Modbus:X4}\nSize: {moduleReport.ByteCount:N0} bytes\n\nDownloaded file saved to:\n{saveDownloadedPath}", "Integrity Check: PASSED", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                AppendLog($">>> [FAIL] CRC16 MISMATCH: Module (0x{moduleReport.Crc16Ccitt:X4}) != Local (0x{localReport.Crc16Ccitt:X4}) <<<", LogType.Error);
                if (showMessageBox)
                {
                    lblCrcVerdict.Text = $"[FAIL] MISMATCH: Module 0x{moduleReport.Crc16Ccitt:X4} != Local 0x{localReport.Crc16Ccitt:X4}";
                    lblCrcVerdict.ForeColor = Color.FromArgb(198, 40, 40); // Dark Red
                    MessageBox.Show($"Verification FAILED!\n\nLocal CRC16: 0x{localReport.Crc16Ccitt:X4} ({localReport.ByteCount:N0} bytes)\nModule CRC16: 0x{moduleReport.Crc16Ccitt:X4} ({moduleReport.ByteCount:N0} bytes)\n\nData mismatch detected!\nDownloaded file saved for inspection to:\n{saveDownloadedPath}", "Integrity Check: FAILED", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            AppendLog("==================================================", LogType.Info);
            return isMatch;
        }

        /// <summary>
        /// Waits asynchronously up to timeoutSeconds for an expected URC (e.g. +HTTPRES:) with live countdown.
        /// </summary>
        private async Task<(bool success, string response)> AwaitUrcAsync(string expectedToken, int timeoutSeconds, CancellationToken ct)
        {
            DateTime deadline = DateTime.Now.AddSeconds(timeoutSeconds);
            int elapsed = 0;

            while (DateTime.Now < deadline)
            {
                ct.ThrowIfCancellationRequested();

                string currentBuffer;
                lock (_rxLock)
                {
                    currentBuffer = _rxBuffer;
                }

                if (currentBuffer.Contains(expectedToken, StringComparison.OrdinalIgnoreCase))
                {
                    await Task.Delay(300, ct);
                    lock (_rxLock)
                    {
                        currentBuffer = _rxBuffer;
                    }
                    return (true, currentBuffer);
                }

                // Check for fast error notification
                if (currentBuffer.Contains("+HTTPRES: 0,") && !currentBuffer.Contains("+HTTPRES: 0,200"))
                {
                    return (false, currentBuffer);
                }

                elapsed++;
                int remaining = Math.Max(0, (int)(deadline - DateTime.Now).TotalSeconds);
                lblDownloadState.Text = $"State: Downloading from HTTP server... ({elapsed}s / {timeoutSeconds}s, remaining: {remaining}s)";
                lblProgress.Text = $"Waiting: {elapsed}s / {timeoutSeconds}s";

                int progressVal = 25 + Math.Min(15, (elapsed * 15) / timeoutSeconds);
                prg1M.Value = Math.Min(progressVal, 40);

                TaskCompletionSource<bool> signal;
                lock (_rxLock)
                {
                    if (_rxSignal == null || _rxSignal.Task.IsCompleted)
                    {
                        _rxSignal = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                    }
                    signal = _rxSignal;
                }

                var waitTask = signal.Task;
                var delayTask = Task.Delay(1000, ct);
                await Task.WhenAny(waitTask, delayTask);
            }

            lock (_rxLock)
            {
                return (_rxBuffer.Contains(expectedToken, StringComparison.OrdinalIgnoreCase), _rxBuffer);
            }
        }

        /// <summary>
        /// Gets the configured chunk size in bytes based on user selection in cboChunkSize (default 200 KB = 204,800 bytes).
        /// Supports values between 1 KB and 200 KB (maximum supported by Fibocom L716 AT+GTFREAD).
        /// </summary>
        private int GetConfiguredChunkSizeBytes()
        {
            try
            {
                string text = cboChunkSize.Text.Trim();
                var match = Regex.Match(text, @"\d+");
                if (match.Success && int.TryParse(match.Value, out int kb) && kb > 0)
                {
                    // Fibocom L716 AT+GTFREAD maximum length is 200KB (204,800 bytes)
                    kb = Math.Clamp(kb, 1, 200);
                    return kb * 1024;
                }
            }
            catch
            {
                // Fallback default
            }
            return 200 * 1024; // Default 200 KB (204,800 bytes)
        }

        /// <summary>
        /// Reads a chunk of bytes using AT+GTFREAD and converts the returned payload to raw bytes.
        /// Supports configurable chunks up to 200KB with dynamic UART timeout calculation.
        /// </summary>
        private async Task<(bool success, byte[] data)> ReadFileChunkAsync(string fileId, int length, CancellationToken ct)
        {
            // Dynamic timeout calculation for UART transfer (115200 bps ≈ 11.5 KB/s)
            // For 1KB (~90ms transfer time), allocates ~5s timeout
            // For 200KB (~18s transfer time), allocates ~27s timeout
            int dynamicTimeoutMs = Math.Max(5000, (int)((length / 9000.0) * 1000) + 4000);
            var (success, response) = await SendAndAwaitResponseAsync($"AT+GTFREAD={fileId},{length}", new[] { "OK" }, dynamicTimeoutMs, ct, postDelayMs: 20);
            if (!success)
            {
                return (false, Array.Empty<byte>());
            }

            int tagIdx = response.IndexOf("+GTFREAD: ", StringComparison.OrdinalIgnoreCase);
            if (tagIdx < 0)
            {
                return (false, Array.Empty<byte>());
            }

            int dataStart = tagIdx + "+GTFREAD: ".Length;
            int okIdx = response.LastIndexOf("OK", StringComparison.OrdinalIgnoreCase);
            if (okIdx <= dataStart)
            {
                return (false, Array.Empty<byte>());
            }

            int payloadLen;
            // If the response contains at least 'length' bytes before 'OK', extract exactly 'length' bytes.
            // This preserves all content bytes (including any trailing \r\n in the file data) without stripping them!
            if (dataStart + length <= okIdx)
            {
                payloadLen = length;
            }
            else
            {
                // Fallback if module returned fewer bytes than requested before OK
                int dataEnd = okIdx;
                while (dataEnd > dataStart && (response[dataEnd - 1] == '\r' || response[dataEnd - 1] == '\n'))
                {
                    dataEnd--;
                }
                payloadLen = Math.Max(0, dataEnd - dataStart);
            }

            if (payloadLen <= 0)
            {
                return (true, Array.Empty<byte>());
            }

            string rawText = response.Substring(dataStart, payloadLen);
            byte[] bytes = Encoding.Latin1.GetBytes(rawText);
            return (true, bytes);
        }

        #endregion

        #region Custom Test Flow (Existing Flow)

        private async void btnCustomTest_Click(object sender, EventArgs e)
        {
            if (_testCts != null)
            {
                _testCts.Cancel();
                return;
            }

            if (_serialPort == null || !_serialPort.IsOpen)
            {
                MessageBox.Show("Please connect to a COM port before running the test!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _testCts = new CancellationTokenSource();
            btnCustomTest.Text = "⏹ Stop Flow";
            btnCustomTest.BackColor = Color.FromArgb(198, 40, 40);

            AppendLog("==================================================", LogType.Info);
            AppendLog(">>> STARTING CUSTOM TEST: HTTP DOWNLOAD & GTF FLOW", LogType.Info);
            AppendLog("==================================================", LogType.Info);

            try
            {
                await ExecuteCustomTestFlowAsync(_testCts.Token);
                AppendLog(">>> [PASSED] ALL TEST STEPS COMPLETED SUCCESSFULLY!", LogType.Info);
            }
            catch (OperationCanceledException)
            {
                AppendLog(">>> [STOPPED] Custom test was stopped by user.", LogType.Error);
            }
            catch (Exception ex)
            {
                AppendLog($">>> [FAILED] Custom test failed: {ex.Message}", LogType.Error);
            }
            finally
            {
                // Always close IP PDP context regardless of pass or fail
                if (_serialPort != null && _serialPort.IsOpen)
                {
                    try
                    {
                        AppendLog("--- Closing PDP context (at+mipcall=0,1) ---", LogType.Info);
                        await SendAndAwaitResponseAsync("at+mipcall=0,1", new[] { "OK" }, 6000, CancellationToken.None, postDelayMs: 500);
                    }
                    catch (Exception ex)
                    {
                        AppendLog($"Notice closing MIPCALL: {ex.Message}", LogType.Info);
                    }
                }

                _testCts?.Dispose();
                _testCts = null;
                btnCustomTest.Text = "⚡ Custom HTTP Flow";
                btnCustomTest.BackColor = Color.FromArgb(106, 27, 154);
            }
        }

        private async Task ExecuteCustomTestFlowAsync(CancellationToken ct)
        {
            AppendLog("--- [Step 1/11] Activate PDP context (at+mipcall=1) ---", LogType.Info);
            var r1 = await SendAndAwaitResponseAsync("at+mipcall=1", new[] { "OK" }, 12000, ct, postDelayMs: 2500);
            if (!r1.success && !r1.response.Contains("+MIPCALL:"))
            {
                throw new Exception("Step 1 failed: at+mipcall=1 did not return OK or +MIPCALL.");
            }
            await Task.Delay(1000, ct);

            AppendLog("--- [Step 2/11] Set HTTP Target URL ---", LogType.Info);
            var r2 = await SendAndAwaitResponseAsync("AT+HTTPSET=\"URL\",\"http://39.108.157.65:8000/test.txt\"", new[] { "OK" }, 6000, ct);
            if (!r2.success) throw new Exception("Step 2 failed: AT+HTTPSET URL did not return OK.");
            await Task.Delay(800, ct);

            string destFile = txtDownloadFile.Text.Trim();
            if (string.IsNullOrEmpty(destFile))
            {
                destFile = "/mnt/userdata/test.bin";
            }
            else if (!destFile.StartsWith("/"))
            {
                destFile = $"/mnt/userdata/{destFile}";
            }

            AppendLog($"--- [Step 3/11] Set download destination file path ({destFile}) ---", LogType.Info);
            var r3 = await SendAndAwaitResponseAsync($"AT+HTTPSET=\"DOWNLOADFILE\",\"{destFile}\"", new[] { "OK" }, 6000, ct);
            if (!r3.success) throw new Exception("Step 3 failed: AT+HTTPSET DOWNLOADFILE did not return OK.");
            await Task.Delay(800, ct);

            // Step 4: AT+HTTPSET? removed

            AppendLog("--- [Step 4/11] Execute HTTP Download (AT+HTTPACT=0,30) ---", LogType.Info);
            var r5 = await SendAndAwaitResponseAsync("AT+HTTPACT=0,30", new[] { "+HTTPRES:" }, 35000, ct, postDelayMs: 1000);
            if (!r5.success && !r5.response.Contains("OK"))
            {
                throw new Exception("Step 4 failed: AT+HTTPACT did not return +HTTPRES: or OK within 35s.");
            }
            AppendLog("Waiting 3s for module to finish writing data to flash storage...", LogType.Info);
            await Task.Delay(3000, ct);

            AppendLog("--- [Step 5/11] Check downloaded file in file list (AT+GTFLIST) ---", LogType.Info);
            var r6 = await SendAndAwaitResponseAsync("AT+GTFLIST", new[] { "OK" }, 6000, ct);
            if (!r6.success) throw new Exception("Step 5 failed: AT+GTFLIST did not return OK.");
            await Task.Delay(1000, ct);

            AppendLog($"--- [Step 6/11] Open downloaded file (AT+GTFOPEN=\"{destFile}\",0) ---", LogType.Info);
            var r7 = await SendAndAwaitResponseAsync($"AT+GTFOPEN=\"{destFile}\",0", new[] { "OK" }, 6000, ct);
            if (!r7.success) throw new Exception("Step 6 failed: AT+GTFOPEN failed.");

            string fileId = "41";
            var match = Regex.Match(r7.response, @"\+GTFOPEN:\s*(\d+)");
            if (match.Success)
            {
                fileId = match.Groups[1].Value;
                AppendLog($"[INFO] Parsed File Descriptor: {fileId}", LogType.Info);
            }
            await Task.Delay(1000, ct);

            AppendLog($"--- [Step 7/11] Query file size (AT+GTFSIZE={fileId}) ---", LogType.Info);
            var r8 = await SendAndAwaitResponseAsync($"AT+GTFSIZE={fileId}", new[] { "OK" }, 6000, ct);
            if (!r8.success) throw new Exception($"Step 7 failed: AT+GTFSIZE={fileId} failed.");
            await Task.Delay(1000, ct);

            AppendLog($"--- [Step 8/11] Read file in 8-byte chunks (AT+GTFREAD={fileId},8) ---", LogType.Info);
            for (int chunkIndex = 1; chunkIndex <= 4; chunkIndex++)
            {
                ct.ThrowIfCancellationRequested();
                AppendLog($"Reading chunk #{chunkIndex}...", LogType.Info);
                var rRead = await SendAndAwaitResponseAsync($"AT+GTFREAD={fileId},8", new[] { "OK" }, 6000, ct);
                if (!rRead.success)
                {
                    AppendLog($"Warning: Chunk #{chunkIndex} did not return OK.", LogType.Error);
                    break;
                }
                await Task.Delay(800, ct);
            }

            AppendLog($"--- [Step 9/11] Close file (at+GTFCLOSE={fileId}) ---", LogType.Info);
            var r10 = await SendAndAwaitResponseAsync($"at+GTFCLOSE={fileId}", new[] { "OK" }, 5000, ct);
            if (!r10.success) throw new Exception($"Step 9 failed: at+GTFCLOSE={fileId} failed.");
            await Task.Delay(1000, ct);

            AppendLog($"--- [Step 10/11] Delete downloaded file (AT+GTFDELETE=\"{destFile}\") ---", LogType.Info);
            var r11 = await SendAndAwaitResponseAsync($"AT+GTFDELETE=\"{destFile}\"", new[] { "OK" }, 5000, ct);
            if (!r11.success) throw new Exception("Step 10 failed: AT+GTFDELETE failed.");
            await Task.Delay(1000, ct);

            AppendLog("--- [Step 11/11] Final list check to verify deletion (AT+GTFLIST) ---", LogType.Info);
            var r12 = await SendAndAwaitResponseAsync("AT+GTFLIST", new[] { "OK" }, 5000, ct);
            if (!r12.success) throw new Exception("Step 11 failed: Final AT+GTFLIST failed.");
        }

        #endregion

        /// <summary>
        /// Sends an AT command and waits asynchronously until all expected tokens arrive or timeout occurs.
        /// </summary>
        private async Task<(bool success, string response)> SendAndAwaitResponseAsync(
            string command,
            string[] expectedTokens,
            int timeoutMs,
            CancellationToken ct,
            int postDelayMs = 400)
        {
            lock (_rxLock)
            {
                _rxBuffer = string.Empty;
                _rxSignal = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            }

            SendCommand(command);

            DateTime deadline = DateTime.Now.AddMilliseconds(timeoutMs);

            while (DateTime.Now < deadline)
            {
                ct.ThrowIfCancellationRequested();

                string currentBuffer;
                lock (_rxLock)
                {
                    currentBuffer = _rxBuffer;
                }

                bool allMatched = expectedTokens.Length > 0 &&
                                  expectedTokens.All(token => currentBuffer.Contains(token, StringComparison.OrdinalIgnoreCase));

                if (allMatched)
                {
                    if (postDelayMs > 0)
                    {
                        await Task.Delay(postDelayMs, ct);
                        lock (_rxLock)
                        {
                            currentBuffer = _rxBuffer;
                        }
                    }
                    return (true, currentBuffer);
                }

                if (currentBuffer.Contains("\r\nERROR\r\n") ||
                    currentBuffer.Contains("+CME ERROR:") ||
                    currentBuffer.Contains("+CMS ERROR:"))
                {
                    return (false, currentBuffer);
                }

                TaskCompletionSource<bool> signal;
                lock (_rxLock)
                {
                    if (_rxSignal == null || _rxSignal.Task.IsCompleted)
                    {
                        _rxSignal = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                    }
                    signal = _rxSignal;
                }

                int remainingMs = (int)(deadline - DateTime.Now).TotalMilliseconds;
                if (remainingMs <= 0) break;

                var waitTask = signal.Task;
                var delayTask = Task.Delay(Math.Min(remainingMs, 400), ct);
                await Task.WhenAny(waitTask, delayTask);
            }

            lock (_rxLock)
            {
                return (false, _rxBuffer);
            }
        }

        /// <summary>
        /// Output formatted log text to the terminal console.
        /// </summary>
        private void AppendLog(string message, LogType type)
        {
            if (rtbLog.IsDisposed) return;

            string timestamp = chkTimestamp.Checked ? $"[{DateTime.Now:HH:mm:ss.fff}] " : string.Empty;

            Color logColor = type switch
            {
                LogType.Tx => Color.FromArgb(77, 208, 225),   // Cyan / Light Blue
                LogType.Rx => Color.FromArgb(129, 199, 132),  // Light Green
                LogType.Error => Color.FromArgb(239, 83, 80), // Red
                LogType.Info => Color.FromArgb(255, 213, 79), // Amber / Yellow
                _ => Color.White
            };

            string prefix = type switch
            {
                LogType.Tx => "TX -> ",
                LogType.Rx => "RX <- ",
                LogType.Error => "[ERROR] ",
                LogType.Info => "[INFO] ",
                _ => ""
            };

            rtbLog.SelectionStart = rtbLog.TextLength;
            rtbLog.SelectionLength = 0;

            rtbLog.SelectionColor = Color.FromArgb(158, 158, 158);
            rtbLog.AppendText(timestamp);

            rtbLog.SelectionColor = logColor;
            rtbLog.AppendText(prefix + message);

            if (!message.EndsWith("\n"))
            {
                rtbLog.AppendText(Environment.NewLine);
            }

            if (chkAutoScroll.Checked)
            {
                rtbLog.ScrollToCaret();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DisconnectPort();
        }
    }
}
