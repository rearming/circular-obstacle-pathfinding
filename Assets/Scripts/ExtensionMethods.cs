using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using Priority_Queue;
using UnityEngine;

public static class ExtensionMethods
{
	public static bool AddWithAction<T>(this HashSet<T> hashSet, T item, Action<T> onAdd)
	{
		onAdd(item);
		return hashSet.Add(item);
	}

	public static void RemoveWithAction<T>(this HashSet<T> list, T item, Action<T> onRemove)
	{
		onRemove(item);
		list.Remove(item);
	}

	public static void ForEach<T>(this HashSet<T> hashSet, Action<T> action)
	{
		foreach (var elem in hashSet)
			action(elem);
	}

	public static void AddRange<T>(this HashSet<T> hashSet, HashSet<T> hashSetToAdd)
	{
		hashSet.UnionWith(hashSetToAdd);
	}

	public static Vector2 ToVec2(this Vector3 vec)
	{
		return new Vector2(vec.x, vec.z);
	}
	
	public static Vector2 Rotate(this Vector2 v, float delta)
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
}