using System;
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

		public void MoveTowards(Vector3 point)
		{
			var dir = (point - transform.position).normalized;
			movementComponent.MovementDir = new Vector2(dir.x, dir.z);
		}
	}
}