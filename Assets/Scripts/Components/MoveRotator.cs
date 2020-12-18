using System;
using UnityEngine;
using Utils;

namespace Components
{
	[RequireComponent(typeof(Movement))]
	public class MoveRotator : MonoBehaviour
	{
		[SerializeField] private float rotationSpeed = 4f;

		private Movement _movement;

		private void Awake()
		{
			_movement = GetComponent<Movement>();
		}

		private void Update()
		{
			if (_movement.MovementDir == Vector2.zero)
				return;
			var rotationTarget = Quaternion.LookRotation(_movement.MovementDir.ToVec3(0f));
			transform.rotation = Quaternion.RotateTowards(transform.rotation, rotationTarget, rotationSpeed);
		}
	}
}