namespace Creatio.Updater
{
	using System;
	using System.Diagnostics;
	using Creatio.Updater.Configuration;
	using global::Updater.Common;

	public static class RedisExecutor
	{
		public static ProcessUtility processUtility = new ProcessUtility();
		public static bool ClearRedisCache(ISiteInfo siteInfo)
		{
			if (UpdaterConfig.GetFeature("SkipClearRedisCache"))
			{
				return true;
			}
			if (Environment.ExitCode != 0)
			{
				ExtendedConsole.WriteLineWarning("\nRedis cache was not cleared. Please successfully update the site first.\n");
				return false;
			}
			string arguments = !string.IsNullOrEmpty(siteInfo.RedisPassword)
				? $"-h {siteInfo.RedisServer} -p {siteInfo.RedisPort} -n {siteInfo.RedisDB} --no-auth-warning -a {siteInfo.RedisPassword} flushdb"
				: $"-h {siteInfo.RedisServer} -p {siteInfo.RedisPort} -n {siteInfo.RedisDB} flushdb";
			return ExecuteCommand(arguments, processUtility);

		}


		private static bool ExecuteCommand(string arguments, ProcessUtility process)
		{
			const string command = "redis-cli";
			try
			{
				var processInfo = new ProcessStartInfo
				{
					FileName = command,
					Arguments = arguments,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					UseShellExecute = false,
					CreateNoWindow = true
				};
				return processUtility.StartProcess(arguments, command, processInfo);
			}
			catch (Exception ex)
			{
				ExtendedConsole.WriteLineWarning($"Can't run the Redis command: '{command} {arguments}': {ex.Message}. Please run command manually.\n");
				return false;
			}
		}
	}
}