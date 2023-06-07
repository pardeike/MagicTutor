using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Brrainz
{
	[StaticConstructorOnStartup]
	internal class StaticGraphics
	{
		internal static readonly Texture shadowTexture = SolidColorMaterials.NewSolidColorTexture(Color.black.ToTransparent(0.25f));
		internal static readonly Texture backgroundTexture = SolidColorMaterials.NewSolidColorTexture(new Color(0x15 / (float)0xff, 0x19 / (float)0xff, 0x1d / (float)0xff));
		internal static readonly Texture borderTexture = SolidColorMaterials.NewSolidColorTexture(Color.white);
		internal static readonly Texture cancelTexture = SolidColorMaterials.NewSolidColorTexture(Color.white.ToTransparent(0.25f));
		//internal static readonly Texture debugTexture = SolidColorMaterials.NewSolidColorTexture(Color.red.ToTransparent(0.5f));

		internal static readonly Dictionary<ScreenPosition, Texture2D> arrows = new Dictionary<ScreenPosition, Texture2D>()
		{
			{ ScreenPosition.left, LoadResource("Brrainz.images.arrow-left.png") },
			{ ScreenPosition.right, LoadResource("Brrainz.images.arrow-right.png") },
			{ ScreenPosition.top, LoadResource("Brrainz.images.arrow-up.png") },
			{ ScreenPosition.bottom, LoadResource("Brrainz.images.arrow-down.png") },
		};

		internal static readonly float arrowLeftRightWidth = arrows[ScreenPosition.left].width;
		internal static readonly float arrowLeftRightHeight = arrows[ScreenPosition.left].height;

		internal static readonly float arrowTopDownWidth = arrows[ScreenPosition.top].width;
		internal static readonly float arrowTopDownHeight = arrows[ScreenPosition.top].height;

		static Texture2D LoadResource(string name)
		{
			var stream = typeof(MagicTutor).Assembly.GetManifestResourceStream(name);
			var bytes = new byte[stream.Length];
			stream.Read(bytes, 0, bytes.Length);
			var texture = new Texture2D(1, 1);
			texture.filterMode = FilterMode.Point;
			texture.LoadImage(bytes, true);
			return texture;
		}
	}
}
