using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using Utils;

namespace TestingEnvironmentScripts
{
	public class NeutralComponent : MonoBehaviour
	{
		[SerializeField] private Color selectionColor = Color.green;

		private Color baseColor;
		public Material Material { get; private set; }

		private MovementComponent movementComponent;
		
		public Vector2? Goal { get; private set; }

		private void Awake()
		{
			Material = GetComponent<Renderer>().material;
			movementComponent = GetComponent<MovementComponent>();
		}

		private void Start()
		{
			baseColor = Material.color;
		}

		public void OnSelect()
		{
			Material.color = selectionColor;
		}

		public void OnDeselect()
		{
			Material.color = baseColor;
		}
		
		public void SetMovement(Vector2 dir)
		{
			movementComponent.MovementDir = dir;
		}

		public void SetMovement(Vector3 dir)
		{
			SetMovement(dir.ToVec2());
		}

		public void SetGoal(Vector3 target)
		{
			Goal = target.ToVec2();
		}

		public void UnsetGoal() => Goal = null;

		public void MoveTowards(Vector3 pos, Action onPointReached)
		{
			StartCoroutine(MoveTowardsRoutine(pos, onPointReached));
		}
		
		private IEnumerator MoveTowardsRoutine(Vector3 point, Action onPointReached)
		{
			var dir = (point - transform.position).normalized;
			SetMovement(dir);
			yield return new WaitWhile(() => Vector2.Distance(transform.position.ToVec2(), point.ToVec2()) > 0.1f);
			SetMovement(Vector2.zero);
			onPointReached?.Invoke();
		}
	}
}