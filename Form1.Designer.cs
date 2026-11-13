namespace AT_command_test
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        // COM Connection
        private System.Windows.Forms.GroupBox grpConnection;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.ComboBox cboPort;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Label lblBaudRate;
        private System.Windows.Forms.ComboBox cboBaudRate;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Label lblStatus;

        // Quick AT Commands
        private System.Windows.Forms.GroupBox grpQuickCommands;
        private System.Windows.Forms.FlowLayoutPanel flpQuickCmd;
        private System.Windows.Forms.Button btnQuickAT;
        private System.Windows.Forms.Button btnQuickATI;
        private System.Windows.Forms.Button btnQuickATI7;
        private System.Windows.Forms.Button btnQuickCPIN;
        private System.Windows.Forms.Button btnQuickCSQ;
        private System.Windows.Forms.Button btnQuickCREG;
        private System.Windows.Forms.Button btnQuickCEREG;
        private System.Windows.Forms.Button btnQuickCOPS;
        private System.Windows.Forms.Button btnQuickCGATT;
        private System.Windows.Forms.Button btnQuickGTFLIST;
        private System.Windows.Forms.Button btnCustomTest;

        // 1MB Download & CRC16 Verification Section
        private System.Windows.Forms.GroupBox grp1MTest;
        private System.Windows.Forms.Label lblUrl;
        private System.Windows.Forms.TextBox txtHttpUrl;
        private System.Windows.Forms.Label lblChunkSize;
        private System.Windows.Forms.ComboBox cboChunkSize;
        private System.Windows.Forms.Label lblTimeout;
        private System.Windows.Forms.TextBox txtTimeout;
        private System.Windows.Forms.Label lblDownloadFile;
        private System.Windows.Forms.TextBox txtDownloadFile;
        private System.Windows.Forms.Label lblLocalFile;
        private System.Windows.Forms.TextBox txtLocalFile;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnCheckLocal;
        private System.Windows.Forms.Label lblLoopCount;
        private System.Windows.Forms.TextBox txtLoopCount;
        private System.Windows.Forms.Button btnRun1MTest;
        private System.Windows.Forms.Label lblCrcVerdict;
        private System.Windows.Forms.Label lblDownloadState;
        private System.Windows.Forms.ProgressBar prg1M;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.ToolTip toolTip1;

        // Terminal Log
        private System.Windows.Forms.GroupBox grpTerminal;
        private System.Windows.Forms.RichTextBox rtbLog;
        private System.Windows.Forms.Panel pnlTerminalHeader;
        private System.Windows.Forms.CheckBox chkAutoScroll;
        private System.Windows.Forms.CheckBox chkTimestamp;
        private System.Windows.Forms.Button btnClearLog;

        // Send Command Bar
        private System.Windows.Forms.Panel pnlSend;
        private System.Windows.Forms.Label lblCommand;
        private System.Windows.Forms.TextBox txtCommand;
        private System.Windows.Forms.CheckBox chkCRLF;
        private System.Windows.Forms.Button btnSend;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            grpConnection = new GroupBox();
            lblPort = new Label();
            cboPort = new ComboBox();
            btnRefresh = new Button();
            lblBaudRate = new Label();
            cboBaudRate = new ComboBox();
            btnConnect = new Button();
            lblStatus = new Label();
            grpQuickCommands = new GroupBox();
            flpQuickCmd = new FlowLayoutPanel();
            btnQuickAT = new Button();
            btnQuickATI = new Button();
            btnQuickATI7 = new Button();
            btnQuickCPIN = new Button();
            btnQuickCSQ = new Button();
            btnQuickCREG = new Button();
            btnQuickCEREG = new Button();
            btnQuickCOPS = new Button();
            btnQuickCGATT = new Button();
            btnQuickGTFLIST = new Button();
            btnCustomTest = new Button();
            grp1MTest = new GroupBox();
            lblUrl = new Label();
            txtHttpUrl = new TextBox();
            lblChunkSize = new Label();
            cboChunkSize = new ComboBox();
            lblTimeout = new Label();
            txtTimeout = new TextBox();
            lblDownloadFile = new Label();
            txtDownloadFile = new TextBox();
            lblLocalFile = new Label();
            txtLocalFile = new TextBox();
            btnBrowse = new Button();
            btnCheckLocal = new Button();
            lblLoopCount = new Label();
            txtLoopCount = new TextBox();
            btnRun1MTest = new Button();
            lblCrcVerdict = new Label();
            lblDownloadState = new Label();
            prg1M = new ProgressBar();
            lblProgress = new Label();
            toolTip1 = new ToolTip(components);
            grpTerminal = new GroupBox();
            rtbLog = new RichTextBox();
            pnlTerminalHeader = new Panel();
            chkAutoScroll = new CheckBox();
            chkTimestamp = new CheckBox();
            btnClearLog = new Button();
            pnlSend = new Panel();
            lblCommand = new Label();
            txtCommand = new TextBox();
            chkCRLF = new CheckBox();
            btnSend = new Button();
            grpConnection.SuspendLayout();
            grpQuickCommands.SuspendLayout();
            flpQuickCmd.SuspendLayout();
            grp1MTest.SuspendLayout();
            grpTerminal.SuspendLayout();
            pnlTerminalHeader.SuspendLayout();
            pnlSend.SuspendLayout();
            SuspendLayout();
            // 
            // grpConnection
            // 
            grpConnection.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grpConnection.Controls.Add(lblPort);
            grpConnection.Controls.Add(cboPort);
            grpConnection.Controls.Add(btnRefresh);
            grpConnection.Controls.Add(lblBaudRate);
            grpConnection.Controls.Add(cboBaudRate);
            grpConnection.Controls.Add(btnConnect);
            grpConnection.Controls.Add(lblStatus);
            grpConnection.Location = new Point(12, 10);
            grpConnection.Name = "grpConnection";
            grpConnection.Size = new Size(936, 62);
            grpConnection.TabIndex = 4;
            grpConnection.TabStop = false;
            grpConnection.Text = "COM Port Settings";
            // 
            // lblPort
            // 
            lblPort.AutoSize = true;
            lblPort.Location = new Point(12, 26);
            lblPort.Name = "lblPort";
            lblPort.Size = new Size(38, 20);
            lblPort.TabIndex = 0;
            lblPort.Text = "Port:";
            // 
            // cboPort
            // 
            cboPort.DropDownStyle = ComboBoxStyle.DropDownList;
            cboPort.Location = new Point(52, 23);
            cboPort.Name = "cboPort";
            cboPort.Size = new Size(125, 28);
            cboPort.TabIndex = 1;
            // 
            // btnRefresh
            // 
            btnRefresh.Location = new Point(185, 21);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(85, 27);
            btnRefresh.TabIndex = 2;
            btnRefresh.Text = "🔄 Refresh";
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Click += btnRefresh_Click;
            // 
            // lblBaudRate
            // 
            lblBaudRate.AutoSize = true;
            lblBaudRate.Location = new Point(285, 26);
            lblBaudRate.Name = "lblBaudRate";
            lblBaudRate.Size = new Size(80, 20);
            lblBaudRate.TabIndex = 3;
            lblBaudRate.Text = "Baud Rate:";
            // 
            // cboBaudRate
            // 
            cboBaudRate.DropDownStyle = ComboBoxStyle.DropDownList;
            cboBaudRate.Location = new Point(355, 23);
            cboBaudRate.Name = "cboBaudRate";
            cboBaudRate.Size = new Size(105, 28);
            cboBaudRate.TabIndex = 4;
            // 
            // btnConnect
            // 
            btnConnect.BackColor = Color.FromArgb(46, 125, 50);
            btnConnect.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnConnect.ForeColor = Color.White;
            btnConnect.Location = new Point(475, 20);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(100, 29);
            btnConnect.TabIndex = 5;
            btnConnect.Text = "Connect";
            btnConnect.UseVisualStyleBackColor = false;
            btnConnect.Click += btnConnect_Click;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblStatus.ForeColor = Color.Red;
            lblStatus.Location = new Point(590, 26);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(116, 20);
            lblStatus.TabIndex = 6;
            lblStatus.Text = "● Disconnected";
            // 
            // grpQuickCommands
            // 
            grpQuickCommands.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grpQuickCommands.Controls.Add(flpQuickCmd);
            grpQuickCommands.Location = new Point(12, 75);
            grpQuickCommands.Name = "grpQuickCommands";
            grpQuickCommands.Size = new Size(936, 95);
            grpQuickCommands.TabIndex = 3;
            grpQuickCommands.TabStop = false;
            grpQuickCommands.Text = "Quick AT Commands (Fibocom L716)";
            // 
            // flpQuickCmd
            // 
            flpQuickCmd.Controls.Add(btnQuickAT);
            flpQuickCmd.Controls.Add(btnQuickATI);
            flpQuickCmd.Controls.Add(btnQuickATI7);
            flpQuickCmd.Controls.Add(btnQuickCPIN);
            flpQuickCmd.Controls.Add(btnQuickCSQ);
            flpQuickCmd.Controls.Add(btnQuickCREG);
            flpQuickCmd.Controls.Add(btnQuickCEREG);
            flpQuickCmd.Controls.Add(btnQuickCOPS);
            flpQuickCmd.Controls.Add(btnQuickCGATT);
            flpQuickCmd.Controls.Add(btnQuickGTFLIST);
            flpQuickCmd.Controls.Add(btnCustomTest);
            flpQuickCmd.Dock = DockStyle.Fill;
            flpQuickCmd.Location = new Point(3, 23);
            flpQuickCmd.Name = "flpQuickCmd";
            flpQuickCmd.Padding = new Padding(4);
            flpQuickCmd.Size = new Size(930, 69);
            flpQuickCmd.TabIndex = 0;
            // 
            // btnQuickAT
            // 
            btnQuickAT.AutoSize = true;
            btnQuickAT.Location = new Point(7, 7);
            btnQuickAT.Name = "btnQuickAT";
            btnQuickAT.Size = new Size(75, 27);
            btnQuickAT.TabIndex = 0;
            btnQuickAT.Text = "AT";
            btnQuickAT.Tag = "AT";
            btnQuickAT.UseVisualStyleBackColor = true;
            btnQuickAT.Click += QuickCommand_Click;
            // 
            // btnQuickATI
            // 
            btnQuickATI.AutoSize = true;
            btnQuickATI.Location = new Point(88, 7);
            btnQuickATI.Name = "btnQuickATI";
            btnQuickATI.Size = new Size(75, 27);
            btnQuickATI.TabIndex = 1;
            btnQuickATI.Text = "ATI";
            btnQuickATI.Tag = "ATI";
            btnQuickATI.UseVisualStyleBackColor = true;
            btnQuickATI.Click += QuickCommand_Click;
            // 
            // btnQuickATI7
            // 
            btnQuickATI7.AutoSize = true;
            btnQuickATI7.Location = new Point(169, 7);
            btnQuickATI7.Name = "btnQuickATI7";
            btnQuickATI7.Size = new Size(95, 27);
            btnQuickATI7.TabIndex = 2;
            btnQuickATI7.Text = "ATI7 (Ver)";
            btnQuickATI7.Tag = "ATI7";
            btnQuickATI7.UseVisualStyleBackColor = true;
            btnQuickATI7.Click += QuickCommand_Click;
            // 
            // btnQuickCPIN
            // 
            btnQuickCPIN.AutoSize = true;
            btnQuickCPIN.Location = new Point(270, 7);
            btnQuickCPIN.Name = "btnQuickCPIN";
            btnQuickCPIN.Size = new Size(88, 27);
            btnQuickCPIN.TabIndex = 3;
            btnQuickCPIN.Text = "AT+CPIN?";
            btnQuickCPIN.Tag = "AT+CPIN?";
            btnQuickCPIN.UseVisualStyleBackColor = true;
            btnQuickCPIN.Click += QuickCommand_Click;
            // 
            // btnQuickCSQ
            // 
            btnQuickCSQ.AutoSize = true;
            btnQuickCSQ.Location = new Point(364, 7);
            btnQuickCSQ.Name = "btnQuickCSQ";
            btnQuickCSQ.Size = new Size(80, 27);
            btnQuickCSQ.TabIndex = 4;
            btnQuickCSQ.Text = "AT+CSQ";
            btnQuickCSQ.Tag = "AT+CSQ";
            btnQuickCSQ.UseVisualStyleBackColor = true;
            btnQuickCSQ.Click += QuickCommand_Click;
            // 
            // btnQuickCREG
            // 
            btnQuickCREG.AutoSize = true;
            btnQuickCREG.Location = new Point(450, 7);
            btnQuickCREG.Name = "btnQuickCREG";
            btnQuickCREG.Size = new Size(90, 27);
            btnQuickCREG.TabIndex = 5;
            btnQuickCREG.Text = "AT+CREG?";
            btnQuickCREG.Tag = "AT+CREG?";
            btnQuickCREG.UseVisualStyleBackColor = true;
            btnQuickCREG.Click += QuickCommand_Click;
            // 
            // btnQuickCEREG
            // 
            btnQuickCEREG.AutoSize = true;
            btnQuickCEREG.Location = new Point(546, 7);
            btnQuickCEREG.Name = "btnQuickCEREG";
            btnQuickCEREG.Size = new Size(98, 27);
            btnQuickCEREG.TabIndex = 6;
            btnQuickCEREG.Text = "AT+CEREG?";
            btnQuickCEREG.Tag = "AT+CEREG?";
            btnQuickCEREG.UseVisualStyleBackColor = true;
            btnQuickCEREG.Click += QuickCommand_Click;
            // 
            // btnQuickCOPS
            // 
            btnQuickCOPS.AutoSize = true;
            btnQuickCOPS.Location = new Point(650, 7);
            btnQuickCOPS.Name = "btnQuickCOPS";
            btnQuickCOPS.Size = new Size(91, 27);
            btnQuickCOPS.TabIndex = 7;
            btnQuickCOPS.Text = "AT+COPS?";
            btnQuickCOPS.Tag = "AT+COPS?";
            btnQuickCOPS.UseVisualStyleBackColor = true;
            btnQuickCOPS.Click += QuickCommand_Click;
            // 
            // btnQuickCGATT
            // 
            btnQuickCGATT.AutoSize = true;
            btnQuickCGATT.Location = new Point(747, 7);
            btnQuickCGATT.Name = "btnQuickCGATT";
            btnQuickCGATT.Size = new Size(97, 27);
            btnQuickCGATT.TabIndex = 8;
            btnQuickCGATT.Text = "AT+CGATT?";
            btnQuickCGATT.Tag = "AT+CGATT?";
            btnQuickCGATT.UseVisualStyleBackColor = true;
            btnQuickCGATT.Click += QuickCommand_Click;
            // 
            // btnQuickGTFLIST
            // 
            btnQuickGTFLIST.AutoSize = true;
            btnQuickGTFLIST.BackColor = Color.FromArgb(0, 121, 107);
            btnQuickGTFLIST.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnQuickGTFLIST.ForeColor = Color.White;
            btnQuickGTFLIST.Location = new Point(850, 7);
            btnQuickGTFLIST.Name = "btnQuickGTFLIST";
            btnQuickGTFLIST.Size = new Size(115, 27);
            btnQuickGTFLIST.TabIndex = 9;
            btnQuickGTFLIST.Text = "📁 AT+GTFLIST";
            btnQuickGTFLIST.Tag = "AT+GTFLIST";
            btnQuickGTFLIST.UseVisualStyleBackColor = false;
            btnQuickGTFLIST.Click += QuickCommand_Click;
            // 
            // btnCustomTest
            // 
            btnCustomTest.AutoSize = true;
            btnCustomTest.BackColor = Color.FromArgb(106, 27, 154);
            btnCustomTest.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnCustomTest.ForeColor = Color.White;
            btnCustomTest.Location = new Point(10, 34);
            btnCustomTest.Margin = new Padding(6, 1, 3, 1);
            btnCustomTest.Name = "btnCustomTest";
            btnCustomTest.Size = new Size(178, 30);
            btnCustomTest.TabIndex = 10;
            btnCustomTest.Text = "⚡ Custom HTTP Flow";
            btnCustomTest.UseVisualStyleBackColor = false;
            btnCustomTest.Click += btnCustomTest_Click;
            // 
            // grp1MTest
            // 
            grp1MTest.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grp1MTest.Controls.Add(lblUrl);
            grp1MTest.Controls.Add(txtHttpUrl);
            grp1MTest.Controls.Add(lblChunkSize);
            grp1MTest.Controls.Add(cboChunkSize);
            grp1MTest.Controls.Add(lblTimeout);
            grp1MTest.Controls.Add(txtTimeout);
            grp1MTest.Controls.Add(lblDownloadFile);
            grp1MTest.Controls.Add(txtDownloadFile);
            grp1MTest.Controls.Add(lblLocalFile);
            grp1MTest.Controls.Add(txtLocalFile);
            grp1MTest.Controls.Add(btnBrowse);
            grp1MTest.Controls.Add(btnCheckLocal);
            grp1MTest.Controls.Add(lblLoopCount);
            grp1MTest.Controls.Add(txtLoopCount);
            grp1MTest.Controls.Add(btnRun1MTest);
            grp1MTest.Controls.Add(lblCrcVerdict);
            grp1MTest.Controls.Add(lblDownloadState);
            grp1MTest.Controls.Add(prg1M);
            grp1MTest.Controls.Add(lblProgress);
            grp1MTest.Location = new Point(12, 175);
            grp1MTest.Name = "grp1MTest";
            grp1MTest.Size = new Size(936, 226);
            grp1MTest.TabIndex = 2;
            grp1MTest.TabStop = false;
            grp1MTest.Text = "HTTP File Download & CRC16 Verification";
            // 
            // lblUrl
            // 
            lblUrl.AutoSize = true;
            lblUrl.Location = new Point(12, 25);
            lblUrl.Name = "lblUrl";
            lblUrl.Size = new Size(83, 20);
            lblUrl.TabIndex = 0;
            lblUrl.Text = "Server URL:";
            // 
            // txtHttpUrl
            // 
            txtHttpUrl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtHttpUrl.Location = new Point(108, 22);
            txtHttpUrl.Name = "txtHttpUrl";
            txtHttpUrl.Size = new Size(442, 27);
            txtHttpUrl.TabIndex = 1;
            txtHttpUrl.Text = "http://39.108.157.65:8000/1M.txt";
            txtHttpUrl.TextChanged += txtHttpUrl_TextChanged;
            // 
            // lblChunkSize
            // 
            lblChunkSize.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblChunkSize.AutoSize = true;
            lblChunkSize.Location = new Point(560, 25);
            lblChunkSize.Name = "lblChunkSize";
            lblChunkSize.Size = new Size(90, 20);
            lblChunkSize.TabIndex = 2;
            lblChunkSize.Text = "Read Chunk:";
            // 
            // cboChunkSize
            // 
            cboChunkSize.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            cboChunkSize.Items.AddRange(new object[] { "200 KB", "100 KB", "64 KB", "32 KB", "16 KB", "4 KB", "1 KB" });
            cboChunkSize.Location = new Point(645, 22);
            cboChunkSize.Name = "cboChunkSize";
            cboChunkSize.Size = new Size(95, 28);
            cboChunkSize.TabIndex = 3;
            cboChunkSize.Text = "200 KB";
            // 
            // lblTimeout
            // 
            lblTimeout.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblTimeout.AutoSize = true;
            lblTimeout.Location = new Point(755, 25);
            lblTimeout.Name = "lblTimeout";
            lblTimeout.Size = new Size(83, 20);
            lblTimeout.TabIndex = 4;
            lblTimeout.Text = "Timeout(s):";
            // 
            // txtTimeout
            // 
            txtTimeout.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            txtTimeout.Location = new Point(835, 22);
            txtTimeout.Name = "txtTimeout";
            txtTimeout.Size = new Size(75, 27);
            txtTimeout.TabIndex = 5;
            txtTimeout.Text = "60";
            // 
            // lblDownloadFile
            // 
            lblDownloadFile.AutoSize = true;
            lblDownloadFile.Location = new Point(12, 55);
            lblDownloadFile.Name = "lblDownloadFile";
            lblDownloadFile.Size = new Size(90, 20);
            lblDownloadFile.TabIndex = 6;
            lblDownloadFile.Text = "Module File:";
            // 
            // txtDownloadFile
            // 
            txtDownloadFile.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtDownloadFile.Location = new Point(108, 52);
            txtDownloadFile.Name = "txtDownloadFile";
            txtDownloadFile.Size = new Size(802, 27);
            txtDownloadFile.TabIndex = 7;
            txtDownloadFile.Text = "/mnt/userdata/1M.txt";
            txtDownloadFile.TextChanged += txtDownloadFile_TextChanged;
            // 
            // lblLocalFile
            // 
            lblLocalFile.AutoSize = true;
            lblLocalFile.Location = new Point(12, 85);
            lblLocalFile.Name = "lblLocalFile";
            lblLocalFile.Size = new Size(73, 20);
            lblLocalFile.TabIndex = 8;
            lblLocalFile.Text = "Local Ref:";
            // 
            // txtLocalFile
            // 
            txtLocalFile.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtLocalFile.Location = new Point(108, 82);
            txtLocalFile.Name = "txtLocalFile";
            txtLocalFile.Size = new Size(707, 27);
            txtLocalFile.TabIndex = 9;
            txtLocalFile.Text = "1M.txt";
            txtLocalFile.TextChanged += txtLocalFile_TextChanged;
            // 
            // btnBrowse
            // 
            btnBrowse.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBrowse.Location = new Point(825, 80);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new Size(85, 27);
            btnBrowse.TabIndex = 10;
            btnBrowse.Text = "Browse...";
            btnBrowse.Click += btnBrowse_Click;
            // 
            // btnCheckLocal
            // 
            btnCheckLocal.BackColor = Color.FromArgb(55, 71, 79);
            btnCheckLocal.ForeColor = Color.White;
            btnCheckLocal.Location = new Point(12, 115);
            btnCheckLocal.Name = "btnCheckLocal";
            btnCheckLocal.Size = new Size(160, 29);
            btnCheckLocal.TabIndex = 11;
            btnCheckLocal.Text = "🔍 Check Local CRC16";
            btnCheckLocal.UseVisualStyleBackColor = false;
            btnCheckLocal.Click += btnCheckLocal_Click;
            // 
            // lblLoopCount
            // 
            lblLoopCount.AutoSize = true;
            lblLoopCount.Location = new Point(178, 119);
            lblLoopCount.Name = "lblLoopCount";
            lblLoopCount.Size = new Size(45, 20);
            lblLoopCount.TabIndex = 12;
            lblLoopCount.Text = "Loop:";
            // 
            // txtLoopCount
            // 
            txtLoopCount.Location = new Point(227, 116);
            txtLoopCount.Name = "txtLoopCount";
            txtLoopCount.Size = new Size(50, 27);
            txtLoopCount.TabIndex = 13;
            txtLoopCount.Text = "0";
            // 
            // btnRun1MTest
            // 
            btnRun1MTest.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btnRun1MTest.BackColor = Color.FromArgb(230, 81, 0);
            btnRun1MTest.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnRun1MTest.ForeColor = Color.White;
            btnRun1MTest.Location = new Point(287, 115);
            btnRun1MTest.Name = "btnRun1MTest";
            btnRun1MTest.Size = new Size(623, 29);
            btnRun1MTest.TabIndex = 14;
            btnRun1MTest.Text = "🚀 Download & Verify CRC16";
            btnRun1MTest.UseVisualStyleBackColor = false;
            btnRun1MTest.Click += btnRun1MTest_Click;
            // 
            // lblCrcVerdict
            // 
            lblCrcVerdict.AutoSize = true;
            lblCrcVerdict.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblCrcVerdict.ForeColor = Color.FromArgb(33, 33, 33);
            lblCrcVerdict.Location = new Point(12, 148);
            lblCrcVerdict.Name = "lblCrcVerdict";
            lblCrcVerdict.Size = new Size(164, 21);
            lblCrcVerdict.TabIndex = 13;
            lblCrcVerdict.Text = "CRC16 Status: Ready";
            // 
            // lblDownloadState
            // 
            lblDownloadState.AutoSize = true;
            lblDownloadState.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            lblDownloadState.ForeColor = Color.FromArgb(21, 101, 192);
            lblDownloadState.Location = new Point(12, 172);
            lblDownloadState.Name = "lblDownloadState";
            lblDownloadState.Size = new Size(160, 20);
            lblDownloadState.TabIndex = 14;
            lblDownloadState.Text = "State: Idle";
            // 
            // prg1M
            // 
            prg1M.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            prg1M.Location = new Point(12, 196);
            prg1M.Name = "prg1M";
            prg1M.Size = new Size(670, 20);
            prg1M.TabIndex = 15;
            // 
            // lblProgress
            // 
            lblProgress.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblProgress.Location = new Point(690, 196);
            lblProgress.Name = "lblProgress";
            lblProgress.Size = new Size(225, 20);
            lblProgress.TabIndex = 16;
            lblProgress.Text = "0 / 1,048,576 bytes (0%)";
            // 
            // grpTerminal
            // 
            grpTerminal.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grpTerminal.Controls.Add(rtbLog);
            grpTerminal.Controls.Add(pnlTerminalHeader);
            grpTerminal.Location = new Point(12, 407);
            grpTerminal.Name = "grpTerminal";
            grpTerminal.Size = new Size(936, 272);
            grpTerminal.TabIndex = 1;
            grpTerminal.TabStop = false;
            grpTerminal.Text = "Terminal Console";
            // 
            // rtbLog
            // 
            rtbLog.BackColor = Color.FromArgb(30, 30, 30);
            rtbLog.Dock = DockStyle.Fill;
            rtbLog.Font = new Font("Consolas", 10F);
            rtbLog.ForeColor = Color.FromArgb(220, 220, 220);
            rtbLog.Location = new Point(3, 55);
            rtbLog.Name = "rtbLog";
            rtbLog.ReadOnly = true;
            rtbLog.Size = new Size(930, 232);
            rtbLog.TabIndex = 0;
            rtbLog.Text = "";
            // 
            // pnlTerminalHeader
            // 
            pnlTerminalHeader.Controls.Add(chkAutoScroll);
            pnlTerminalHeader.Controls.Add(chkTimestamp);
            pnlTerminalHeader.Controls.Add(btnClearLog);
            pnlTerminalHeader.Dock = DockStyle.Top;
            pnlTerminalHeader.Location = new Point(3, 23);
            pnlTerminalHeader.Name = "pnlTerminalHeader";
            pnlTerminalHeader.Size = new Size(930, 32);
            pnlTerminalHeader.TabIndex = 1;
            // 
            // chkAutoScroll
            // 
            chkAutoScroll.AutoSize = true;
            chkAutoScroll.Checked = true;
            chkAutoScroll.CheckState = CheckState.Checked;
            chkAutoScroll.Location = new Point(8, 6);
            chkAutoScroll.Name = "chkAutoScroll";
            chkAutoScroll.Size = new Size(104, 24);
            chkAutoScroll.TabIndex = 0;
            chkAutoScroll.Text = "Auto-scroll";
            // 
            // chkTimestamp
            // 
            chkTimestamp.AutoSize = true;
            chkTimestamp.Checked = true;
            chkTimestamp.CheckState = CheckState.Checked;
            chkTimestamp.Location = new Point(105, 6);
            chkTimestamp.Name = "chkTimestamp";
            chkTimestamp.Size = new Size(222, 24);
            chkTimestamp.TabIndex = 1;
            chkTimestamp.Text = "Show timestamp [HH:mm:ss]";
            // 
            // btnClearLog
            // 
            btnClearLog.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClearLog.Location = new Point(1560, 2);
            btnClearLog.Name = "btnClearLog";
            btnClearLog.Size = new Size(95, 26);
            btnClearLog.TabIndex = 2;
            btnClearLog.Text = "🗑 Clear Log";
            btnClearLog.Click += btnClearLog_Click;
            // 
            // pnlSend
            // 
            pnlSend.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnlSend.Controls.Add(lblCommand);
            pnlSend.Controls.Add(txtCommand);
            pnlSend.Controls.Add(chkCRLF);
            pnlSend.Controls.Add(btnSend);
            pnlSend.Location = new Point(12, 690);
            pnlSend.Name = "pnlSend";
            pnlSend.Size = new Size(936, 50);
            pnlSend.TabIndex = 0;
            // 
            // lblCommand
            // 
            lblCommand.AutoSize = true;
            lblCommand.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblCommand.Location = new Point(5, 15);
            lblCommand.Name = "lblCommand";
            lblCommand.Size = new Size(85, 20);
            lblCommand.TabIndex = 0;
            lblCommand.Text = "Command:";
            // 
            // txtCommand
            // 
            txtCommand.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtCommand.Font = new Font("Consolas", 10F);
            txtCommand.Location = new Point(96, 11);
            txtCommand.Name = "txtCommand";
            txtCommand.Size = new Size(584, 27);
            txtCommand.TabIndex = 1;
            txtCommand.KeyDown += txtCommand_KeyDown;
            // 
            // chkCRLF
            // 
            chkCRLF.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            chkCRLF.Checked = true;
            chkCRLF.CheckState = CheckState.Checked;
            chkCRLF.Location = new Point(690, 15);
            chkCRLF.Name = "chkCRLF";
            chkCRLF.Size = new Size(140, 20);
            chkCRLF.TabIndex = 2;
            chkCRLF.Text = "Append CR+LF (\\r\\n)";
            // 
            // btnSend
            // 
            btnSend.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSend.BackColor = Color.FromArgb(25, 118, 210);
            btnSend.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnSend.ForeColor = Color.White;
            btnSend.Location = new Point(838, 9);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(92, 31);
            btnSend.TabIndex = 3;
            btnSend.Text = "➤ Send";
            btnSend.UseVisualStyleBackColor = false;
            btnSend.Click += btnSend_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(980, 760);
            Controls.Add(pnlSend);
            Controls.Add(grpTerminal);
            Controls.Add(grp1MTest);
            Controls.Add(grpQuickCommands);
            Controls.Add(grpConnection);
            Font = new Font("Segoe UI", 9F);
            MinimumSize = new Size(920, 680);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Fibocom AT Command Tester - COM Port & 1MB CRC16 Verification";
            FormClosing += Form1_FormClosing;
            Load += Form1_Load;
            grpConnection.ResumeLayout(false);
            grpConnection.PerformLayout();
            grpQuickCommands.ResumeLayout(false);
            flpQuickCmd.ResumeLayout(false);
            flpQuickCmd.PerformLayout();
            grp1MTest.ResumeLayout(false);
            grp1MTest.PerformLayout();
            grpTerminal.ResumeLayout(false);
            pnlTerminalHeader.ResumeLayout(false);
            pnlTerminalHeader.PerformLayout();
            pnlSend.ResumeLayout(false);
            pnlSend.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
    }
}
