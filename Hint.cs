using UnityEngine;

namespace Brrainz
{
	public abstract class Hint
	{
		public bool acknowledged;

		internal Rect areaOfInterest;
		internal MagicTutor tutor;

		public abstract Vector2 WindowSize(Rect areaOfInterest);
		public abstract Vector2 WindowOffset(Rect areaOfInterest);
		public abstract void DoWindowContents(Rect canvas);

		public virtual void Acknowledge()
		{
			acknowledged = true;
			tutor.Save();
		}

		public virtual bool ShouldAvoidScreenPosition(ScreenPosition position)
		{
			return false;
		}
	}
}
