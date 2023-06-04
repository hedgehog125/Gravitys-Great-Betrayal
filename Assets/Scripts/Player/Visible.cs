using Array = System.Array;
using String = System.String;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;

namespace Player {
	public class Visible : MonoBehaviour {
		private static readonly Vector2[] RELATIVE_GRAVITY_CAM_ROTATIONS = {
			// +X is to the right on the screen, +Y is rotating to look upwards
			new(-90, 0), // Left
			new(90, 0), // Right
			new(0, -90), // Forwards
			new(0, 90), // Backwards
			new(180, 0) // Up, rotate to the right
		};

		[SerializeField] private float m_transparentCameraDistance;
		[SerializeField] private Transform m_modelRoot;
		[SerializeField] private float rotateTime; // In seconds

		[NonSerialized] public float LookAngle;

		private int directionWas;
		private float currentRotateRatio;
		private float rotateRatioPerSecond;
		private Quaternion rotateStartRotation;
		private int relativeCamRotateDir = -1;
		private Vector3 targetCamRotateAmount;

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
			Vector3 targetPresetRotation = Gravity.Rotations[targetDirectionID];
			if (targetDirectionID == directionWas) {
				transform.eulerAngles = targetPresetRotation;
			}
			else {
				Quaternion targetAsQuaterion = Quaternion.Euler(targetPresetRotation);

				currentRotateRatio += rotateRatioPerSecond * Time.deltaTime;
				if (currentRotateRatio >= 1f) {
					if (relativeCamRotateDir != 5) {
						Vector3 snappedRotationChange = targetCamRotateAmount;
						int previousXAxisID = Util.GetAbsLargestAxis(Gravity.ApplyToDirection(Vector3.forward, directionWas));

						if (Globals.CurrentGravityController != null) {
							Globals.CurrentCameraController.RotateBy(
								Mathf.DeltaAngle(targetPresetRotation[previousXAxisID], snappedRotationChange[previousXAxisID]),
								0f
							);
						}
					}
					transform.rotation = targetAsQuaterion;

					directionWas = targetDirectionID;
					currentRotateRatio = 0;
					rotateStartRotation = targetAsQuaterion;
					relativeCamRotateDir = -1;
				}
				else {
					if (relativeCamRotateDir == -1) {
						if (Globals.CurrentCameraController == null) {
							relativeCamRotateDir = 5; // Always use the fallback
						}
						else {
							Vector3[] cameraDirections = {
								LastDirRemoveY(-cam.transform.right),
								LastDirRemoveY(cam.transform.right),
								LastDirRemoveY(cam.transform.forward),
								LastDirRemoveY(-cam.transform.forward),
								cam.transform.up
							};

							relativeCamRotateDir = 0;
							while (relativeCamRotateDir < cameraDirections.Length) {
								Vector3Int direction = Util.GetAbsLargestAxisAsVec(cameraDirections[relativeCamRotateDir]);

								if (Gravity.DirectionToID(direction) == targetDirectionID) {
									break;
								}
								relativeCamRotateDir++;
							}
						}
						

						if (relativeCamRotateDir != 5) {
							Vector2Int rotateCameraAxes = new(
								Util.GetAbsLargestAxis(cam.transform.forward, true),
								Util.GetAbsLargestAxis(cam.transform.right, true)
							);

							Vector2 relativeRotation = RELATIVE_GRAVITY_CAM_ROTATIONS[relativeCamRotateDir];

							int xAxisID = Mathf.Abs(rotateCameraAxes.x) - 1;
							int xAxisSign = (int)Mathf.Sign(rotateCameraAxes.x);
							int yAxisID = Mathf.Abs(rotateCameraAxes.y) - 1;
							int yAxisSign = (int)Mathf.Sign(rotateCameraAxes.y);

							targetCamRotateAmount = new(0, 0, 0);
							targetCamRotateAmount[xAxisID] = relativeRotation.x * xAxisSign;
							targetCamRotateAmount[yAxisID] = relativeRotation.y * yAxisSign;
							if (directionWas == 1) { // Upside down
								targetCamRotateAmount[xAxisID] *= -1;
								targetCamRotateAmount[yAxisID] *= -1;
							}
						}
					}

					if (relativeCamRotateDir == 5) { // Failsafe
						transform.rotation = Quaternion.Lerp(rotateStartRotation, targetAsQuaterion, currentRotateRatio);
					}
					else {
						transform.rotation = rotateStartRotation;
						transform.eulerAngles += targetCamRotateAmount * currentRotateRatio;
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