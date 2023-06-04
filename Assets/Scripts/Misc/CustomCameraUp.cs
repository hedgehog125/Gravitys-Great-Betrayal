// Credit: based on https://forum.unity.com/threads/lookat-center-delay-when-camera-activates.696977/#post-4663325

using UnityEngine;
using Cinemachine;
using System;

/// <summary>
/// An add-on module for Cinemachine Virtual Camera that overrides Up
/// </summary>
[SaveDuringPlay]
[AddComponentMenu("")] // Hide in menu
public class CustomCameraUp : CinemachineExtension {
	[NonSerialized] public Vector3 Up = new(0, 1, 0);

	protected override void PostPipelineStageCallback(
		CinemachineVirtualCameraBase vcam,
		CinemachineCore.Stage stage, ref CameraState state, float deltaTime) {
		if (stage == CinemachineCore.Stage.Body && Up != null) {
			state.ReferenceUp = Up;
		}
	}
}