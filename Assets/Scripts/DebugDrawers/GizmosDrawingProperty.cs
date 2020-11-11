using System;
using UnityEngine;

namespace DebugDrawers
{
	[Serializable]
	public class GizmosDrawingProperty
	{
		public bool draw = true;
		public Color color = Color.green;

		public static implicit operator bool(GizmosDrawingProperty property) => property.draw;
	}
}