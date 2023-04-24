using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace Player {
	public class CameraController : MonoBehaviour {
		[SerializeField] private CinemachineFreeLook m_defaultVCam;
		[SerializeField] private CustomCameraUp m_cameraUpController;

		private void Awake() {
			Cursor.lockState = CursorLockMode.Locked;

			m_cameraUpController.Up = new Vector3(0, -1, 0);
		}
		private void Start() {
			m_defaultVCam.Follow = Globals.CurrentPlayer.transform;
			m_defaultVCam.LookAt = Globals.CurrentPlayer.transform;
		}
	}
}
