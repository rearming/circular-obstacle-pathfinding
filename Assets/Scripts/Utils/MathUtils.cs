using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Pathfinding.CircularObstacleGraph;
using UnityEngine;

namespace Utils
{
	public static class MathUtils
	{
		public static Vector2 GetQuadBezier(Vector2 start, Vector2 end, Vector2 middle, float t)
		{
			t = Mathf.Clamp01(t);
			return Mathf.Pow(1 - t, 2) * start + 2 * t * (1 - t) * middle + Mathf.Pow(t, 2) * end;
		}

		public static Vector2 GetCubeBezier(Vector2 start, Vector2 end, Vector2 mid1, Vector2 mid2, float t)
		{
			t = Mathf.Clamp01(t);
			return (1 - t) * GetQuadBezier(start, mid1, mid2, t) + t * GetQuadBezier(mid1, mid2, end, t);
		}

		public static Vector2 Middle(in Vector2 a, in Vector2 b)
		{
			return (a + b) / 2;
		}

		public static List<Vector2> SplitRecursive(in Vector2 a, in Vector2 b, int depth)
		{
			var result = new List<Vector2>();
			depth--;
			if (depth <= 0)
			{
				result.Add(Middle(a, b));
				return result;
			}

			result.AddRange(SplitRecursive(a, Middle(a, b), depth));
			result.AddRange(SplitRecursive(Middle(a, b), b, depth));
			return result;
		}

		public static List<Vector2> SplitArc(in Vector2 a, in Vector2 b, int splits)
		{
			var points = new List<Vector2>();
			var deltaAngle = Vector2.SignedAngle(a, b) * Mathf.Deg2Rad / (splits + 1);
			
			for (var i = 0; i < splits; i++)
				points.Add(a.Rotate(deltaAngle * (i + 1)));

			return points;
		}
	}
}