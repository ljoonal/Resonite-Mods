using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using NeosModLoader;
using UnityEngine;

namespace PrivacyShield
{
	[HarmonyPatch]
	class PrivacyShieldMod : NeosMod
	{
		public override string Name => BuildInfo.Name;
		public override string Author => BuildInfo.Author;
		public override string Version => BuildInfo.Version;
		public override string Link => BuildInfo.Link;

		// A value to be added to the configured FPS spoof value
		//[AutoRegisterConfigKey]
		//private static readonly ModConfigurationKey<float> VarianceFPS = new("VarianceFPS", "How much the FPS can change by", () => 10f);
		//[AutoRegisterConfigKey]
		// A value to be added to the configured ping spoof value
		//private static readonly ModConfigurationKey<float> MinFPS = new("MinFPS", "The FPS spoof target before adding variance.", () => 10f);

		[AutoRegisterConfigKey]
		private static readonly ModConfigurationKey<string> TimeZoneSpoof = new("TimeZoneSpoof", "The timezone to spoof to.", () => "UTC");

		private static PrivacyShieldMod Instance;

		public override void OnEngineInit()
		{
			Instance = this;
			try
			{
				var config = Instance.GetConfiguration();
				//Harmony harmony = new(BuildInfo.GUID);
				//harmony.PatchAll();
				Traverse.Create(typeof(TimeZoneInfo)).Field("local").SetValue(TimeZoneInfo.FindSystemTimeZoneById(config.GetValue(TimeZoneSpoof)));
				Msg("Patched successfully");
			}
			catch (Exception ex)
			{
				Error(ex);
			}
		}

		/*
		[HarmonyPatch(typeof(FrooxEngine.StandaloneSystemInfo), "frames", MethodType.Getter)]
		[HarmonyPrefix]
		private static bool PatchFPS(ref float __result)
		{
			var config = Instance.GetConfiguration();
			var minFPS = config.GetValue(MinFPS);
			// Run original getter if spoofing is disabled
			if (minFPS < 0) return true;
			var rng = new System.Random();
			var rand = rng.Next(0, (int)config.GetValue(VarianceFPS));
			// Otherwise use our value and don't run original getter.
			__result = minFPS + (float)rand;
			return false;
		}*/
	}
}
