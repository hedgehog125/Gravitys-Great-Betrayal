using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static Cinemachine.AxisState;

namespace Player {
	public class CameraInputProvider : MonoBehaviour, IInputAxisProvider {
		private Vector2 scaledLookInput;
		private Vector2 unscaledLookInput;
		private void OnLook() {
			scaledLookInput = Globals.CurrentPlayer.ScaledLookInput;
			unscaledLookInput = Globals.CurrentPlayer.UnscaledLookInput;
		}

		private void Start() {
			Globals.CurrentPlayer.ListenForLookInput(OnLook);
		}

		public virtual float GetAxisValue(int axis) {
			Vector2 input = scaledLookInput + (unscaledLookInput * (Time.deltaTime * 50));

			if (axis == 0) return input.x;
			else if (axis == 1) return input.y;
			else return 0;
		}
	}
}
