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
			PrintPointsOnCircle();
		}

		private void Update()
		{
			GetCircles();
			CircularGenerator.SetCircles(circles);
			CircularGenerator.GenerateGraph();
		}

		private void PrintPointsOnCircle()
		{
			Debug.Log($"points on circle count: [{CircularGenerator.pointsOnCircle.Count.ToString()}]");
			CircularGenerator.pointsOnCircle.ForEachDictList((circleHash, point) =>
			{
				Debug.Log($"circle hash: [{circleHash.ToString()}], point: [{point.ToString()}]");
			});
		}

		private void OnDrawGizmos()
		{
			if (!Application.isPlaying)
				return;
			CircularGenerator.surfingEdges.ForEachDictList((_, edge) =>
			{
				Gizmos.color = Color.green;
				Gizmos.DrawLine(Convert(edge.a, gizmosHeight), Convert(edge.b, gizmosHeight));
				Gizmos.color = Color.yellow;
				Gizmos.DrawSphere(Convert(edge.a, gizmosHeight), 0.04f);
				Gizmos.DrawSphere(Convert(edge.b, gizmosHeight), 0.04f);

				Vector3 Convert(Vector2 vec, float height) => new Vector3(vec.x, height, vec.y);
			});
		}

		private void OnEnable()
		{
			SceneView.duringSceneGui += SceneUpdate;
		}

		private void SceneUpdate(SceneView sceneView)
		{
			Handles.DrawWireArc(Vector3.zero, Vector3.up, Vector3.right, 145, 2);
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