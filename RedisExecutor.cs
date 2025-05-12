namespace Creatio.Updater
{
	using System;
	using System.Diagnostics;
	using Creatio.Updater.Configuration;


	public interface IProcUtility
	{
		bool StartProcess(string argumnets, string command, ProcessStartInfo processInfo);
	}

	public class ProcUtility : IProcUtility
	{

		public bool StartProcess(string argumnets, string command, ProcessStartInfo processInfo) {
			using var process = new Process {
				StartInfo = processInfo
			};
			process.Start();
			string errors = process.StandardError.ReadToEnd().Trim();
			process.WaitForExit();
			if (process.ExitCode == 0 && string.IsNullOrEmpty(errors)) {
				ExtendedConsole.WriteLineInfo("Process started successfully.");
				return true;
			}
			ExtendedConsole.WriteLineWarning($"Can't clean Process:\n\t{errors}. Please run command manually.\n");
			return false;
		}
	}

	public static class RedisExecutor
	{
		public static bool ClearRedisCache(ISiteInfo siteInfo, IProcUtility procUtility) {
			if (UpdaterConfig.GetFeature("SkipClearRedisCache")) {
				return true;
			}
			if (Environment.ExitCode != 0) {
				ExtendedConsole.WriteLineWarning("\nRedis cache was not cleared. Please successfully update the site first.\n");
				return false;
			}
			string arguments = !string.IsNullOrEmpty(siteInfo.RedisPassword)
				? $"-h {siteInfo.RedisServer} -p {siteInfo.RedisPort} -n {siteInfo.RedisDB} --no-auth-warning -a {siteInfo.RedisPassword} flushdb"
				: $"-h {siteInfo.RedisServer} -p {siteInfo.RedisPort} -n {siteInfo.RedisDB} flushdb";
			return ExecuteCommand(arguments, procUtility);
		}

		private static bool ExecuteCommand(string arguments, IProcUtility procUtility) {
			const string command = "redis-cli";
			try {
				var processInfo = new ProcessStartInfo {
					FileName = command,
					Arguments = arguments,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					UseShellExecute = false,
					CreateNoWindow = true
				};
				return procUtility.StartProcess(arguments, command, processInfo);

			} catch (Exception ex) {
				ExtendedConsole.WriteLineWarning($"Can't run the Redis command: '{command} {arguments}': {ex.Message}. Please run command manually.\n");
				return false;
			}
		}
	}
}