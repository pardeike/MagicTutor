using System;
using UnityEngine;
using Verse;

namespace Brrainz
{
	public static class Tools
	{
		public static void DrawRotated(this float degree, Vector2 center, Action drawer)
		{
			var oldMatrix = GUI.matrix;
			GUIUtility.RotateAroundPivot(degree, center);
			drawer();
			GUI.matrix = oldMatrix;
		}

		public static Vector2 SizeWith(this string text, float maxWidth, GameFont font)
		{
			var oldFond = Text.Font;
			Text.Font = font;
			var rect = GUILayoutUtility.GetRect(new GUIContent(text), GUIStyle.none, GUILayout.MaxWidth(maxWidth));
			Text.Font = oldFond;
			return rect.size;
		}

		public static Rect MovedBy(this Rect rect, Vector2 offset)
		{
			rect.x += offset.x;
			rect.y += offset.y;
			return rect;
		}

		public static float Width(this string text, GameFont font)
		{
			var oldFond = Text.Font;
			Text.Font = font;
			var result = Text.CalcSize(text).x;
			Text.Font = oldFond;
			return result;
		}

		public static float Height(this string text, float width, GameFont font)
		{
			var oldFond = Text.Font;
			Text.Font = font;
			var result = Text.CalcHeight(text, width);
			Text.Font = oldFond;
			return result;
		}

		public static void Label(this string text, Rect rect, Color color = default, GameFont font = GameFont.Tiny, TextAnchor anchor = TextAnchor.MiddleCenter)
		{
			var oldFond = Text.Font;
			var oldAnchor = Text.Anchor;
			var oldColor = GUI.color;
			Text.Anchor = anchor;
			Text.Font = font;
			GUI.color = color == default ? Color.white : color;
			Widgets.Label(rect, text);
			Text.Font = oldFond;
			Text.Anchor = oldAnchor;
			GUI.color = oldColor;
		}
	}
}
