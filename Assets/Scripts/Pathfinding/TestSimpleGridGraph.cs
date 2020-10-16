using UnityEngine;

namespace Pathfinding
{
	public class TestSimpleGridGraph : MonoBehaviour
	{
		[SerializeField] private int width;
		[SerializeField] private int height;

		private Graph<Vector2Int> graph = new Graph<Vector2Int>();
		
		private void Start()
		{
			GenerateGridGraph();
			PrintGraph();
		}

		private void PrintGraph()
		{
			foreach (var node in graph)
			{
				Debug.Log($"Node: [{node.Content.ToString()}]");
				foreach (var neighbor in graph.Neighbors(node))
				{
					Debug.Log($"Neighbor: [{neighbor.Content.ToString()}]]");
				}
				Debug.Log("-------------------------------------------");
			}
		}

		private void GenerateGridGraph()
		{
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					var newNode = new Node<Vector2Int>(new Vector2Int(x, y));
					graph.AddNode(newNode);
					ConnectGridNode(newNode.Content);
				}
			}
		}

		private void ConnectGridNode(Vector2Int nodeContent)
		{
			for (int y = nodeContent.y - 1; y <= nodeContent.y + 1; y++)
			{
				for (int x = nodeContent.x - 1; x <= nodeContent.x + 1; x++)
				{
					if (x < 0 || y < 0 || x >= width || y >= height)
						continue;
					if (x == nodeContent.x && y == nodeContent.y)
						continue;
					graph.ConnectNodes(nodeContent, new Vector2Int(x, y));
				}
			}
		}
	}
}