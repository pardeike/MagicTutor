using UnityEngine;
using Verse;

namespace Brrainz
{
	public class StandardHint : Hint
	{
		const float contentWidth = 400;
		const float borderWidth = 4;
		const float padding = 12;
		const float headerHeight = 50;

		public override Vector2 WindowSize(Rect areaOfInterest)
		{
			var outerSpacing = 2 * (borderWidth + borderWidth + padding);
			var width = contentWidth + outerSpacing;
			var height = headerHeight + padding + message.Height(contentWidth, GameFont.Small) + outerSpacing;

			var (primary, secondary) = GetScreenPosition();
			if (ShouldAvoidScreenPosition(primary)) primary = secondary;
			switch (primary)
			{
				case ScreenPosition.left:
				case ScreenPosition.right:
					width += StaticGraphics.halfLeftPointerSize.y;
					break;
				case ScreenPosition.top:
				case ScreenPosition.bottom:
					height += StaticGraphics.halfLeftPointerSize.y;
					break;
			}
			return new Vector2(width, height);
		}

		public override Vector2 WindowOffset(Rect areaOfInterest)
		{
			var windowSize = WindowSize(areaOfInterest);
			var (primary, secondary) = GetScreenPosition();
			if (ShouldAvoidScreenPosition(primary)) primary = secondary;
			return primary switch
			{
				ScreenPosition.left => new Vector2(windowSize.x + areaOfInterest.width / 2, 0) / 2,
				ScreenPosition.right => new Vector2(-windowSize.x - areaOfInterest.width / 2, 0) / 2,
				ScreenPosition.top => new Vector2(0, windowSize.y + areaOfInterest.height / 2) / 2,
				ScreenPosition.bottom => new Vector2(0, -windowSize.y - areaOfInterest.height / 2) / 2,
				_ => default,
			};
		}

		public string message = "";

		public override void DoWindowContents(Rect canvas)
		{
			// GUI.DrawTexture(canvas, StaticGraphics.debugTexture, ScaleMode.StretchToFill, true, 1f, Color.white, 0, 0);

			var outerRect = canvas;

			var (primary, secondary) = GetScreenPosition();
			if (ShouldAvoidScreenPosition(primary)) primary = secondary;
			switch (primary)
			{
				case ScreenPosition.left:
					outerRect.xMin += StaticGraphics.halfLeftPointerSize.y;
					break;
				case ScreenPosition.right:
					outerRect.xMax -= StaticGraphics.halfLeftPointerSize.y;
					break;
				case ScreenPosition.top:
					outerRect.yMin += StaticGraphics.halfLeftPointerSize.y;
					break;
				case ScreenPosition.bottom:
					outerRect.yMax -= StaticGraphics.halfLeftPointerSize.y;
					break;
			}

			var innerRect = outerRect.ContractedBy(borderWidth, borderWidth);

			GUI.DrawTexture(innerRect, StaticGraphics.backgroundTexture, ScaleMode.StretchToFill, true, 1f, Color.white, 0, 2 * borderWidth);
			GUI.DrawTexture(outerRect, StaticGraphics.shadowTexture, ScaleMode.StretchToFill, true, 1f, Color.white, borderWidth, 3 * borderWidth);
			GUI.DrawTexture(innerRect, StaticGraphics.borderTexture, ScaleMode.StretchToFill, true, 1f, Color.white, borderWidth, 2 * borderWidth);

			innerRect = innerRect.ContractedBy(borderWidth + padding, borderWidth + padding);
			if (Widgets.ButtonInvisible(innerRect))
				Acknowledge();

			var headerRect = innerRect.TopPartPixels(headerHeight);

			if (Widgets.ButtonImage(headerRect.RightPartPixels(18).TopPartPixels(18), TexButton.CloseXSmall, true))
				Acknowledge();

			var thumbnail = tutor.mod.Content.ModMetaData.PreviewImage;
			if (thumbnail != null)
			{
				var previewRect = headerRect.LeftPartPixels(headerHeight * thumbnail.width / thumbnail.height);
				GUI.DrawTexture(previewRect, tutor.mod.Content.ModMetaData.PreviewImage, ScaleMode.ScaleAndCrop);

				var titleRect = previewRect;
				titleRect.x += titleRect.width;
				var aboutRect = Rect.MinMaxRect(previewRect.xMax + padding, previewRect.yMin - 4, headerRect.xMax - 18 - padding, previewRect.yMax);
				GUI.Label(aboutRect, tutor.mod.Content.Name + "\n" + tutor.mod.Content.ModMetaData.AuthorsString);
			}
			else
				GUI.Label(headerRect, tutor.mod.Content.Name + "\n" + tutor.mod.Content.ModMetaData.AuthorsString);

			//var dismissText = "click here to dismiss";
			//var tHeight = dismissText.Height(innerRect.width, GameFont.Tiny);
			//dismissText.Label(innerRect.BottomPartPixels(tHeight), Color.white.ToTransparent(0.8f), GameFont.Tiny, TextAnchor.LowerCenter);

			var messageRect = innerRect;
			messageRect.yMin = headerRect.yMax + padding;
			messageRect.yMax = innerRect.yMax;
			message.Label(messageRect, default, GameFont.Small, TextAnchor.UpperLeft);

			switch (primary)
			{
				case ScreenPosition.left:
					DrawArrow(new Vector2(2 * borderWidth + StaticGraphics.halfLeftPointerSize.y, outerRect.height / 2), 90);
					break;
				case ScreenPosition.right:
					DrawArrow(new Vector2(outerRect.width - 2 * borderWidth, outerRect.height / 2), 270);
					break;
				case ScreenPosition.top:
					DrawArrow(new Vector2(outerRect.width / 2, 2 * borderWidth + StaticGraphics.halfLeftPointerSize.y), 180);
					break;
				case ScreenPosition.bottom:
					DrawArrow(new Vector2(outerRect.width / 2, outerRect.height - 2 * borderWidth), 0);
					break;
			}
		}

		internal void DrawArrow(Vector2 pos, float rotation)
		{
			rotation.DrawRotated(pos, () =>
			{
				var r = new Rect(pos, StaticGraphics.halfLeftPointerSize);
				r.x -= StaticGraphics.halfLeftPointerSize.x;
				GUI.DrawTexture(r, StaticGraphics.halfLeftPointer);
				r.x += StaticGraphics.halfLeftPointerSize.x;
				GUI.DrawTexture(r, StaticGraphics.halfRightPointer);
			});
		}

		private (ScreenPosition, ScreenPosition) GetScreenPosition()
		{
			var screenRect = new Rect(0, 0, Screen.width, Screen.height);
			var delta = screenRect.center - areaOfInterest.center;
			var primary = delta.x < 0 ? ScreenPosition.right : ScreenPosition.left;
			var secondary = delta.y < 0 ? ScreenPosition.bottom : ScreenPosition.top;
			var shouldSwap = false;
			if (secondary == ScreenPosition.left && areaOfInterest.xMin < screenRect.xMin + 200) shouldSwap = true;
			if (secondary == ScreenPosition.right && areaOfInterest.xMax > screenRect.xMax - 200) shouldSwap = true;
			if (secondary == ScreenPosition.top && areaOfInterest.yMin < screenRect.yMin + 200) shouldSwap = true;
			if (secondary == ScreenPosition.bottom && areaOfInterest.yMax > screenRect.yMax - 200) shouldSwap = true;
			if (shouldSwap)
			{
				var swap = primary;
				primary = secondary;
				secondary = swap;
			}
			return (primary, secondary);
		}
	}
}
