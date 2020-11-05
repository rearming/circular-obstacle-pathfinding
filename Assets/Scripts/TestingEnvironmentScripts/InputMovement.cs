using UnityEngine;

namespace TestingEnvironmentScripts
{
	public class InputMovement : MonoBehaviour
	{
		[SerializeField] private bool invertX;
		[SerializeField] private bool invertY;

		private Movement _movement;

		private void Start()
		{
			_movement = GetComponent<Movement>();
		}

		private void Update()
		{
			var x = Input.GetAxis("Horizontal") * (invertX ? -1 : 1);
			var y = Input.GetAxis("Vertical") * (invertY ? -1 : 1);
			_movement.MovementDir = new Vector2(x, y);
		}
	}
}