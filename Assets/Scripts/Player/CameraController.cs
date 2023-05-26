using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace Player {
	public class CameraController : MonoBehaviour {
		[SerializeField] private CinemachineFreeLook m_defaultVCam;
		[SerializeField] private CustomCameraUp m_cameraUpController;

		private void Start() {
			Transform target = Globals.CurrentPlayer.VisibleController.transform;
			m_defaultVCam.Follow = target;
			m_defaultVCam.LookAt = target;
		}

		private void Update() {
			m_cameraUpController.Up = Globals.CurrentPlayer.VisibleController.transform.up;
		}
	}
}
