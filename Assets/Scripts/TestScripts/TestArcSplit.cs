using System;
using System.Collections.Generic;
using System.Linq;
using Pathfinding.CircularObstacleGraph;
using UnityEngine;
using Utils;

namespace TestScripts
{
	public class TestArcSplit : MonoBehaviour
	{
		[Header("Gizmos Drawing")]
		[SerializeField] private Color aColor;
		[SerializeField] private Color bColor;
		[SerializeField] private Color middlePointsColor;
		[SerializeField] private float pointRadius;

		[SerializeField] private float gizmosHeight = -2f;
		
		[Header("Test Params")]
		[SerializeField] [Range(0, 360)] private float aRotation = 0f;
		[SerializeField] [Range(0, 360)] private float bRotation = 45f;
		[SerializeField] [Range(1, 6)] private int splits = 1;

		[Header("Debug Values")]
		[SerializeField] private float arcLength;
		
		private SphereCollider _circleCollider2D;
		private Circle _circle;

		private Vector2 _a;
		private Vector2 _b;
		private List<Vector2> _splittedPoints;

		private void Awake()
		{
			_circleCollider2D = GetComponent<SphereCollider>();
		}

		private void Update()
		{
			_circle = new Circle(_circleCollider2D.radius, _circleCollider2D.transform.position.ToVec2());
			
			_a = Vector2.up * _circle.radius;
			_a = _a.Rotate(aRotation * Mathf.Deg2Rad);
			_b = Vector2.up * _circle.radius;
			_b = _b.Rotate(bRotation * Mathf.Deg2Rad);
			
			SplitArc();
			
			CalculateArcLength();
		}

		private void CalculateArcLength()
		{
			arcLength = _circle.ArcLength(Vector2.SignedAngle(_a, _b));
		}

		private void OnDrawGizmos()
		{
			if (!Application.isPlaying) return;

			Gizmos.color = aColor;
			Gizmos.DrawSphere((_a + _circle.center).ToVec3(gizmosHeight), pointRadius);

			Gizmos.color = bColor;
			Gizmos.DrawSphere((_b + _circle.center).ToVec3(gizmosHeight), pointRadius);

			Gizmos.color = middlePointsColor;
			foreach (var point in _splittedPoints)
			{
				Gizmos.DrawSphere(point.ToVec3(gizmosHeight), pointRadius);
			}

			Gizmos.color = Color.red;
			
		}

		private void SplitArc()
		{
			_splittedPoints = MathUtils.SplitArc(_a, _b, splits).Select(p => p + _circle.center).ToList();
			// _splittedPoints = MathUtils.SplitRecursive(_a, _b, splitsDepth);
		}
	}
}