using System;
using TestingEnvironmentScripts;
using UnityEngine;
using UnityEngine.UIElements.Experimental;
using Utils;

namespace TestScripts
{
	public class SlerpTest : MonoBehaviour
	{
		[SerializeField] private Transform pos1;
		[SerializeField] private Transform pos2;

		[Range(0f, 1f)] [SerializeField] private float t;

		[SerializeField] private Transform arcMiddle;

		private MovementComponent movementComponent;

		private void Awake()
		{
			movementComponent = GetComponent<MovementComponent>();
		}

		private void Update()
		{
			Vector2 arcPoint;
			
			if (movementComponent.IsRealNull())
			{
				arcPoint = CalculateArc();
				transform.position = pos1.position.ToVec2().Slerp(arcPoint, t).ToVec3(transform.position.y);
			}
			else
			{
				var distance = Vector2.Distance(transform.position.ToVec2(), pos2.position.ToVec2()) / 2;
				// distance = ;
				arcPoint = transform.position.ToVec2().ArcPoint(pos2.position.ToVec2(), distance);
				movementComponent.MovementDir = (arcPoint - transform.position.ToVec2()).normalized;
			}
			
			arcMiddle.transform.position = arcPoint.ToVec3(arcMiddle.transform.position.y);
		}

		private Vector2 CalculateArc()
		{
			var a = pos1.position.ToVec2();
			var b = pos2.position.ToVec2();
			var c = (a + b) / 2;

			var ab = (b - a).normalized;
			var x = c + new Vector2(-ab.y, ab.x) * (Vector2.Distance(a, b) / 2);

			return x;
		}
	}
}