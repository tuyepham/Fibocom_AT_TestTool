using System.Security.Cryptography;

namespace AT_command_test
{
    public static class Crc16Helper
    {
        public const ushort CRC16_CCITT_INIT = 0xFFFF;
        public const ushort CRC16_MODBUS_INIT = 0xFFFF;
        public const ushort CRC16_XMODEM_INIT = 0x0000;

        #region Incremental / Rolling / Chunk-by-Chunk Calculations

        /// <summary>
        /// Updates running CRC-16-CCITT (False, Poly 0x1021) with a new chunk of bytes.
        /// Start with CRC16_CCITT_INIT (0xFFFF).
        /// </summary>
        public static ushort UpdateCrc16Ccitt(ushort currentCrc, byte[] chunk, int offset = 0, int count = -1)
        {
            if (count < 0) count = chunk.Length - offset;
            ushort crc = currentCrc;
            for (int i = offset; i < offset + count; i++)
            {
                crc ^= (ushort)(chunk[i] << 8);
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 0x8000) != 0)
                        crc = (ushort)((crc << 1) ^ 0x1021);
                    else
                        crc = (ushort)(crc << 1);
                }
            }
            return crc;
        }

        /// <summary>
        /// Updates running CRC-16-Modbus (Poly 0x8005 / Reflected 0xA001) with a new chunk of bytes.
        /// Start with CRC16_MODBUS_INIT (0xFFFF).
        /// </summary>
        public static ushort UpdateCrc16Modbus(ushort currentCrc, byte[] chunk, int offset = 0, int count = -1)
        {
            if (count < 0) count = chunk.Length - offset;
            ushort crc = currentCrc;
            for (int i = offset; i < offset + count; i++)
            {
                crc ^= chunk[i];
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 0x0001) != 0)
                        crc = (ushort)((crc >> 1) ^ 0xA001);
                    else
                        crc = (ushort)(crc >> 1);
                }
            }
            return crc;
        }

        /// <summary>
        /// Updates running CRC-16-XMODEM (Poly 0x1021, Init 0x0000) with a new chunk of bytes.
        /// </summary>
        public static ushort UpdateCrc16Xmodem(ushort currentCrc, byte[] chunk, int offset = 0, int count = -1)
        {
            if (count < 0) count = chunk.Length - offset;
            ushort crc = currentCrc;
            for (int i = offset; i < offset + count; i++)
            {
                crc ^= (ushort)(chunk[i] << 8);
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 0x8000) != 0)
                        crc = (ushort)((crc << 1) ^ 0x1021);
                    else
                        crc = (ushort)(crc << 1);
                }
            }
            return crc;
        }

        #endregion

        #region Complete Block Calculations

        public static ushort ComputeCrc16Ccitt(byte[] data)
        {
            return UpdateCrc16Ccitt(CRC16_CCITT_INIT, data);
        }

        public static ushort ComputeCrc16Modbus(byte[] data)
        {
            return UpdateCrc16Modbus(CRC16_MODBUS_INIT, data);
        }

        public static ushort ComputeCrc16Xmodem(byte[] data)
        {
            return UpdateCrc16Xmodem(CRC16_XMODEM_INIT, data);
        }

        public static string ComputeMd5(byte[] data)
        {
            using var md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(data);
            return Convert.ToHexString(hash);
        }

        #endregion

        #region Streaming / Rolling Checksum Calculator

        /// <summary>
        /// Streamable Checksum accumulator. Processes chunks incrementally with minimal memory footprint (O(1) RAM).
        /// </summary>
        public class RollingChecksumCalculator : IDisposable
        {
            public ushort Crc16Ccitt { get; private set; } = CRC16_CCITT_INIT;
            public ushort Crc16Modbus { get; private set; } = CRC16_MODBUS_INIT;
            public ushort Crc16Xmodem { get; private set; } = CRC16_XMODEM_INIT;
            public long TotalBytesProcessed { get; private set; } = 0;

            private readonly IncrementalHash _md5 = IncrementalHash.CreateHash(HashAlgorithmName.MD5);

            public void AppendChunk(byte[] chunk, int offset = 0, int count = -1)
            {
                if (count < 0) count = chunk.Length - offset;
                if (count <= 0) return;

                Crc16Ccitt = UpdateCrc16Ccitt(Crc16Ccitt, chunk, offset, count);
                Crc16Modbus = UpdateCrc16Modbus(Crc16Modbus, chunk, offset, count);
                Crc16Xmodem = UpdateCrc16Xmodem(Crc16Xmodem, chunk, offset, count);
                _md5.AppendData(chunk, offset, count);

                TotalBytesProcessed += count;
            }

            public ChecksumReport FinalizeReport()
            {
                byte[] hash = _md5.GetHashAndReset();
                return new ChecksumReport
                {
                    ByteCount = (int)TotalBytesProcessed,
                    Crc16Ccitt = Crc16Ccitt,
                    Crc16Modbus = Crc16Modbus,
                    Crc16Xmodem = Crc16Xmodem,
                    Md5 = Convert.ToHexString(hash)
                };
            }

            public void Dispose()
            {
                _md5.Dispose();
            }
        }

        #endregion

        public class ChecksumReport
        {
            public int ByteCount { get; set; }
            public ushort Crc16Ccitt { get; set; }
            public ushort Crc16Modbus { get; set; }
            public ushort Crc16Xmodem { get; set; }
            public string Md5 { get; set; } = string.Empty;

            public override string ToString()
            {
                return $"Size: {ByteCount:N0} bytes | CRC16-CCITT: 0x{Crc16Ccitt:X4} | CRC16-Modbus: 0x{Crc16Modbus:X4} | MD5: {Md5}";
            }
        }

        public static ChecksumReport Analyze(byte[] data)
        {
            return new ChecksumReport
            {
                ByteCount = data.Length,
                Crc16Ccitt = ComputeCrc16Ccitt(data),
                Crc16Modbus = ComputeCrc16Modbus(data),
                Crc16Xmodem = ComputeCrc16Xmodem(data),
                Md5 = ComputeMd5(data)
            };
        }

        /// <summary>
        /// Reads a large file from disk in streams without loading the entire file into RAM at once.
        /// </summary>
        public static async Task<ChecksumReport> AnalyzeFileStreamAsync(string filePath, CancellationToken ct = default)
        {
            using var calc = new RollingChecksumCalculator();
            byte[] buffer = new byte[8192];

            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 8192, useAsync: true);
            int bytesRead;
            while ((bytesRead = await fs.ReadAsync(buffer, 0, buffer.Length, ct)) > 0)
            {
                calc.AppendChunk(buffer, 0, bytesRead);
            }

            return calc.FinalizeReport();
        }
    }
}
