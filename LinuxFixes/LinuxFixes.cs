using System;
using System.Reflection;
using HarmonyLib;
using NeosModLoader;
using UnityEngine;

namespace LinuxFixes
{
	[HarmonyPatch]
	class LinuxFixesMod : NeosMod
	{
		public override string Name => BuildInfo.Name;
		public override string Author => BuildInfo.Author;
		public override string Version => BuildInfo.Version;
		public override string Link => BuildInfo.Link;

		public override void OnEngineInit()
		{
			try
			{
				Harmony harmony = new(BuildInfo.GUID);
				harmony.PatchAll();
				Msg("Patched successfully");
			}
			catch (Exception ex)
			{
				Error(ex);
			}
		}

		[HarmonyPatch(typeof(MouseDriver), nameof(MouseDriver.UpdateMouse))]
		[HarmonyPostfix]
		private static void OverwriteLockState()
		{
			Cursor.lockState = CursorLockMode.Locked;
		}
	}
}
