using ScriptableObjects;
using UnityEngine;

namespace TestingEnvironmentScripts
{
    public class MovementComponent : MonoBehaviour
    {
        [SerializeField] private MovementSpec movementSpec;
    
        public Vector2 MovementDir { get; set; }
    
        void Update()
        {
            transform.position += new Vector3(MovementDir.x, 0, MovementDir.y) * (movementSpec.Speed * Time.deltaTime);
        }
    }
}
