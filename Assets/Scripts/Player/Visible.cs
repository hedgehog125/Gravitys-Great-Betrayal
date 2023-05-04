using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player {
	public class Visible : MonoBehaviour {
		[SerializeField] private float m_transparentCameraDistance;
		[SerializeField] private List<MeshRenderer> m_renderers;

		[HideInInspector] public float LookAngle;

		private Camera cam;
		private void Awake() {
			cam = Camera.main;
		}

		private void Update() {
			transform.eulerAngles = Gravity.Rotations[Globals.CurrentGravityController.Direction];
			m_renderers[0].transform.localEulerAngles = new Vector3(0, LookAngle, 0);

			if (Vector3.Distance(transform.position, cam.transform.position) < m_transparentCameraDistance) {
				m_renderers.ForEach(ren => ren.material.SetFloat("Alpha", 0.25f));
			}
			else {
				m_renderers.ForEach(ren => ren.material.SetFloat("Alpha", 1f));
			}
		}
	}
}
