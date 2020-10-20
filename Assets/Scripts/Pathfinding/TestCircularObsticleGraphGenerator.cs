using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace Pathfinding
{
	public class TestCircularObsticleGraphGenerator : MonoBehaviour
	{
		[SerializeField] private Transform circlesTParent;
		private Tuple<Transform, CapsuleCollider> [] circlesObstacles;
		
		[SerializeField] private Transform start;
		[SerializeField] private Transform goal;

		[SerializeField] private float gizmosHeight = 1f;
		
		private Circle[] circles;
		private CircularObsticleGraphGenerator<Vector2> CircularGenerator;

		private void Start()
		{
			circlesObstacles = circlesTParent.GetComponentsInChildren<CapsuleCollider>()
				.Where(cc => cc.gameObject.activeSelf)
				.Select(cc => Tuple.Create(cc.gameObject.transform, cc))
				.ToArray();
			
			GetCircles();
			
			CircularGenerator = new CircularObsticleGraphGenerator<Vector2>(circles, start.position.ToVec2(), goal.position.ToVec2());

			// PrintBitangents();
		}

		private void Update()
		{
			GetCircles();
			CircularGenerator.SetCircles(circles);
			CircularGenerator.GenerateGraph();
		}

		private void PrintBitangents()
		{
			CircularGenerator.ForEachBitangent(bitangent =>
			{
				Debug.Log($"bitangent [{bitangent.ToString()}]");
				Debug.Log($"hash: [{bitangent.GetHashCode().ToString()}]");
			});
		}
		
		private void OnDrawGizmos()
		{
			if (!Application.isPlaying)
				return;
			Gizmos.color = Color.green;
			var conv = new Func<Vector2, float, Vector3>((vec, height) => new Vector3(vec.x, height, vec.y));
			CircularGenerator.ForEachBitangent(bitangent =>
			{
				Gizmos.DrawLine(conv(bitangent.a, gizmosHeight), conv(bitangent.b, gizmosHeight));
			});
		}

		private void GetCircles()
		{
			circles = circlesObstacles.Select(t => new Circle(t.Item2.ScaledRadius(), t.Item1.position.ToVec2())).ToArray();
		}
	}
}