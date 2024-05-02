using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using MonkeyLoader.Logging;
using MonkeyLoader.Patching;
using MonkeyLoader.Resonite;

namespace LatestLog
{
	class LatestLogMonkey : ResoniteMonkey<LatestLogMonkey>
	{
		[DllImport("libc", EntryPoint = "symlink")]
		private static extern int symlink(string source, string name);
		[DllImport("kernel32.dll")]
		static extern bool CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, uint dwFlags);

		protected override bool OnEngineInit()
		{
			StreamWriter logWriter = FrooxEngineBootstrap.LogStream;
			string fullPath = ((FileStream)logWriter.BaseStream).Name;
			string target = Path.Combine(
				Directory.GetParent(Directory.GetParent(fullPath).FullName).FullName,
				"Latest.log");
			Logger.Info(() => "Creating symlink from: `" + fullPath + "` to `" + target + "`");
			if (File.Exists(target)) File.Delete(target);
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				if (CreateSymbolicLink(fullPath, target, 0x2)) Logger.Debug(() => "Windows symlink created!");
				else
				{
					Logger.Error(() => "Windows symlink creation failed :/");
					return false;
				}
			}
			else
			{
				int error = symlink(fullPath, target);
				if (error == 0) Logger.Debug(() => "Linux symlink created!");
				else
				{
					Logger.Error(() => "Linux symlink creation failed :/ error: " + error);
					return false;
				}
			}

			return true;
		}

		protected override IEnumerable<IFeaturePatch> GetFeaturePatches()
		{
			return Array.Empty<IFeaturePatch>();
		}
	}
}
