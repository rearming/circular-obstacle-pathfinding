using System;
using System.Collections;
using UnityEngine;

namespace UnityTemplateProjects
{
	public class NeutralComponent : MonoBehaviour
	{
		[SerializeField] private Color selectionColor = Color.green;

		private Color baseColor;
		public Material Material { get; private set; }

		private MovementComponent movementComponent;

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

		public void MoveTowards(Vector3 pos)
		{
			StartCoroutine(MoveTowardsRoutine(pos));
		}
		
		private IEnumerator MoveTowardsRoutine(Vector3 point)
		{
			var dir = (point - transform.position).normalized;
			SetMovement(dir);
			yield return new WaitWhile(() => Vector2.Distance(transform.position.ToVec2(), point.ToVec2()) > 0.1f);
			SetMovement(Vector2.zero);
		}
	}
}