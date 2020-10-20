using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace Pathfinding
{
	public class TestCircularObsticleGraphGenerator : MonoBehaviour
	{
		[SerializeField] private Transform [] circlesT;
		
		[SerializeField] private Transform start;
		[SerializeField] private Transform goal;

		[SerializeField] private float gizmosHeight = 1f;
		
		private Circle[] circles;
		private CircularObsticleGraphGenerator<Vector2> CircularGenerator;

		private void Start()
		{
			GetCircles();
			
			CircularGenerator = new CircularObsticleGraphGenerator<Vector2>(circles, start.position.ToVec2(), goal.position.ToVec2());
			
			CircularGenerator.GenerateGraph();
			
			// PrintBitangents();
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
			circles = circlesT.Select(t => 
				new Circle(t.GetComponent<CapsuleCollider>().ScaledRadius(), t.position.ToVec2())).ToArray();
		}
	}
}