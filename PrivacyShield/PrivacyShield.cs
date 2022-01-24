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
		//private static readonly ModConfigurationKey<float> VarianceFPS = new("VarianceFPS", "How much the FPS can change by", () => 1.5f);
		// A value to be added to the configured ping spoof value
		[AutoRegisterConfigKey]
		private static readonly ModConfigurationKey<float> MinFPS = new("MinFPS", "The FPS spoof target before adding variance.", () => 30f);
		[AutoRegisterConfigKey]
		private static readonly ModConfigurationKey<string> TimeZoneSpoof = new("TimeZoneSpoof", "The timezone to spoof to.", () => "UTC");

		private static PrivacyShieldMod Instance;
		//private static readonly System.Random rng = new();

		public override void OnEngineInit()
		{
			Instance = this;
			try
			{
				var config = Instance.GetConfiguration();
				Traverse.Create(typeof(TimeZoneInfo)).Field("local").SetValue(TimeZoneInfo.FindSystemTimeZoneById(config.GetValue(TimeZoneSpoof)));
			}
			catch (Exception ex)
			{
				Error(ex);
			}
			try
			{
				Harmony harmony = new(BuildInfo.GUID);
				harmony.PatchAll();
			}
			catch (Exception ex)
			{
				Error(ex);
			}
		}


		[HarmonyPatch(typeof(FrooxEngine.World), "RefreshStep")]
		[HarmonyPostfix]
		private static void PatchFPS(ref FrooxEngine.World __instance)
		{
			var config = Instance.GetConfiguration();
			var minFPS = config.GetValue(MinFPS);
			// Run original getter if spoofing is disabled
			if (minFPS < 0) return;
			//double random = rng.NextDouble() * config.GetValue(VarianceFPS);
			__instance.LocalUser.FPS = minFPS;
		}
	}
}
