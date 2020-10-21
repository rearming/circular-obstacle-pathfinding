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

		[SerializeField] private float gizmosHeight = 1f;
		
		private Circle[] circles;
		private CircularObsticleGraphGenerator<Vector2> CircularGenerator;
		
		

		private void Start()
		{
			circlesObstacles = circlesTParent.GetComponentsInChildren<CapsuleCollider>()
				.Where(cc => cc.gameObject.activeSelf)
				.Select(cc => (cc.gameObject.transform, cc))
				.ToArray();
			
			GetCircles();
			
			CircularGenerator = new CircularObsticleGraphGenerator<Vector2>(circles, start.position.ToVec2(), goal.position.ToVec2());
			
			CircularGenerator.GenerateGraph();
			
		}

		private void Update()
		{
			GetCircles();
			CircularGenerator.SetCircles(circles);
			CircularGenerator.GenerateGraph();
		}

		private void PrintPointsOnCircle()
		{
			Debug.Log($"points on circle count: [{CircularGenerator.PointsOnCircle.Count.ToString()}]");
			CircularGenerator.PointsOnCircle.ForEachDictListElem((circleHash, point) =>
			{
				Debug.Log($"circle hash: [{circleHash.ToString()}], point: [{point.ToString()}]");
			});
		}

		private void OnDrawGizmos()
		{
			if (!Application.isPlaying)
				return;
			
			DrawSurfingEdges();
			DrawSortedCirclePoints();
			DrawHuggingEdges();
		}

		private void DrawSurfingEdges()
		{
			CircularGenerator.SurfingEdges.ForEachDictListElem((_, edge) =>
			{
				Gizmos.color = Color.green;
				Gizmos.DrawLine(edge.a.ToVec3(gizmosHeight), edge.b.ToVec3(gizmosHeight));
				Gizmos.color = Color.yellow;
				Gizmos.DrawSphere(edge.a.ToVec3(gizmosHeight), 0.04f);
				Gizmos.DrawSphere(edge.b.ToVec3(gizmosHeight), 0.04f);
			});
		}

		private void DrawSortedCirclePoints()
		{
			CircularGenerator.PointsOnCircle.ForEachDictList((circleHash, pointList) =>
			{
				for (var i = 0; i < pointList.Count; i++)
				{
					var t = pointList.Count > 1 ? (float) i / (pointList.Count - 1) : 0;
					var center = CircularGenerator.Circles[circleHash].center.ToVec3(gizmosHeight);
					Gizmos.color = Color.Lerp(Color.red, Color.blue, t);
					Gizmos.DrawLine(center, Vector3.Lerp(center, pointList[i].ToVec3(gizmosHeight), Mathf.Lerp(0.5f, 1f, t)));
				}
			});
		}

		private void DrawHuggingEdges()
		{
			CircularGenerator.HuggingEdges.ForEachDictListElem(edge =>
			{
				Gizmos.color = ColorUtility.TryParseHtmlString("#a200cf", out var color) ? color : Color.blue;
				Gizmos.DrawLine(edge.a.ToVec3(gizmosHeight), edge.b.ToVec3(gizmosHeight));
			});
		}

		private void OnEnable()
		{
			SceneView.duringSceneGui += SceneUpdate;
		}

		private void SceneUpdate(SceneView sceneView)
		{
			// Handles.DrawBezier(Vector3.zero, Vector3.up, Vector3.right, 145, 2);
		}
		
		private void OnDisable()
		{
			SceneView.duringSceneGui -= SceneUpdate;
		}

		private void GetCircles()
		{
			circles = circlesObstacles.Select(t => new Circle(t.Item2.ScaledRadius(), t.Item1.position.ToVec2())).ToArray();
		}
	}
}