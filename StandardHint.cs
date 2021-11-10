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

		public virtual void DrawArrow(Vector2 pos, float rotation)
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
	}
}
