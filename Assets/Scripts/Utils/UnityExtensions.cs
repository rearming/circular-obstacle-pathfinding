using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
	public static class UnityExtensions
	{
		public static Vector2 ToVec2(this Vector3 vec)
		{
			return new Vector2(vec.x, vec.z);
		}
		
		public static Vector3 ToVec3(this Vector2 vec, float height) => new Vector3(vec.x, height, vec.y);
		
	
		public static Vector2 Rotate(this Vector2 v, float delta)
		{
			return new Vector2(
				v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
				v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
			);
		}

		public static float Cross(this Vector2 a, Vector2 b)
		{
			return a.x * b.y - b.y * a.x;
		}

		public static float ScaledRadius(this CapsuleCollider col)
		{
			var scale = Mathf.Max(col.gameObject.transform.localScale.x, col.gameObject.transform.localScale.z);
			return col.radius * scale;
		}
	}
}