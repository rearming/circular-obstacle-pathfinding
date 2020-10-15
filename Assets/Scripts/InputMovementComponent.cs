using System;
using UnityEngine;

namespace UnityTemplateProjects
{
	public class InputMovementComponent : MonoBehaviour
	{
		[SerializeField] private bool invertX;
		[SerializeField] private bool invertY;
		
		private MovementComponent movementComponent;

		private void Start()
		{
			movementComponent = GetComponent<MovementComponent>();
		}

		private void Update()
		{
			var x = Input.GetAxis("Horizontal") * (invertX ? -1 : 1);
			var y = Input.GetAxis("Vertical") * (invertY ? -1 : 1);
			movementComponent.MovementDir = new Vector2(x, y);
		}
	}
}