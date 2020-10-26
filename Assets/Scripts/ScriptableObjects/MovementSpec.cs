using UnityEngine;

namespace ScriptableObjects
{
	[CreateAssetMenu(fileName = "MovementSpec_1", menuName = "Specs/Movement Spec", order = 0)]
	public class MovementSpec : ScriptableObject
	{
		[SerializeField] private float speed = 2f;
		public float Speed => speed;
	}
}