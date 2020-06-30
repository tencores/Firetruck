using System;
using System.IO;

namespace FiretruckRouteGenerator.Utils
{
    public class OutputWriter : IDisposable
    {
        private bool _disposed;

        private const string APPDATA_PATH = "Firetruck Route Generator";
        private const string DEFAULT_FILE = "result.txt";
        private readonly TextWriter _outputWriter;


        public OutputWriter(string logFilePath = null, bool append = true)
        {
            var fullPath = logFilePath;

            if (fullPath is null)
            {
                fullPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    APPDATA_PATH);

                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                }
                fullPath = Path.Combine(fullPath, DEFAULT_FILE);
            }

            _outputWriter = new StreamWriter(fullPath, append);
        }

        ~OutputWriter()
        {
            Dispose(false);
        }

        public void PrintStartSession()
        {
            if (_disposed) return;

            _outputWriter.WriteLine("***********************************");
            _outputWriter.WriteLine("*********** New Session ***********");
            _outputWriter.WriteLine("***********************************");
            _outputWriter.WriteLine("");
        }

        public void PrintEndSession()
        {
            _outputWriter.WriteLine("");
            _outputWriter.WriteLine("***********************************");
            _outputWriter.WriteLine("");
        }

        public void Write(string log)
        {
            if (_disposed) return;

            _outputWriter.Write(log);
        }

        public void WriteLine(string log)
        {
            if (_disposed) return;

            _outputWriter.WriteLine(log);
        }

        public void Flush()
        {
            if (_disposed) return;

            _outputWriter.Flush();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _outputWriter.Flush();

                _outputWriter.Close();
            }

            _disposed = true;
        }
    }
}