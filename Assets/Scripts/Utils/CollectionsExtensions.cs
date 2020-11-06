using System;
using System.Collections.Generic;

namespace Utils
{
	public static class CollectionsExtensions
	{
		#region Array

		public static void ForEach<T>(this T[] array, Action<T> action)
		{
			Array.ForEach(array, action);
		}

		#endregion

		#region Hash Set

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

		#endregion

		#region Dictionary

		public static void AddToDictList<T, TK>(this IDictionary<T, List<TK>> dictionary, T key, TK item)
		{
			if (dictionary.TryGetValue(key, out var list))
				list.Add(item);
			else
				dictionary.Add(key, new List<TK> {item});
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
					action(list.Key, item);
			}
		}

		public static void ForEachDictListElem<T, TK>(this IDictionary<T, List<TK>> dictionary, Action<TK> action)
		{
			foreach (var list in dictionary)
			{
				foreach (var item in list.Value)
					action(item);
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

			foreach (var (key, idx) in removals)
				dictionary[key].RemoveAt(idx);
		}

		#endregion
	}
}