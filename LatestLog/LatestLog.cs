using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using HarmonyLib;
using NeosModLoader;
using UnityEngine;

namespace LatestLog
{
	class LatestLogMod : NeosMod
	{
		[DllImport("libc", EntryPoint = "symlink")]
		private static extern int symlink(string source, string name);
		[DllImport("kernel32.dll")]
		static extern bool CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, uint dwFlags);

		public override string Name => BuildInfo.Name;
		public override string Author => BuildInfo.Author;
		public override string Version => BuildInfo.Version;
		public override string Link => BuildInfo.Link;
		public override void OnEngineInit()
		{
			FrooxEngineRunner engineRunner = Resources.FindObjectsOfTypeAll<FrooxEngineRunner>().First();
			StreamWriter logWriter = Traverse.Create(engineRunner).Field("logStream").GetValue<StreamWriter>();
			string fullPath = ((FileStream)logWriter.BaseStream).Name;
			string target = Path.Combine(
				Directory.GetParent(Directory.GetParent(fullPath).FullName).FullName,
				"Latest.log");
			Msg("Creating symlink from: `" + fullPath + "` to `" + target + "`");
			if (File.Exists(target)) File.Delete(target);
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				if (CreateSymbolicLink(fullPath, target, 0x2)) Debug("Windows symlink created!");
				else Error("Windows symlink creation failed :/");
			}
			else
			{
				int error = symlink(fullPath, target);
				if (error == 0) Debug("Linux symlink created!");
				else Error("Linux symlink creation failed :/ error: " + error);
			}
		}
	}
}
