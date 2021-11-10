using System;
using UnityEngine;

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
