using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace Player {
	public class CameraController : MonoBehaviour {
		[SerializeField] private CinemachineFreeLook m_defaultVCam;

		private void Start() {
			m_defaultVCam.Follow = Globals.CurrentPlayerObject.transform;
			m_defaultVCam.LookAt = Globals.CurrentPlayerObject.transform;
		}
	}
}
