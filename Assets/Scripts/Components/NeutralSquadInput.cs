using System;
using ScriptableObjects;
using UnityEngine;

namespace Components
{
	public class NeutralSquadInput : MonoBehaviour
	{
		[SerializeField] private NeutralSquadKeymap keymap;

		private Action _selection;
		private Action _move;
		private Action _toggleStopMovement;
		private Action<bool> _toggleMultipleSelection;

		public void SetupCallbacks(Action selection, Action move, Action toggleStopMovement, Action<bool> toggleMultipleSelection)
		{
			_selection = selection;
			_move = move;
			_toggleMultipleSelection = toggleMultipleSelection;
			_toggleStopMovement = toggleStopMovement;
		}
		
		private void Update()
		{
			if (Input.GetMouseButtonDown((int) keymap.SelectButton)) _selection.Invoke();
			if (Input.GetMouseButtonDown((int) keymap.MoveButton)) _move.Invoke();
			if (Input.GetKeyDown(keymap.ToggleStopMovement)) _toggleStopMovement.Invoke();
			if (Input.GetKey(keymap.MultipleSelection)) _toggleMultipleSelection.Invoke(true);
			if (Input.GetKeyUp(keymap.MultipleSelection)) _toggleMultipleSelection.Invoke(false);
		}
	}
}