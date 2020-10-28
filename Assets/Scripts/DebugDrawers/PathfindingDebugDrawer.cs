using System;
using System.Collections.Generic;
using Pathfinding.CircularObstacleGraph;
using Pathfinding.Graph;
using UnityEngine;
using Utils;

namespace DebugDrawers
{
	public class PathfindingDebugDrawer : MonoBehaviour
	{
		[Header("Debug Draw")]

		[SerializeField] private float gizmosHeight = 1f;

		[Serializable]
		private class GizmosDrawingProperty
		{
			public bool draw = true;
			public Color color = Color.green;
		}
		
		[SerializeField] private GizmosDrawingProperty gizmosSurfingEdges;
		[SerializeField] private GizmosDrawingProperty gizmosHuggingEdges;
		[SerializeField] private GizmosDrawingProperty gizmosGraph;
		[SerializeField] private GizmosDrawingProperty gizmosPath;
		
		[SerializeField] private bool drawSortedCirclePoints = true;

		public CircularObsticleGraphGenerator graphGenerator;
		public List<NodeWithEdge<Vector2>> path;

		public void Setup(CircularObsticleGraphGenerator graphGenerator, List<NodeWithEdge<Vector2>> path)
		{
			this.graphGenerator = graphGenerator;
			this.path = path;
		}
		
		private void OnDrawGizmos()
		{
			if (!Application.isPlaying) return;
			
			if (gizmosSurfingEdges.draw) DrawSurfingEdges();
			if (gizmosHuggingEdges.draw) DrawHuggingEdges();
			if (drawSortedCirclePoints) DrawSortedCirclePoints();
			if (gizmosGraph.draw) DrawGraph();
			if (gizmosPath.draw) DrawPath();
		}

		private void DrawSurfingEdges()
		{
			graphGenerator.SurfingEdges.ForEachDictListElem((_, edge) =>
			{
				Gizmos.color = gizmosSurfingEdges.color;
				Gizmos.DrawLine(edge.a.ToVec3(gizmosHeight), edge.b.ToVec3(gizmosHeight));
				Gizmos.color = Color.yellow;
				Gizmos.DrawSphere(edge.a.ToVec3(gizmosHeight), 0.04f);
				Gizmos.DrawSphere(edge.b.ToVec3(gizmosHeight), 0.04f);
			});
		}

		private void DrawHuggingEdges()
		{
			graphGenerator.HuggingEdges.ForEachDictListElem(edge =>
			{
				Gizmos.color = gizmosHuggingEdges.color;
				Gizmos.DrawLine(edge.a.ToVec3(gizmosHeight), edge.b.ToVec3(gizmosHeight));
			});
		}

		private void DrawSortedCirclePoints()
		{
			graphGenerator.PointsOnCircle.ForEachDictList((circleHash, pointList) =>
			{
				for (var i = 0; i < pointList.Count; i++)
				{
					var t = pointList.Count > 1 ? (float) i / (pointList.Count - 1) : 0;
					var center = graphGenerator.Circles[circleHash].center.ToVec3(gizmosHeight);
					Gizmos.color = Color.Lerp(Color.red, Color.blue, t);
					Gizmos.DrawLine(center, Vector3.Lerp(center, pointList[i].ToVec3(gizmosHeight), Mathf.Lerp(0.5f, 1f, t)));
				}
			});
		}

		private void DrawGraph()
		{
			Gizmos.color = gizmosGraph.color;
			foreach (var node in graphGenerator.graph)
			{
				foreach (var connectedNode in node.links)
				{
					Gizmos.DrawLine(node.Content.ToVec3(gizmosHeight), connectedNode.node.Content.ToVec3(gizmosHeight));
				}
			}
		}

		private void DrawPath()
		{
			if (path == null)
				return;
			
			Gizmos.color = gizmosPath.color;
			
			for (int i = 1; i < path.Count; i++)
			{
				Gizmos.DrawLine(path[i - 1].node.Content.ToVec3(gizmosHeight), path[i].node.Content.ToVec3(gizmosHeight));
			}
		}
	}
}