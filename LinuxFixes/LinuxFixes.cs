using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using ResoniteModLoader;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PenguNotifs
{
	[HarmonyPatch]
	class LinuxFixesMod : ResoniteMod	{
		public override string Name => BuildInfo.Name;
		public override string Author => BuildInfo.Author;
		public override string Version => BuildInfo.Version;
		public override string Link => BuildInfo.Link;

		/*[DllImport("libX11.so.6")]
		private static unsafe extern int XWarpPointer(void* display, int src_w, int dest_w, int src_x, int src_y, uint src_width, uint src_height, int dest_x, int dest_y);
		[DllImport("libX11.so.6")]
		private static unsafe extern int* XOpenDisplay(void* display_name);
		[DllImport("libX11.so.6")]
		private static unsafe extern int* XFlush(void* display);
		[DllImport("libX11.so.6")]
		private static unsafe extern int XGetInputFocus(void* display, int* focus_return, int* revert_to_return);

		private static unsafe void* DisplayPointer;*/

		public override void OnEngineInit()
		{
			try
			{
				//DisplayPointer = XOpenDisplay(null);
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

		[HarmonyPatch(typeof(FrooxEngine.ContextMenu), nameof(FrooxEngine.ContextMenu.OpenMenu))]
		[HarmonyPostfix]
		private static void CenterMouseOnContextMenu()
		{
			Debug($"Warping mouse to the center");
			/*WarpCursorPositionOnXorg(new Vector2(Screen.width / 2, Screen.height / 2));*/
			Cursor.lockState = CursorLockMode.None;
			Mouse.current.WarpCursorPosition(
				new Vector2(Screen.width / 2, Screen.height / 2)
			);
		}

		/*
		private static unsafe void WarpCursorPositionOnXorg(Vector2 __0)
		{
			Debug($"Setting mouse pointer to ({(int)__0.x}, {(int)__0.y})");
			int focus_return, revert_to_return;
			XGetInputFocus(DisplayPointer, &focus_return, &revert_to_return);
			XWarpPointer(DisplayPointer, 0, focus_return, 0, 0, 0, 0, (int)__0.x, (int)__0.y);
			XFlush(DisplayPointer);
			Debug($"Set mouse pointer to ({(int)__0.x}, {(int)__0.y})");
		}*/

		[HarmonyPatch(typeof(MouseDriver), nameof(MouseDriver.UpdateMouse))]
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> FixReverseScrollwheel(IEnumerable<CodeInstruction> instructions)
		{
			var found = false;
			foreach (var instruction in instructions)
			{
				if (instruction.Is(OpCodes.Ldc_I4_S, (sbyte)100))
				{
					instruction.operand = (sbyte)instruction.operand * -1;
					found = true;
				}
				yield return instruction;
			}
			if (found is false)
				throw new Exception("Cannot find scrollwheel multiplier in MouseDriver");
		}
	}
}
