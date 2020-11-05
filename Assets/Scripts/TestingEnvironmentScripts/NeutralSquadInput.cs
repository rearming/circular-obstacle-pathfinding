using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace TestingEnvironmentScripts
{
	public class NeutralSquadInput : MonoBehaviour
	{
		[Serializable]
		public enum MouseButton
		{
			LeftButton = 0,
			RightButton = 1
		}

		[SerializeField] private MouseButton selectButton;
		[SerializeField] private MouseButton moveButton;
		[SerializeField] private KeyCode multipleSelection = KeyCode.LeftControl;
		[SerializeField] private KeyCode toggleStopMovement = KeyCode.Space;
		private readonly HashSet<Neutral> _selectedNeutrals = new HashSet<Neutral>();

		private (Neutral, CircularPathfinder)[] _allNeutrals;

		private Camera _cam;

		private void Awake()
		{
			_cam = Camera.main;
		}

		private void Start()
		{
			_allNeutrals = FindObjectsOfType<Neutral>()
				.Select(nc => (nc, nc.GetComponent<CircularPathfinder>()))
				.ToArray();
			StartCoroutine(ToggleStopNeutrals());
		}

		private void Update()
		{
			if (Input.GetMouseButtonDown((int) selectButton))
				SelectNeutral();
			if (Input.GetMouseButtonDown((int) moveButton))
				MoveNeutrals();
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

			if (!Input.GetKey(multipleSelection))
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

		private void MoveNeutrals()
		{
			if (!RaycastMousePos(out _, out var pos))
				return;
			_selectedNeutrals.ForEach(n => n.SetGoal(pos));
		}

		private IEnumerator ToggleStopNeutrals()
		{
			var stopped = false;
			while (true)
			{
				if (Input.GetKeyDown(toggleStopMovement))
					stopped = !stopped;
				if (stopped)
					_selectedNeutrals.ForEach(n => n.Stop());
				yield return null;
			}
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