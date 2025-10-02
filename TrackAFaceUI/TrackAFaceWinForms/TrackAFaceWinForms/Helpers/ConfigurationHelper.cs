using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackAFaceWinForms.Helpers
{
    public static class ConfigurationHelper
    {
        private const string DEFAULT_PYTHON_PATH = "python";
        private const string DEFAULT_ENGINE_API_FILENAME = "engine_api.py";
        private const string DEFAULT_SESSIONS_DIR = "sessions";
        private const string DEFAULT_LOGS_DIR = "logs";
        private const string DEFAULT_EXPORTS_DIR = "exports";
        public static string PythonPath
        {
            get
            {
                string configPath = ConfigurationManager.AppSettings["PythonPath"];
                return !string.IsNullOrWhiteSpace(configPath) ? configPath : DEFAULT_PYTHON_PATH;
            }
        }

        public static string EngineApiPath
        {
            get
            {
                string configPath = ConfigurationManager.AppSettings["EngineApiPath"];
                if (!string.IsNullOrWhiteSpace(configPath))
                {
                    return Path.GetFullPath(configPath);
                }

                string currentDir = AppDomain.CurrentDomain.BaseDirectory;
                string projectRoot = Directory.GetParent(currentDir).Parent.Parent.Parent.Parent.FullName;
                return Path.Combine(projectRoot, DEFAULT_ENGINE_API_FILENAME);
            }
        }

        public static string SessionsDirectory
        {
            get
            {
                string configDir = ConfigurationManager.AppSettings["SessionsDirectory"];
                string dir = string.IsNullOrWhiteSpace(configDir) ? DEFAULT_SESSIONS_DIR : configDir;
                string fullPath = Path.IsPathRooted(dir) ? dir : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dir);
                EnsureDirectoryExists(fullPath);
                return fullPath;
            }
        }

        public static string LogsDirectory
        {
            get
            {
                string configDir = ConfigurationManager.AppSettings["LogsDirectory"];
                string dir = string.IsNullOrWhiteSpace(configDir) ? DEFAULT_LOGS_DIR : configDir;
                string fullPath = Path.IsPathRooted(dir) ? dir : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dir);
                EnsureDirectoryExists(fullPath);
                return fullPath;
            }
        }

        public static string ExportsDirectory
        {
            get
            {
                string configDir = ConfigurationManager.AppSettings["ExportsDirectory"];
                string dir = string.IsNullOrWhiteSpace(configDir) ? DEFAULT_EXPORTS_DIR : configDir;
                string fullPath = Path.IsPathRooted(dir) ? dir : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dir);
                EnsureDirectoryExists(fullPath);
                return fullPath;
            }
        }

        public static bool IsPythonAvailable()
        {
            try
            {
                var process = new System.Diagnostics.Process
                {
                    StartInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = PythonPath,
                        Arguments = "--version",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    }
                };
                process.Start();
                process.WaitForExit(5000);
                return process.ExitCode == 0;
            }
            catch { return false; }
        }

        public static bool IsEngineApiAvailable()
        {
            return File.Exists(EngineApiPath);
        }

        public static string GetConfigurationSummary()
        {
            return $@"Configuration Track-A-FACE:
            Python: {PythonPath} - {(IsPythonAvailable() ? "✓" : "✗")}
            Engine: {EngineApiPath} - {(IsEngineApiAvailable() ? "✓" : "✗")}
            Sessions: {SessionsDirectory}
            Logs: {LogsDirectory}
            Exports: {ExportsDirectory}";
                    }

        private static void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
