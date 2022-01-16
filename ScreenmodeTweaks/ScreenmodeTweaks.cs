using System;
using System.Reflection;
using HarmonyLib;
using NeosModLoader;
using UnityEngine;

namespace ScreenmodeTweaks
{
	[HarmonyPatch]
	class ScreenmodeTweaksMod : NeosMod
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

		[HarmonyPatch(typeof(FrooxEngineRunner), "Update")]
		[HarmonyPostfix]
		public static void OverwriteVSyncUpdate()
		{
			//if (DisableVSync.Value) 
			QualitySettings.vSyncCount = 0;
		}
	}
}
