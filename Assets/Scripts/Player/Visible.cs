using Array = System.Array;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Player {
	public class Visible : MonoBehaviour {
		private static readonly Vector2[] RELATIVE_GRAVITY_CAM_ROTATIONS = {
			// +X is right relative to the camera, snapped to the closest axis
			new(90, 0), // Left
			new(-90, 0), // Right
			new(-90, 0), // Forwards
			new(90, 0), // Backwards
			new(180, 0) // Up, rotate to the right
		};

		[SerializeField] private float m_transparentCameraDistance;
		[SerializeField] private Transform m_modelRoot;
		[SerializeField] private float rotateTime; // In seconds

		[HideInInspector] public float LookAngle;

		private int directionWas;
		private float currentRotateRatio;
		private float rotateRatioPerSecond;
		private Quaternion rotateStartRotation;
		private int relativeRotateDirection = -1;
		private int rotationCamForwardAxis; // The axis that was moved the camera forwards and backwards before gravity was switched

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
			Gravity gravityController = Globals.CurrentGravityController;

			int targetDirectionID = gravityController.Direction;
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
					relativeRotateDirection = -1;
				}
				else {
					if (relativeRotateDirection == -1) {
						Vector3[] cameraDirections = {
							LastDirRemoveY(-cam.transform.right),
							LastDirRemoveY(cam.transform.right),
							LastDirRemoveY(cam.transform.forward),
							LastDirRemoveY(-cam.transform.forward),
							cam.transform.up
						};

						relativeRotateDirection = 0;
						while (relativeRotateDirection < cameraDirections.Length) {
							Vector3Int direction = Util.GetAbsLargestAxisAsVec(cameraDirections[relativeRotateDirection]);

							if (Gravity.DirectionToID(direction) == targetDirectionID) {
								break;
							}
							relativeRotateDirection++;
						}

						rotationCamForwardAxis = Util.GetAbsLargestAxis(cam.transform.forward);
					}

					if (relativeRotateDirection == 5) { // Failsafe
						transform.rotation = Quaternion.Lerp(rotateStartRotation, targetAsQuaterion, currentRotateRatio);
					}
					else {
						Vector3 targetRotateAmount = new(0, 0, 0);
						//targetRotateAmount[0] = ;
						//RELATIVE_GRAVITY_CAM_ROTATIONS[relativeRotateDirection];


						transform.rotation = rotateStartRotation;
						transform.Rotate(targetRotateAmount * currentRotateRatio);
					}
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

		private Vector3 LastDirRemoveY(Vector3 vec) {
			return Util.RemoveAdjustedY(vec, directionWas);
		}
	}
}