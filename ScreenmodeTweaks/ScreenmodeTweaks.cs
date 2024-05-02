using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using MonkeyLoader.Configuration;
using MonkeyLoader.Patching;
using MonkeyLoader.Resonite;
using UnityEngine;
using UnityFrooxEngineRunner;

namespace ScreenmodeTweaks
{
	public class ScreenmodeTweaksConfig : ConfigSection
	{
		public override string Description => Assembly.GetExecutingAssembly().GetName().FullName;
		public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;
		protected override IncompatibleConfigHandling IncompatibilityHandling => IncompatibleConfigHandling.ForceLoad;

		public override string Id => this.GetType().Name;

		public readonly DefiningConfigKey<int> FrameLimitFocused = new("FrameLimitFocused", "What to set the FPS limit to when focused", () => -1);
		public readonly DefiningConfigKey<int> FrameLimitUnfocused = new("FrameLimitUnfocused", "What to set the FPS limit to when unfocused", () => 30);
		public readonly DefiningConfigKey<int> VSyncCount = new("vSyncCount", "Sets Unity's QualitySettings.vSyncCount to this", () => 1);


		public ScreenmodeTweaksConfig() { }
	}

	[HarmonyPatch]
	class ScreenmodeTweaksMonkey : ConfiguredResoniteMonkey<ScreenmodeTweaksMonkey, ScreenmodeTweaksConfig>
	{

		protected override bool OnEngineInit()
		{
			try
			{
				Config.ItemChanged += delegate { UpdateFrameRateTarget(Application.isFocused); };
				Application.focusChanged += UpdateFrameRateTarget;
				UpdateFrameRateTarget(Application.isFocused);

				Harmony harmony = new(Assembly.GetExecutingAssembly().GetName().FullName);
				harmony.PatchAll();
				Logger.Info(() => "Patched successfully");
			}
			catch (Exception ex)
			{
				Logger.Error(() => ex.ToString());
				return false;
			}
			return true;
		}

		private void UpdateFrameRateTarget(bool hasFocus)
		{
			Logger.Debug(() => $"UpdateFrameRate {hasFocus}");
			int frameRateTarget;
			if (hasFocus) frameRateTarget = ConfigSection.FrameLimitFocused.GetValue();
			else frameRateTarget = ConfigSection.FrameLimitUnfocused.GetValue();

			Application.targetFrameRate = frameRateTarget;
		}


		[HarmonyPatch(typeof(FrooxEngineRunner), "Update")]
		[HarmonyPostfix]
		public static void OverwriteVSyncUpdate()
		{
			QualitySettings.vSyncCount = ConfigSection.VSyncCount.GetValue();
		}

		protected override IEnumerable<IFeaturePatch> GetFeaturePatches()
		{
			return Array.Empty<IFeaturePatch>();
		}
	}
}
