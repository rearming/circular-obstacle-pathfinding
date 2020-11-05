using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utils
{
	public static class UnityExtensions
	{
		public static bool IsRealNull(this Object unityObject)
		{
			// ReSharper disable once RedundantCast.0
			return (object) unityObject == null;
		}

		public static Vector2 ToVec2(this Vector3 vec)
		{
			return new Vector2(vec.x, vec.z);
		}

		public static Vector3 ToVec3(this Vector2 vec, float height)
		{
			return new Vector3(vec.x, height, vec.y);
		}

		public static Vector2 Slerp(this in Vector2 a, in Vector2 b, float t)
		{
			return Vector3.Slerp(a.ToVec3(0f), b.ToVec3(0f), t).ToVec2();
		}

		public static float InverseLerp(this in Vector3 value, in Vector3 start, in Vector3 end)
		{
			var ab = end - start;
			var av = value - start;
			return Vector3.Dot(av, ab) / Vector3.Dot(ab, ab);
		}

		public static float InverseLerp(this in Vector2 value, in Vector2 start, in Vector2 end)
		{
			return value.ToVec3(0).InverseLerp(start.ToVec3(0), end.ToVec3(0));
		}


		/// <summary>
		/// Rotates vector v counterclockwise by angle delta.
		/// </summary>
		/// <param name="v"></param>
		/// <param name="delta">In radians.</param>
		/// <returns></returns>
		public static Vector2 Rotate(this in Vector2 v, float delta)
		{
			return new Vector2(
				v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
				v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
			);
		}

		public static float ScaledRadius(this CapsuleCollider col)
		{
			var scale = Mathf.Max(col.gameObject.transform.localScale.x, col.gameObject.transform.localScale.z);
			return col.radius * scale;
		}

		/// <summary>
		///     In fact, tolerance == sqrt(provided tolerance)
		/// </summary>
		/// <param name="v1"></param>
		/// <param name="v2"></param>
		/// <param name="tolerance"></param>
		/// <returns></returns>
		public static bool AlmostEqual(this in Vector2 v1, in Vector2 v2, in float tolerance)
		{
			return Vector2.SqrMagnitude(v1 - v2) < tolerance;
		}

		public static bool AlmostEqual(this in float a, in float b, in float tolerance)
		{
			return Math.Abs(a - b) < tolerance;
		}
	}
}