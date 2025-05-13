namespace Updater.Redis
{
	using System;
	using System.Diagnostics;
	using Creatio.Updater;
	using Creatio.Updater.Configuration;
	using Updater.Common;

	#region Class: RedisExecutor

	public static class RedisExecutor
	{

		#region Fields: Private

		private static IProcessUtility _processUtility;

		#endregion

		#region Properties: Public

		public static IProcessUtility ProcessUtility
		{
			get => _processUtility ??= new ProcessUtility();
			set => _processUtility = value;
		}

		#endregion

		#region Methods: Public

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
			return ExecuteCommand(arguments, ProcessUtility);
		}

		#endregion

		#region Methods: Private

		private static bool ExecuteCommand(string arguments, IProcessUtility processUtility)
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
				bool result = processUtility.StartProcess(processInfo);
				return result;
			}
			catch (Exception ex)
			{
				ExtendedConsole.WriteLineWarning(
					$"Can't run the Redis command: '{command} {arguments}': {ex.Message}. Please run command manually.\n");
				return false;
			}
		}

		#endregion

	}

	#endregion

}