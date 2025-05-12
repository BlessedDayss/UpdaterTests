namespace Updater.Common
{
    using System.Diagnostics;
    using Creatio.Updater;

    public interface IProcessUtility
    {
        bool StartProcess(string argumnets, string command, ProcessStartInfo processInfo);
    }

    public class ProcessUtility : IProcessUtility
    {

        public bool StartProcess(string argumnets, string command, ProcessStartInfo processInfo)
        {
            using var process = new Process
            {
                StartInfo = processInfo
            };
            process.Start();
            string errors = process.StandardError.ReadToEnd().Trim();
            process.WaitForExit();
            if (process.ExitCode == 0 && string.IsNullOrEmpty(errors))
            {
                ExtendedConsole.WriteLineInfo("Process started successfully.");
                return true;
            }
            ExtendedConsole.WriteLineWarning($"Can't run Process:\n\t{errors}. Please run command manually.\n");
            return false;
        }
    }
}