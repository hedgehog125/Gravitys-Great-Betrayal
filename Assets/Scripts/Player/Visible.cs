using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player {
	public class Visible : MonoBehaviour {
		[SerializeField] private float m_transparentCameraDistance;

		private MeshRenderer ren;
		private Camera cam;
		private void Awake() {
			ren = GetComponent<MeshRenderer>();
			cam = Camera.main;
		}

		private void Update() {
			transform.eulerAngles = Gravity.Rotations[Globals.CurrentGravityController.Direction];

			if (Vector3.Distance(transform.position, cam.transform.position) < m_transparentCameraDistance) {
				ren.material.SetFloat("Alpha", 0.25f);
			}
			else {
				ren.material.SetFloat("Alpha", 1);
			}
		}
	}
}
