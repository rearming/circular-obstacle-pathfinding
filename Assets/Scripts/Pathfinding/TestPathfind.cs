using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pathfinding
{
	public class TestPathfind : MonoBehaviour
	{
		[TextArea(10, 30)]
		[SerializeField] private string rawMap;
		
		[SerializeField] private char start = 'S';
		[SerializeField] private char end = 'E';
		[SerializeField] private char empty = '*';
		[SerializeField] private char obstacle = '#';

		private char[,] map;
		
		private Graph<Vector2Int> graph = new Graph<Vector2Int>();
		
		private void Start()
		{
			GetMap();
			GetNodes();
			ConnectNodes();
		}

		private void GetMap()
		{
			var splitMap = rawMap.Split('\n');
			var maxWidth = splitMap.Max(s => s.Length);
			var maxHeight = splitMap.Length;
			
			splitMap = splitMap.Select(s => s.PadRight(maxWidth, obstacle)).ToArray();
			rawMap = string.Join("\n", splitMap); // correct in inspector

			map = new char[maxHeight, maxWidth];
			for (int y = 0; y < maxHeight; y++)
			{
				for (int x = 0; x < maxWidth; x++)
					map[y, x] = splitMap[y][x];
			}
		}

		private void GetNodes()
		{
			for (int y = 0; y < map.GetLength(0); y++)
			{
				for (int x = 0; x < map.GetLength(1); x++)
				{
					if (map[y, x] != obstacle)
						graph.AddNode(new Vector2Int(x, y));
				}
			}
		}

		private void ConnectNodes()
		{
			foreach (var node in graph)
				ConnectGridNode(node.Content);
		}

		private void ConnectGridNode(Vector2Int nodeContent)
		{
			for (int y = nodeContent.y - 1; y <= nodeContent.y + 1; y++)
			{
				for (int x = nodeContent.x - 1; x <= nodeContent.x + 1; x++)
				{
					if (x == nodeContent.x && y == nodeContent.y)
						continue;
					graph.ConnectNodes(nodeContent, new Vector2Int(x, y));
				}
			}
		}
	}
}