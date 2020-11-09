using System;
using UnityEngine;

namespace ScriptableObjects
{
	[CreateAssetMenu(fileName = "Neutral Squad Keymap_1", menuName = "Keymaps/Neutral Squad", order = 0)]
	public class NeutralSquadKeymap : ScriptableObject
	{
		[Serializable]
		public enum MouseButton
		{
			LeftButton = 0,
			RightButton = 1
		}

		[SerializeField] private MouseButton selectButton;
		public MouseButton SelectButton => selectButton;
		
		[SerializeField] private MouseButton moveButton;
		public MouseButton MoveButton => moveButton;
		
		[SerializeField] private KeyCode multipleSelection = KeyCode.LeftControl;
		public KeyCode MultipleSelection => multipleSelection;

		[SerializeField] private KeyCode toggleStopMovement = KeyCode.Space;
		public KeyCode ToggleStopMovement => toggleStopMovement;
	}
}