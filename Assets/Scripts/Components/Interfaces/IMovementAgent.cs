using UnityEngine;

namespace Components.Interfaces
{
	public interface IMovementAgent
	{
		Vector2? GetGoal();
		float GetSpeed();
		void UnsetGoal();
	}
}