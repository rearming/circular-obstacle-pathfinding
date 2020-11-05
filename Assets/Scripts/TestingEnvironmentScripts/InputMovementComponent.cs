using UnityEngine;

namespace TestingEnvironmentScripts
{
	public class InputMovementComponent : MonoBehaviour
	{
		[SerializeField] private bool invertX;
		[SerializeField] private bool invertY;

		private MovementComponent _movementComponent;

		private void Start()
		{
			_movementComponent = GetComponent<MovementComponent>();
		}

		private void Update()
		{
			var x = Input.GetAxis("Horizontal") * (invertX ? -1 : 1);
			var y = Input.GetAxis("Vertical") * (invertY ? -1 : 1);
			_movementComponent.MovementDir = new Vector2(x, y);
		}
	}
}