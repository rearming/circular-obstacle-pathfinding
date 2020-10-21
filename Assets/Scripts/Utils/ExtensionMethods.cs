using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
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

		public static void AddToDictList<T,TK>(this IDictionary<T, List<TK>> dictionary, T key, TK item)
		{
			if (dictionary.TryGetValue(key, out var list))
				list.Add(item);
			else
				dictionary.Add(key, new List<TK>{ item });
		}
	
		public static void AddRangeToDictList<T, TK>(this IDictionary<T, List<TK>> dictionary, T key, List<TK> itemList)
		{
			if (dictionary.TryGetValue(key, out var list))
				list.AddRange(itemList);
			else
				dictionary.Add(key, itemList);
		}

		public static void ForEachDictListElem<T, TK>(this IDictionary<T, List<TK>> dictionary, Action<T, TK> action)
		{
			foreach (var list in dictionary)
			{
				foreach (var item in list.Value)
				{
					action(list.Key, item);
				}
			}
		}
		
		public static void ForEachDictListElem<T, TK>(this IDictionary<T, List<TK>> dictionary, Action<TK> action)
		{
			foreach (var list in dictionary)
			{
				foreach (var item in list.Value)
				{
					action(item);
				}
			}
		}
		
		public static void ForEachDictList<T, TK>(this IDictionary<T, List<TK>> dictionary, Action<List<TK>> action)
		{
			foreach (var list in dictionary)
				action(list.Value);
		}
		
		public static void ForEachDictList<T, TK>(this IDictionary<T, List<TK>> dictionary, Action<T, List<TK>> action)
		{
			foreach (var list in dictionary)
				action(list.Key, list.Value);
		}

		public static void RemoveDictListIf<T, TK>(this IDictionary<T, List<TK>> dictionary, Func<TK, bool> removeIf)
		{
			var removals = new List<(T, int)>(); // key, index
			foreach (var list in dictionary)
			{
				for (var i = 0; i < list.Value.Count; i++)
				{
					var item = list.Value[i];
					if (removeIf(item))
						removals.Add((list.Key, i));
				}
			}

			foreach (var (key, bitangentIdx) in removals)
			{
				dictionary[key].RemoveAt(bitangentIdx);
			}
		}

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