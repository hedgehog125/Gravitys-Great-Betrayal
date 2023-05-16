using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player {
	public class Visible : MonoBehaviour {
		[SerializeField] private float m_transparentCameraDistance;
		[SerializeField] private List<MeshRenderer> m_renderers;
		[SerializeField] private float rotateSpeed;

		[HideInInspector] public float LookAngle;

		private Camera cam;
		private void Awake() {
			cam = Camera.main;
		}
		private void Start() {
			transform.eulerAngles = Gravity.Rotations[Globals.CurrentGravityController.Direction];
		}

		private void Update() {
			transform.eulerAngles = transform.eulerAngles;
			Debug.Log(transform.eulerAngles);
			return;
			{
				Vector3 targetRotation = Gravity.Rotations[Globals.CurrentGravityController.Direction];
				Vector3 rot = transform.eulerAngles;
				for (int i = 0; i < 3; i++) {
					float adjusted = (rot[i] + 180) % 360;
					float diffToTarget = ((targetRotation[i] + 180) % 360) - adjusted;

					rot[i] = Mathf.Abs(diffToTarget) < rotateSpeed?
						targetRotation[i]
						: rot[i] + (diffToTarget > 0? rotateSpeed : -rotateSpeed)
					;
				}
				transform.eulerAngles = rot;
			}


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