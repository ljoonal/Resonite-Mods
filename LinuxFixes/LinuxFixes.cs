using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using HarmonyLib;
using MonkeyLoader.Configuration;
using MonkeyLoader.Patching;
using MonkeyLoader.Resonite;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LinuxFixes
{
	public class LinuxFixesConfig : ConfigSection
	{
		public override string Description => Assembly.GetExecutingAssembly().GetName().FullName;
		public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;
		protected override IncompatibleConfigHandling IncompatibilityHandling => IncompatibleConfigHandling.ForceLoad;

		public override string Id => this.GetType().Name;

		public readonly DefiningConfigKey<bool> ReverseScroll = new("ReverseScroll", "If to reverse the scroll direction (requires game restart)", () => false);
		public readonly DefiningConfigKey<CursorLockMode?> SetCursorLockMode = new("SetCursorLockMode", "Which cursor lock mode to enforce", () => null);
		public readonly DefiningConfigKey<bool> WarpMouseToCenter = new("WarpMouseToCenter", "If to warp mouse to center on context menu open", () => true);
		public LinuxFixesConfig() { }
	}

	[HarmonyPatch]
	[HarmonyPatchCategory(nameof(LinuxFixesMonkey))]
	class LinuxFixesMonkey : ConfiguredResoniteMonkey<LinuxFixesMonkey, LinuxFixesConfig>
	{
		public LinuxFixesMonkey() { }
		public override string Name => Assembly.GetExecutingAssembly().GetName().Name;

		[HarmonyPatch(typeof(MouseDriver), nameof(MouseDriver.UpdateMouse))]
		[HarmonyPostfix]
		private static void OverwriteLockState()
		{
			var lockMode = ConfigSection.SetCursorLockMode.GetValue();
			if (lockMode.HasValue)
			{
				Cursor.lockState = lockMode.Value;
			}
		}

		[HarmonyPatch(typeof(FrooxEngine.ContextMenu), nameof(FrooxEngine.ContextMenu.OpenMenu))]
		[HarmonyPostfix]
		private static void CenterMouseOnContextMenu()
		{
			if (!ConfigSection.WarpMouseToCenter.GetValue()) return;
			Logger.Debug(() => $"Warping mouse to the center");
			Cursor.lockState = CursorLockMode.None;
			Mouse.current.WarpCursorPosition(
				new Vector2(Screen.width / 2, Screen.height / 2)
			);
		}

		[HarmonyPatch(typeof(MouseDriver), nameof(MouseDriver.UpdateMouse))]
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> FixReverseScrollwheel(IEnumerable<CodeInstruction> instructions)
		{
			var found = false;
			foreach (var instruction in instructions)
			{
				if (instruction.Is(OpCodes.Ldc_I4_S, (sbyte)100))
				{
					instruction.operand = (sbyte)instruction.operand * (ConfigSection.ReverseScroll.GetValue() ? -1 : 1);
					found = true;
				}
				yield return instruction;
			}
			if (found is false)
				throw new Exception("Cannot find scrollwheel multiplier in MouseDriver");
		}


		[HarmonyPatch(typeof(RuntimeInformation), nameof(RuntimeInformation.OSDescription))]
		[HarmonyPrefix]
		private static bool FixNtDllImportingOSDescription(ref string __result)
		{
			__result = "Linux";
			return false;
		}

		[HarmonyPatch(typeof(RuntimeInformation), nameof(RuntimeInformation.OSArchitecture))]
		[HarmonyPrefix]
		private static bool FixNtDllImportingOSArchitecture(ref Architecture __result)
		{
			__result = Architecture.X64;
			return false;
		}

		protected override IEnumerable<IFeaturePatch> GetFeaturePatches()
		{
			// yield return new FeaturePatch(nameof(MouseDriver.UpdateMouse), PatchCompatibility.HookOnly);
			return Array.Empty<IFeaturePatch>();
		}
	}
}
