using System.Collections.Generic;
using System.Linq;
using EditorExtensions;
using TestingEnvironmentScripts;
using UnityEngine;
using Utils;

namespace TestScripts
{
	public class TestMovementComponent : MonoBehaviour
	{
		[SerializeField] private Transform parentPointsTransform;
		
		private Movement _movement;
		
		private List<Vector2> _movementPoints;

		private void Awake()
		{
			_movement = GetComponent<Movement>();
		}

		private void Start()
		{
			_movementPoints = GetMovementPoints();
		}

		[ExposeMethodInEditor]
		// ReSharper disable once UnusedMember.Local
		private void MoveByPoints()
		{
			_movement.MoveByPoints(_movementPoints);
		}

		private List<Vector2> GetMovementPoints()
		{
			return parentPointsTransform.GetComponentsInChildren<Transform>()
				.Where(t => t.gameObject != parentPointsTransform.gameObject)
				.Select(t => t.position.ToVec2())
				.ToList();
		}
	}
}