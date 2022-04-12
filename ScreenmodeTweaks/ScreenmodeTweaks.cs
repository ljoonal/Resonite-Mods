using System;
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

		[AutoRegisterConfigKey]
		private static readonly ModConfigurationKey<int> FrameLimitFocused = new("FrameLimitFocused", "What to set the FPS limit to when focused", () => -1);
		[AutoRegisterConfigKey]
		private static readonly ModConfigurationKey<int> FrameLimitUnfocused = new("FrameLimitUnfocused", "What to set the FPS limit to when unfocused", () => 30);
		[AutoRegisterConfigKey]
		private static readonly ModConfigurationKey<int> VSyncCount = new("vSyncCount", "Sets Unity's QualitySettings.vSyncCount to this", () => 1);

		private static ModConfiguration Config;

		public override void OnEngineInit()
		{
			try
			{
				Config = GetConfiguration();
				Config.Save(true);
				Config.OnThisConfigurationChanged += delegate { UpdateFrameRateTarget(Application.isFocused); };
				Application.focusChanged += UpdateFrameRateTarget;
				UpdateFrameRateTarget(Application.isFocused);

				Harmony harmony = new(BuildInfo.GUID);
				harmony.PatchAll();
				Msg("Patched successfully");
			}
			catch (Exception ex)
			{
				Error(ex);
			}
		}

		private void UpdateFrameRateTarget(bool hasFocus)
		{
			Debug($"UpdateFrameRate {hasFocus}");
			int frameRateTarget;
			if (hasFocus) frameRateTarget = Config.GetValue(FrameLimitFocused);
			else frameRateTarget = Config.GetValue(FrameLimitUnfocused);

			Application.targetFrameRate = frameRateTarget;
		}


		[HarmonyPatch(typeof(FrooxEngineRunner), "Update")]
		[HarmonyPostfix]
		public static void OverwriteVSyncUpdate()
		{
			QualitySettings.vSyncCount = Config.GetValue(VSyncCount);
		}
	}
}
