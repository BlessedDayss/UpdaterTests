namespace Updater.Common
{
    using System.Diagnostics;
    using Creatio.Updater;

    #region Interface: IProcessUtility

    public interface IProcessUtility
    {

        #region Methods: Public

        bool StartProcess(ProcessStartInfo processInfo);

        #endregion

    }

    #endregion

    #region Class: ProcessUtility

    public class ProcessUtility : IProcessUtility
    {

        #region Methods: Public

        public bool StartProcess(ProcessStartInfo processInfo)
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

        #endregion

    }

    #endregion

}