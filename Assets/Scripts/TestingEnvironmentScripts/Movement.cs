using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ScriptableObjects;
using UnityEditor.Rendering;
using UnityEngine;
using Utils;

namespace TestingEnvironmentScripts
{
	public class Movement : MonoBehaviour
	{
		[SerializeField] private MovementSpec movementSpec;

		public Vector2 MovementDir { get; set; }
		public float Speed => movementSpec.Speed;

		private Coroutine _moveByPointsRoutine;
		private Coroutine _moveTowardsRoutine;

		private const float DefaultPointReachDistance = 0.05f;

		public void MoveByPoints(IEnumerable<Vector3> points, float pointReachDistance = DefaultPointReachDistance, Action onMovementEnded = null)
		{
			if (_moveByPointsRoutine != null)
				StopCoroutine(_moveByPointsRoutine);
			_moveByPointsRoutine = StartCoroutine(MoveByPointsRoutine(points, pointReachDistance, onMovementEnded));
		}

		public void MoveTowards(Vector3 pos, float pointReachDistance = DefaultPointReachDistance, Action onPointReached = null)
		{
			if (_moveTowardsRoutine != null)
				StopCoroutine(_moveTowardsRoutine);
			_moveTowardsRoutine = StartCoroutine(MoveTowardsRoutine(pos, pointReachDistance, onPointReached));
		}

		public void Stop()
		{
			StopAllCoroutines();
			MovementDir = Vector2.zero;
		}

		private void Update()
		{
			transform.position += new Vector3(MovementDir.x, 0, MovementDir.y) * (movementSpec.Speed * Time.deltaTime);
		}

		private IEnumerator MoveByPointsRoutine(IEnumerable<Vector3> points, float pointReachDistance, Action onMovementEnded)
		{
			var pointsEnumerator = points.GetEnumerator();
			while (pointsEnumerator.MoveNext())
			{
				var localTargetReached = false;
				MoveTowards(pointsEnumerator.Current, pointReachDistance, () => localTargetReached = true);
				yield return new WaitWhile(() => !localTargetReached);
			}
			onMovementEnded?.Invoke();
		}

		private IEnumerator MoveTowardsRoutine(Vector3 point, float pointReachDistance, Action onPointReached)
		{
			MovementDir = (point - transform.position).normalized.ToVec2();
			yield return new WaitWhile(() => Vector2.Distance(transform.position.ToVec2(), point.ToVec2()) > pointReachDistance);
			MovementDir = Vector2.zero;
			onPointReached?.Invoke();
		}
	}
}