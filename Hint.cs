using System;
using UnityEngine;
using Verse;

namespace Brrainz
{
	public abstract class Hint
	{
		public bool acknowledged;
		public TimeSpan visibleTime = TimeSpan.Zero;

		internal Rect areaOfInterest;
		internal MagicTutor tutor;
		internal bool visible = false;

		public abstract Vector2 WindowSize(Rect areaOfInterest);
		public abstract Vector2 WindowOffset(Rect areaOfInterest);

		public abstract void DrawWindow(Rect canvas);
		public abstract void DoWindowContents(Rect innerRect);

		public virtual void Acknowledge()
		{
			acknowledged = true;
			tutor.Save();
			MagicTutor.ClearCurrentHint();
		}

		public virtual bool ShouldAvoidScreenPosition(ScreenPosition position)
		{
			return false;
		}

		public virtual (ScreenPosition primary, ScreenPosition secondary) GetScreenPosition()
		{
			var screenRect = new Rect(0, 0, UI.screenWidth, UI.screenHeight);
			var delta = screenRect.center - areaOfInterest.center;
			var primary = delta.x < 0 ? ScreenPosition.right : ScreenPosition.left;
			var secondary = delta.y < 0 ? ScreenPosition.bottom : ScreenPosition.top;
			var shouldSwap = false;
			var safeMargin = 200;
			if (secondary == ScreenPosition.left && areaOfInterest.xMin < screenRect.xMin + safeMargin) shouldSwap = true;
			if (secondary == ScreenPosition.right && areaOfInterest.xMax > screenRect.xMax - safeMargin) shouldSwap = true;
			if (secondary == ScreenPosition.top && areaOfInterest.yMin < screenRect.yMin + safeMargin) shouldSwap = true;
			if (secondary == ScreenPosition.bottom && areaOfInterest.yMax > screenRect.yMax - safeMargin) shouldSwap = true;
			return shouldSwap ? (secondary, primary) : (primary, secondary);
		}
	}
}
