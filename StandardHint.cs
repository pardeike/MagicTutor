using UnityEngine;
using Verse;

namespace Brrainz
{
	public class StandardHint : Hint
	{
		public static readonly float borderWidth = 4;
		public static readonly float padding = 12;
		public static readonly float headerHeight = 50;

		public override Vector2 WindowSize(Rect areaOfInterest)
		{
			var outerSpacing = 2 * (borderWidth + borderWidth + padding);
			var width = ContentWidth + outerSpacing;
			var height = headerHeight + padding + ContentHeight + outerSpacing;

			var (primary, secondary) = GetScreenPosition();
			if (ShouldAvoidScreenPosition(primary)) primary = secondary;
			switch (primary)
			{
				case ScreenPosition.left:
				case ScreenPosition.right:
					width += StaticGraphics.arrowLeftRightWidth - 1 - borderWidth;
					break;
				case ScreenPosition.top:
				case ScreenPosition.bottom:
					height += StaticGraphics.arrowTopDownHeight - 1 - borderWidth;
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

		public virtual float ContentWidth => 400f;
		public virtual float ContentHeight => message.Height(ContentWidth, GameFont.Small);

		public override void DrawWindow(Rect canvas)
		{
			var outerRect = canvas;

			var (primary, secondary) = GetScreenPosition();
			if (ShouldAvoidScreenPosition(primary)) primary = secondary;
			switch (primary)
			{
				case ScreenPosition.left:
					outerRect.xMin += StaticGraphics.arrowLeftRightWidth - 1 - borderWidth;
					break;
				case ScreenPosition.right:
					outerRect.xMax -= StaticGraphics.arrowLeftRightWidth - 1 - borderWidth;
					break;
				case ScreenPosition.top:
					outerRect.yMin += StaticGraphics.arrowTopDownHeight - 1 - borderWidth;
					break;
				case ScreenPosition.bottom:
					outerRect.yMax -= StaticGraphics.arrowTopDownHeight - 1 - borderWidth;
					break;
			}

			var innerRect = outerRect.ContractedBy(borderWidth, borderWidth);

			GUI.DrawTexture(innerRect, StaticGraphics.backgroundTexture, ScaleMode.StretchToFill, true, 1f, Color.white, 0, 2 * borderWidth);
			GUI.DrawTexture(outerRect, StaticGraphics.shadowTexture, ScaleMode.StretchToFill, true, 1f, Color.white, borderWidth, 3 * borderWidth);
			GUI.DrawTexture(innerRect, StaticGraphics.borderTexture, ScaleMode.StretchToFill, true, 1f, Color.white, borderWidth, 2 * borderWidth);

			var r = canvas;
			switch (primary)
			{
				case ScreenPosition.left:
					r.x = borderWidth;
					r.y = canvas.center.y - Mathf.RoundToInt(StaticGraphics.arrowLeftRightHeight / 2);
					break;
				case ScreenPosition.right:
					r.x = canvas.xMax - StaticGraphics.arrowLeftRightWidth - borderWidth;
					r.y = canvas.center.y - Mathf.RoundToInt(StaticGraphics.arrowLeftRightHeight / 2);
					break;
				case ScreenPosition.top:
					r.x = canvas.center.x - Mathf.RoundToInt(StaticGraphics.arrowTopDownWidth / 2);
					r.y = borderWidth;
					break;
				case ScreenPosition.bottom:
					r.x = innerRect.x + innerRect.width / 2 - StaticGraphics.arrowTopDownWidth / 2;
					r.y = canvas.yMax - StaticGraphics.arrowTopDownHeight - borderWidth;
					break;
			}
			var arrow = StaticGraphics.arrows[primary];
			r.width = arrow.width;
			r.height = arrow.height;
			GUI.DrawTexture(r, arrow, ScaleMode.ScaleAndCrop, true);

			innerRect = innerRect.ContractedBy(borderWidth + padding, borderWidth + padding);

			DoWindowContents(innerRect);

			if (Widgets.ButtonInvisible(innerRect))
				Acknowledge();
		}

		public override void DoWindowContents(Rect innerRect)
		{
			var content = tutor.CurrentMod.Content;
			var headerRect = innerRect.TopPartPixels(headerHeight);

			if (Widgets.ButtonImage(headerRect.RightPartPixels(18).TopPartPixels(18), TexButton.CloseXSmall, true))
				Acknowledge();

			var thumbnail = content.ModMetaData.PreviewImage;
			if (thumbnail != null)
			{
				var previewRect = headerRect.LeftPartPixels(headerHeight * thumbnail.width / thumbnail.height);
				GUI.DrawTexture(previewRect, content.ModMetaData.PreviewImage, ScaleMode.ScaleAndCrop);

				var titleRect = previewRect;
				titleRect.x += titleRect.width;
				var aboutRect = Rect.MinMaxRect(previewRect.xMax + padding, previewRect.yMin - 4, headerRect.xMax - 18 - padding, previewRect.yMax);
				GUI.Label(aboutRect, content.Name + "\n" + content.ModMetaData.AuthorsString);
			}
			else
				GUI.Label(headerRect, content.Name + "\n" + content.ModMetaData.AuthorsString);

			var messageRect = innerRect;
			messageRect.yMin = headerRect.yMax + padding;
			messageRect.yMax = innerRect.yMax;
			message.Label(messageRect, default, GameFont.Small, TextAnchor.UpperLeft);
		}
	}
}
