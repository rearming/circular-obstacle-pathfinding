#if ENABLE_INPUT_SYSTEM && ENABLE_INPUT_SYSTEM_PACKAGE
#define USE_INPUT_SYSTEM
    using UnityEngine.InputSystem;
    using UnityEngine.InputSystem.Controls;
#endif

using UnityEditor;
using UnityEngine;

namespace TestingEnvironmentScripts
{
	public class SimpleCameraController : MonoBehaviour
	{
		[Header("Movement Settings")] [Tooltip("Exponential boost factor on translation, controllable by mouse wheel.")]
		public float boost = 3.5f;

		[Tooltip("Time it takes to interpolate camera position 99% of the way to the target.")] [Range(0.001f, 1f)]
		public float positionLerpTime = 0.2f;

		[Header("Rotation Settings")]
		[Tooltip("X = Change in mouse position.\nY = Multiplicative factor for camera rotation.")]
		public AnimationCurve mouseSensitivityCurve =
			new AnimationCurve(new Keyframe(0f, 0.5f, 0f, 5f), new Keyframe(1f, 2.5f, 0f, 0f));

		[Tooltip("Time it takes to interpolate camera rotation 99% of the way to the target.")] [Range(0.001f, 1f)]
		public float rotationLerpTime = 0.01f;

		[Tooltip("Whether or not to invert our Y axis for mouse input to rotation.")]
		public bool invertY;

		private readonly CameraState _mInterpolatingCameraState = new CameraState();

		private readonly CameraState _mTargetCameraState = new CameraState();

		private void Update()
		{
			var translation = Vector3.zero;

#if ENABLE_LEGACY_INPUT_MANAGER

			// Exit Sample  
			if (Input.GetKey(KeyCode.Escape))
			{
				Application.Quit();
#if UNITY_EDITOR
				EditorApplication.isPlaying = false;
#endif
			}

			// Hide and lock cursor when right mouse button pressed
			if (Input.GetMouseButtonDown(1)) Cursor.lockState = CursorLockMode.Locked;

			// Unlock and show cursor when right mouse button released
			if (Input.GetMouseButtonUp(1))
			{
				Cursor.visible = true;
				Cursor.lockState = CursorLockMode.None;
			}

			// Rotation
			if (Input.GetMouseButton(1))
			{
				var mouseMovement =
					new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y") * (invertY ? 1 : -1));

				var mouseSensitivityFactor = mouseSensitivityCurve.Evaluate(mouseMovement.magnitude);

				_mTargetCameraState.yaw += mouseMovement.x * mouseSensitivityFactor;
				_mTargetCameraState.pitch += mouseMovement.y * mouseSensitivityFactor;
			}

			// Translation
			translation = GetInputTranslationDirection() * Time.deltaTime;

			// Speed up movement when shift key held
			if (Input.GetKey(KeyCode.LeftShift)) translation *= 10.0f;

			// Modify movement by a boost factor (defined in Inspector and modified in play mode through the mouse scroll wheel)
			boost += Input.mouseScrollDelta.y * 0.2f;
			translation *= Mathf.Pow(2.0f, boost);

#elif USE_INPUT_SYSTEM
            // TODO: make the new input system work
#endif

			_mTargetCameraState.Translate(translation);

			// Framerate-independent interpolation
			// Calculate the lerp amount, such that we get 99% of the way to our target in the specified time
			var positionLerpPct = 1f - Mathf.Exp(Mathf.Log(1f - 0.99f) / positionLerpTime * Time.deltaTime);
			var rotationLerpPct = 1f - Mathf.Exp(Mathf.Log(1f - 0.99f) / rotationLerpTime * Time.deltaTime);
			_mInterpolatingCameraState.LerpTowards(_mTargetCameraState, positionLerpPct, rotationLerpPct);

			_mInterpolatingCameraState.UpdateTransform(transform);
		}

		private void OnEnable()
		{
			_mTargetCameraState.SetFromTransform(transform);
			_mInterpolatingCameraState.SetFromTransform(transform);
		}

		private Vector3 GetInputTranslationDirection()
		{
			var direction = new Vector3();
			if (Input.GetKey(KeyCode.W)) direction += Vector3.forward;
			if (Input.GetKey(KeyCode.S)) direction += Vector3.back;
			if (Input.GetKey(KeyCode.A)) direction += Vector3.left;
			if (Input.GetKey(KeyCode.D)) direction += Vector3.right;
			if (Input.GetKey(KeyCode.Q)) direction += Vector3.down;
			if (Input.GetKey(KeyCode.E)) direction += Vector3.up;
			return direction;
		}

		private class CameraState
		{
			public float pitch;
			public float roll;
			public float x;
			public float y;
			public float yaw;
			public float z;

			public void SetFromTransform(Transform t)
			{
				pitch = t.eulerAngles.x;
				yaw = t.eulerAngles.y;
				roll = t.eulerAngles.z;
				x = t.position.x;
				y = t.position.y;
				z = t.position.z;
			}

			public void Translate(Vector3 translation)
			{
				var rotatedTranslation = Quaternion.Euler(pitch, yaw, roll) * translation;

				x += rotatedTranslation.x;
				y += rotatedTranslation.y;
				z += rotatedTranslation.z;
			}

			public void LerpTowards(CameraState target, float positionLerpPct, float rotationLerpPct)
			{
				yaw = Mathf.Lerp(yaw, target.yaw, rotationLerpPct);
				pitch = Mathf.Lerp(pitch, target.pitch, rotationLerpPct);
				roll = Mathf.Lerp(roll, target.roll, rotationLerpPct);

				x = Mathf.Lerp(x, target.x, positionLerpPct);
				y = Mathf.Lerp(y, target.y, positionLerpPct);
				z = Mathf.Lerp(z, target.z, positionLerpPct);
			}

			public void UpdateTransform(Transform t)
			{
				t.eulerAngles = new Vector3(pitch, yaw, roll);
				t.position = new Vector3(x, y, z);
			}
		}
	}
}