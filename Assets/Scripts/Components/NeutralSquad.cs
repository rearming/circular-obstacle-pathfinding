using System.Collections.Generic;
using System.Linq;
using Components;
using UnityEngine;
using Utils;

namespace TestingEnvironmentScripts
{
	public class NeutralSquad : MonoBehaviour
	{
		[SerializeField] private NeutralSquadInput input;

		private readonly HashSet<Neutral> _selectedNeutrals = new HashSet<Neutral>();
		private (Neutral, CircularPathfinder)[] _allNeutrals;

		private Camera _cam;

		private bool _neutralsStopped;
		private bool _multipleSelection;
		
		private void Awake()
		{
			_cam = Camera.main;
			input = input == null ? GetComponent<NeutralSquadInput>() : input;
			Debug.Assert(input != null, "Neutral Squad Component requires Input reference assigned.");
		}

		private void Start()
		{
			_allNeutrals = FindObjectsOfType<Neutral>()
				.Select(nc => (nc, nc.GetComponent<CircularPathfinder>()))
				.ToArray();
			
			input.SetupCallbacks(
				SelectNeutral,
				MoveNeutralsToMousePos,
				() => _multipleSelection = true,
				ToggleStopNeutrals);
		}

		private void SelectNeutral()
		{
			if (!RaycastMousePos(out var hit, out _))
				return;
			if (!hit.collider.gameObject.TryGetComponent<Neutral>(out var neutral))
			{
				DeselectAll();
				return;
			}
			if (!_multipleSelection)
				DeselectAll();
			
			if (!_selectedNeutrals.AddWithAction(neutral, n => n.OnSelect()))
				_selectedNeutrals.RemoveWithAction(neutral, n => n.OnDeselect());
		}

		private void DeselectAll()
		{
			foreach (var neutral in _selectedNeutrals)
				neutral.OnDeselect();
			_selectedNeutrals.Clear();
		}

		private void MoveNeutralsToMousePos()
		{
			if (!RaycastMousePos(out _, out var pos))
				return;
			_selectedNeutrals.ForEach(n => n.SetGoal(pos));
		}

		private void ToggleStopNeutrals()
		{
			_neutralsStopped = !_neutralsStopped;
			if (_neutralsStopped)
				_selectedNeutrals.ForEach(n => n.Stop());
		}

		private bool RaycastMousePos(out RaycastHit hit, out Vector3 pos)
		{
			pos = Vector3.zero;
			var ray = _cam.ScreenPointToRay(Input.mousePosition);
			if (!Physics.Raycast(ray, out hit))
				return false;
			pos = hit.point;
			return true;
		}
	}
}