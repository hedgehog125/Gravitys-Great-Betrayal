using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static Cinemachine.AxisState;

namespace Player {
	public class CameraInputProvider : MonoBehaviour, IInputAxisProvider {
		private Vector2 lookInput;
		private void OnLook() {
			lookInput = Globals.CurrentPlayer.LookInput;
		}

		private void Start() {
			Globals.CurrentPlayer.ListenForLookInput(OnLook);
		}

		public virtual float GetAxisValue(int axis) {
			if (axis == 0) return lookInput.x;
			else if (axis == 1) return lookInput.y;
			else return 0;
		}
	}
}
