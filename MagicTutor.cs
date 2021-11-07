﻿using HarmonyLib;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using UnityEngine;
using Verse;

namespace Brrainz
{
	public class MagicTutor
	{
		const string tutorID = "brrainz.magictutor";
		const int tutorWindowID = 1294583817;

		internal Mod mod;
		private readonly string fileName;
		private readonly string baseFolderPath = Path.Combine(GenFilePaths.ConfigFolderPath, "MagicTutor");
		private readonly ConcurrentDictionary<string, Hint> hints = new ConcurrentDictionary<string, Hint>();
		private static readonly HintDelegate hintDelegate;

		static MagicTutor()
		{
			hintDelegate = SharedState.GetDelegate();
			AddTutorToOnGUI();
			PatchGetWindowAt();
		}

		public MagicTutor(Mod mod)
		{
			this.mod = mod;
			var name = mod.Content.FolderName.Replace('.', '-').Replace(' ', '-');
			fileName = $"{name}.txt";
		}

		private static void AddTutorToOnGUI()
		{
			var original = AccessTools.Method(typeof(Root), nameof(Root.OnGUI));
			var patch = SymbolExtensions.GetMethodInfo(() => HintsOnGUI());
			if (Harmony.GetPatchInfo(original)?.Postfixes?.FirstOrDefault(postfix => postfix?.owner == tutorID) == null)
			{
				var harmony = new Harmony(tutorID);
				_ = harmony.Patch(original, postfix: new HarmonyMethod(patch));
			}
		}

		private static void PatchGetWindowAt()
		{
			var original = AccessTools.Method(typeof(WindowStack), nameof(WindowStack.GetWindowAt));
			Window dummy = default;
			var patch = SymbolExtensions.GetMethodInfo(() => GetWindowAt_HidingUs(ref dummy));
			if (Harmony.GetPatchInfo(original)?.Postfixes?.FirstOrDefault(postfix => postfix?.owner == tutorID) == null)
			{
				var harmony = new Harmony(tutorID);
				_ = harmony.Patch(original, postfix: new HarmonyMethod(patch));
			}
		}

		public void Start()
		{
			var path = Path.Combine(baseFolderPath, fileName);
			try
			{
				if (Directory.Exists(baseFolderPath) == false)
					_ = Directory.CreateDirectory(baseFolderPath);
				if (File.Exists(path) == false) return;
				File.ReadAllLines(path).Do(line =>
				{
					if (hints.TryGetValue(line, out var hint))
						hint.acknowledged = true;
				});
			}
			catch (Exception ex)
			{
				Log.Error($"Exception while reading {path}: {ex}");
			}
		}

		internal void Save()
		{
			if (Directory.Exists(baseFolderPath) == false)
				_ = Directory.CreateDirectory(baseFolderPath);
			var path = Path.Combine(baseFolderPath, fileName);
			try
			{
				var lines = hints.Where(hint => hint.Value.acknowledged).Select(hint => hint.Key).ToArray();
				File.WriteAllLines(path, lines);
			}
			catch (Exception ex)
			{
				Log.Error($"Exception while writing {path}: {ex}");
			}
		}

		private static void HintsOnGUI()
		{
			if (LongEventHandler.ShouldWaitForEvent) return;
			if (Event.current.type != EventType.Repaint) return;
			ref object obj = ref hintDelegate();
			var renderedHint = obj as Hint;
			obj = null;

			if (renderedHint == null) return;

			var windowSize = renderedHint.WindowSize(renderedHint.areaOfInterest);
			var rect = new Rect(renderedHint.areaOfInterest.center - windowSize / 2 + renderedHint.WindowOffset(renderedHint.areaOfInterest), windowSize);

			Find.WindowStack.ImmediateWindow(tutorWindowID, rect, WindowLayer.Super, () =>
			{
				rect = rect.AtZero();
				renderedHint.DoWindowContents(rect);
			}, false, false, 0f);
		}

		private static void GetWindowAt_HidingUs(ref Window __result)
		{
			if (__result != null && __result.ID == -Math.Abs(tutorWindowID))
				__result = Find.WindowStack.currentlyDrawnWindow;
		}

		public void RegisterContext(string context, Hint hint, bool forceUpdate = false)
		{
			if (hints.ContainsKey(context) && forceUpdate == false) return;
			hint.acknowledged = false;
			hints[context] = hint;
		}

		public T DoHintableAction<T>(string context, Rect areaOfInterest, Func<Action, T> action)
		{
			if (hints.TryGetValue(context, out var hint) == false || hint.acknowledged) return action(() => { });
			var result = action(hint.Acknowledge);
			if (hintDelegate() == null)
			{
				var windowOffset = Find.WindowStack.currentlyDrawnWindow?.windowRect.position ?? Vector2.zero;
				hint.areaOfInterest = areaOfInterest.MovedBy(windowOffset);
				hint.tutor = this;
				object obj = hint;
				hintDelegate() = obj;
			}
			return result;
		}
	}
}