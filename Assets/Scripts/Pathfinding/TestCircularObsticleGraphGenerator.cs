using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pathfinding
{
	public class TestCircularObsticleGraphGenerator : MonoBehaviour
	{
		[SerializeField] private Transform [] circlesT;
		
		[SerializeField] private Transform start;
		[SerializeField] private Transform goal;
		
		private Circle[] circles;
		private CircularObsticleGraphGenerator<Vector2> CircularGenerator;

		private void Start()
		{
			GetCircles();
			
			CircularGenerator = new CircularObsticleGraphGenerator<Vector2>(circles, start.position.ToVec2(), goal.position.ToVec2());
			
			CircularGenerator.GenerateGraph();
			
			// TestHashSet2();
			PrintBitangents();
			// TestHashSet();
		}

		private void PrintBitangents()
		{
			foreach (var bitangent in CircularGenerator.Bitangents)
			{
				Debug.Log($"bitangent [{bitangent.ToString()}]");
			}
		}

		public struct BrokenHash
		{
			private int hash;
			public int randomNum;

			public BrokenHash(int hash, int randomNum)
			{
				this.hash = hash;
				this.randomNum = randomNum;
			}

			public override bool Equals(object obj)
			{
				return (obj is BrokenHash brokenHash) && brokenHash.hash == hash && brokenHash.randomNum == randomNum;
			}

			public override int GetHashCode() => randomNum.GetHashCode();

			public override string ToString() => $"data: [{randomNum.ToString()}]";

		}
		
		private void TestHashSet2()
		{
			var hs1 = new HashSet<BrokenHash>();

			hs1.Add(new BrokenHash(1, 32));
			hs1.Add(new BrokenHash(1, 52));
			hs1.Add(new BrokenHash(1, 88));
			hs1.Add(new BrokenHash(3, 32));
			hs1.Add(new BrokenHash(1, 32));
			
			var hs2 = new HashSet<BrokenHash>();
			hs2.Add(new BrokenHash(1, 44));
			hs2.Add(new BrokenHash(2, 32));
			hs2.Add(new BrokenHash(2, 66));
			
			PrintHashSet(hs1, "hs1");
			PrintHashSet(hs2, "hs2");
			
			hs1.UnionWith(hs2);
			PrintHashSet(hs1, "UNION");
		}
		
		private void TestHashSet()
		{
			var hs1 = new HashSet<Bitangent>();
			hs1.Add(new Bitangent(new Vector2(14, 5), new Vector2(5, 14)));
			hs1.Add(new Bitangent(Vector2.one, Vector2.one));
			hs1.Add(new Bitangent(Vector2.zero, Vector2.zero));
			var hs2 = new HashSet<Bitangent>();
			hs2.Add(new Bitangent(new Vector2(5, 14), new Vector2(14, 5)));
			hs2.Add(new Bitangent(Vector2.one, Vector2.one));
			hs2.Add(new Bitangent(Vector2.down, Vector2.down));

			PrintHashSet(hs1, "hs1");
			PrintHashSet(hs2, "hs2");
			hs1.UnionWith(hs2);
			PrintHashSet(hs1, "union");
		}

		private void PrintHashSet<T>(HashSet<T> hs, string name)
		{
			Debug.Log($"Printing hash set [{name}]");
			foreach (var item in hs)
			{
				Debug.Log($"item: [{item.ToString()}]");
				Debug.Log($"hash code: [{item.GetHashCode().ToString()}]");
			}
			Debug.Log("-------------------------");
		}
		

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.green;
			// Gizmos.DrawLine();
		}

		private void GetCircles()
		{
			circles = circlesT.Select(t => new Circle(t.GetComponent<CapsuleCollider>().radius, t.position)).ToArray();
		}
	}
}