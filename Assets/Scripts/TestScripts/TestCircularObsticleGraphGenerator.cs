using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Utils;
using Random = System.Random;

namespace Pathfinding
{
	public class TestCircularObsticleGraphGenerator : MonoBehaviour
	{
		[SerializeField] private Transform circlesTParent;
		private (Transform, CapsuleCollider) [] circlesObstacles;

		[SerializeField] private Transform start;
		[SerializeField] private Transform goal;

		private Circle[] circles;
		private CircularObsticleGraphGenerator circularGenerator;

		#region Debug Drawing Properties

		[Header("Debug Draw")]
		
		[SerializeField] private float gizmosHeight = 1f;
		
		[SerializeField] private bool hideObstacles;
		[SerializeField] private bool hideObstacleMeshRenderers;
		
		[Serializable]
		private class GizmosDrawingProperty
		{
			public bool draw = true;
			public Color color = Color.green;
		}
		
		[SerializeField] private GizmosDrawingProperty gizmosSurfingEdges;
		[SerializeField] private GizmosDrawingProperty gizmosHuggingEdges;
		
		[SerializeField] private bool drawSortedCirclePoints = true;
		
		[SerializeField] private GizmosDrawingProperty gizmosGraph;

		private (Transform, Renderer)[] circlesDebug;

		#endregion

		private void Start()
		{
			GetObsticles();
			GetCircles();
			
			circularGenerator = new CircularObsticleGraphGenerator(circles, start.position.ToVec2(), goal.position.ToVec2());
			
			circularGenerator.GenerateGraph();
		}

		private void Update()
		{
			GetCircles();
			circularGenerator.SetCircles(circles);
			circularGenerator.GenerateGraph();
			
			ToggleObsticlesVisibility();
		}

		private void GetObsticles()
		{
			circlesObstacles = circlesTParent.GetComponentsInChildren<CapsuleCollider>()
				.Where(cc => cc.gameObject.activeSelf)
				.Select(cc => (cc.gameObject.transform, cc))
				.ToArray();
			
			circlesDebug = circlesTParent.GetComponentsInChildren<CapsuleCollider>()
				.Where(cc => cc.gameObject.activeSelf)
				.Select(cc => (cc.gameObject.transform, cc.gameObject.GetComponent<Renderer>()))
				.ToArray();
		}
		
		private void GetCircles()
		{
			circles = circlesObstacles.Select(t => new Circle(t.Item2.ScaledRadius(), t.Item1.position.ToVec2())).ToArray();
		}
		
		#region Debug Drawing

		private void OnDrawGizmos()
		{
			if (!Application.isPlaying)
				return;
			
			if (gizmosSurfingEdges.draw) DrawSurfingEdges();
			if (gizmosHuggingEdges.draw) DrawHuggingEdges();
			if (drawSortedCirclePoints) DrawSortedCirclePoints();
			if (gizmosGraph.draw) DrawGraph();
		}

		private void DrawSurfingEdges()
		{
			circularGenerator.SurfingEdges.ForEachDictListElem((_, edge) =>
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
			circularGenerator.HuggingEdges.ForEachDictListElem(edge =>
			{
				Gizmos.color = gizmosHuggingEdges.color;
				Gizmos.DrawLine(edge.a.ToVec3(gizmosHeight), edge.b.ToVec3(gizmosHeight));
			});
		}

		private void DrawSortedCirclePoints()
		{
			circularGenerator.PointsOnCircle.ForEachDictList((circleHash, pointList) =>
			{
				for (var i = 0; i < pointList.Count; i++)
				{
					var t = pointList.Count > 1 ? (float) i / (pointList.Count - 1) : 0;
					var center = circularGenerator.Circles[circleHash].center.ToVec3(gizmosHeight);
					Gizmos.color = Color.Lerp(Color.red, Color.blue, t);
					Gizmos.DrawLine(center, Vector3.Lerp(center, pointList[i].ToVec3(gizmosHeight), Mathf.Lerp(0.5f, 1f, t)));
				}
			});
		}

		private void DrawGraph()
		{
			Gizmos.color = gizmosGraph.color;
			foreach (var node in circularGenerator.graph)
			{
				foreach (var connectedNode in node.Links)
				{
					Gizmos.DrawLine(node.Content.ToVec3(gizmosHeight), connectedNode.Node.Content.ToVec3(gizmosHeight));
				}
			}
		}

		private void ToggleObsticlesVisibility()
		{
			circlesDebug.ForEach(tcc => tcc.Item1.gameObject.SetActive(!hideObstacles));
			circlesDebug.ForEach(tcc => tcc.Item2.enabled = !hideObstacleMeshRenderers);
		}
		
		#endregion
	}
}