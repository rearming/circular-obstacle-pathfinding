using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RVO;
using UnityEngine;
using Utils;

namespace Components.CollisionAvoidance
{
	public class CollisionAvoidanceStaticObstacles : MonoBehaviour
	{
		private CollisionAvoidanceOverlapper _overlapper;

		private List<Mesh> _meshes;
		public List<List<RVO.Vector2>> Obstacles { get; private set; }

		private void Awake()
		{
			_meshes = GetComponentsInChildren<MeshFilter>().Select(m => m.mesh).ToList();
			
			_overlapper = GetComponent<CollisionAvoidanceOverlapper>();
			_overlapper.SetupCallbacks(contacts =>
			{
				Obstacles = contacts.Select(list => list.Select(v => v.ToRVOVec2()).ToList()).ToList();
			});
		}

		public void AddObstacles()
		{
			Obstacles = null;
			_overlapper.Setup();
			StartCoroutine(AddObstaclesRoutine());
		}
		
		private IEnumerator AddObstaclesRoutine()
		{
			yield return new WaitWhile(() => Obstacles == null);
			Simulator.Instance.obstacles_ = new List<Obstacle>();
			foreach (var obstacle in Obstacles)
				Simulator.Instance.addObstacle(obstacle);
			Simulator.Instance.processObstacles();
		}
	}
}