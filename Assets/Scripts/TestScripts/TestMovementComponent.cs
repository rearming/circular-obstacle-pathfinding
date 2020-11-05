using System;
using System.Linq;
using EditorExtensions;
using TestingEnvironmentScripts;
using UnityEngine;

namespace TestScripts
{
	public class TestMovementComponent : MonoBehaviour
	{
		[SerializeField] private Transform parentPointsTransform;
		
		private Movement _movement;
		
		private Vector3 [] _movementPoints;

		private void Awake()
		{
			_movement = GetComponent<Movement>();
		}

		private void Start()
		{
			_movementPoints = GetMovementPoints();
		}

		[ExposeMethodInEditor]
		private void MoveByPoints()
		{
			_movement.MoveByPoints(_movementPoints);
		}

		private Vector3[] GetMovementPoints()
		{
			return parentPointsTransform.GetComponentsInChildren<Transform>()
				.Where(t => t.gameObject != parentPointsTransform.gameObject)
				.Select(t => t.position)
				.ToArray();
		}
	}
}