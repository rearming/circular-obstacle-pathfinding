using System;
using System.Collections.Generic;
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
}