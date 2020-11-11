using System;
using System.Collections;
using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;
using Utils;

namespace Components
{
	public class Movement : MonoBehaviour
	{
		[SerializeField] private MovementSpec movementSpec;

		public Vector2 MovementDir { get; set; }
		public float Speed => movementSpec.Speed;
		public Vector2? Velocity { get; private set; }

		private Coroutine _moveByPointsRoutine;
		private Coroutine _moveTowardsRoutine;

		private const float DefaultPointReachDistance = 0.05f;

		public void MoveVelocity(Vector2 velocity)
		{
			Velocity = velocity;
		}

		public void Stop()
		{
			StopAllCoroutines();
			MovementDir = Vector2.zero;
		}

		public void MoveByPoints(List<Vector2> points, float pointReachDistance = DefaultPointReachDistance, Action onMovementEnded = null)
		{
			if (_moveByPointsRoutine != null)
				StopCoroutine(_moveByPointsRoutine);
			_moveByPointsRoutine = StartCoroutine(MoveByPointsRoutine(points, pointReachDistance, onMovementEnded));
		}

		public void MoveTowards(Vector2 pos, float pointReachDistance = DefaultPointReachDistance, Action onPointReached = null)
		{
			if (_moveTowardsRoutine != null)
				StopCoroutine(_moveTowardsRoutine);
			_moveTowardsRoutine = StartCoroutine(MoveTowardsRoutine(pos, pointReachDistance, onPointReached));
		}

		private void FixedUpdate()
		{
			if (Velocity != null)
				transform.position += new Vector3(Velocity.Value.x, 0, Velocity.Value.y) * Time.deltaTime; 
			else
				transform.position += new Vector3(MovementDir.x, 0, MovementDir.y) * (movementSpec.Speed * Time.deltaTime);
		}

		private IEnumerator MoveByPointsRoutine(List<Vector2> points, float pointReachDistance, Action onMovementEnded)
		{
			var currentPointIdx = 0;
			while (currentPointIdx < points.Count)
			{
				var localTargetReached = false;
				var idx = currentPointIdx;
				var currentTarget = points[idx];
				MoveTowards(currentTarget, pointReachDistance, () => localTargetReached = true);
				yield return new WaitWhile(() =>
				{
					if (currentTarget != points[idx])
					{
						currentTarget = points[idx];
						MoveTowards(currentTarget, pointReachDistance, () => localTargetReached = true);
					}
					return !localTargetReached;
				});
				currentPointIdx++;
			}
			onMovementEnded?.Invoke();
		}

		private IEnumerator MoveTowardsRoutine(Vector2 point, float pointReachDistance, Action onPointReached)
		{
			MovementDir = (point - transform.position.ToVec2()).normalized;
			yield return new WaitWhile(() => Vector2.Distance(transform.position.ToVec2(), point) > pointReachDistance);
			MovementDir = Vector2.zero;
			onPointReached?.Invoke();
		}
	}
}