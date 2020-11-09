using System;
using ScriptableObjects;
using UnityEngine;

namespace TestingEnvironmentScripts
{
	public class NeutralSquadInput : MonoBehaviour
	{
		[SerializeField] private NeutralSquadKeymap keymap;

		private Action _selection;
		private Action _move;
		private Action _multipleSelection;
		private Action _toggleStopMovement;

		public void SetupCallbacks(Action selection, Action move, Action multipleSelection, Action toggleStopMovement)
		{
			_selection = selection;
			_move = move;
			_multipleSelection = multipleSelection;
			_toggleStopMovement = toggleStopMovement;
		}
		
		private void Update()
		{
			if (Input.GetMouseButtonDown((int) keymap.SelectButton)) _selection.Invoke();
			if (Input.GetMouseButtonDown((int) keymap.MoveButton)) _move.Invoke();
			if (Input.GetKey(keymap.MultipleSelection)) _multipleSelection.Invoke();
			if (Input.GetKeyDown(keymap.ToggleStopMovement)) _toggleStopMovement.Invoke();
		}
	}
}