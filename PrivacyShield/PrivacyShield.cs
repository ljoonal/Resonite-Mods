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

		[AutoRegisterConfigKey]
		private static readonly ModConfigurationKey<float> SpoofFPS = new("FpsSpoof", "The FPS to spoof to.", () => 30f);
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
			var targetFPS = config.GetValue(SpoofFPS);
			// Run original getter if spoofing is disabled
			if (targetFPS <= 10 || targetFPS >= 144) return;
			//double random = rng.NextDouble() * config.GetValue(VarianceFPS);
			__instance.LocalUser.FPS = targetFPS;
		}
	}
}
