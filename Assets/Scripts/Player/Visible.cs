using Array = System.Array;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player {
	public class Visible : MonoBehaviour {
		[SerializeField] private float m_transparentCameraDistance;
		[SerializeField] private Transform m_modelRoot;
		[SerializeField] private float rotateTime; // In seconds

		[HideInInspector] public float LookAngle;

		private int directionWas;
		private float currentRotateRatio;
		private float rotateRatioPerSecond;
		private Quaternion rotateStartRotation;

		private Camera cam;
		private MeshRenderer[] renderers;
		private void Awake() {
			cam = Camera.main;
			renderers = GetComponentsInChildren<MeshRenderer>();
		}
		private void Start() {
			directionWas = Globals.CurrentGravityController.Direction;
			transform.eulerAngles = Gravity.Rotations[directionWas];
			rotateStartRotation = transform.rotation;

			rotateRatioPerSecond = 1 / rotateTime;
		}

		private void Update() {
			int targetDirectionID = Globals.CurrentGravityController.Direction;
			Vector3 targetRotation = Gravity.Rotations[targetDirectionID];
			if (targetDirectionID == directionWas) {
				transform.eulerAngles = targetRotation;
			}
			else {
				Quaternion targetAsQuaterion = Quaternion.Euler(targetRotation);

				currentRotateRatio += rotateRatioPerSecond * Time.deltaTime;
				if (currentRotateRatio >= 1f) {
					transform.rotation = targetAsQuaterion;

					directionWas = targetDirectionID;
					currentRotateRatio = 0;
					rotateStartRotation = targetAsQuaterion;
				}
				else {
					transform.rotation = Quaternion.Lerp(rotateStartRotation, targetAsQuaterion, currentRotateRatio);
				}
			}
			m_modelRoot.localEulerAngles = new Vector3(0, LookAngle, 0);

			if (Vector3.Distance(transform.position, cam.transform.position) < m_transparentCameraDistance) {
				Array.ForEach(renderers, ren => ren.material.SetFloat("Alpha", 0.25f));
			}
			else {
				Array.ForEach(renderers, ren => ren.material.SetFloat("Alpha", 1f));
			}
		}
	}
}