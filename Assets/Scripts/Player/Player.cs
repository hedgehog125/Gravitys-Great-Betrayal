using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Player {
	public class Player : MonoBehaviour {
		private readonly UnityEvent lookInputEvent = new();

		// Note that these are independent. Scaled is from inputs that are already scaled like mice and unscaled is for controllers
		[HideInInspector] public Vector2 ScaledLookInput { get; private set; }
		[HideInInspector] public Vector2 UnscaledLookInput { get; private set; }
		private void OnScaledLook(InputValue input) {
			ScaledLookInput = input.Get<Vector2>();
			lookInputEvent.Invoke();
		}
		private void OnUnscaledLook(InputValue input) {
			UnscaledLookInput = input.Get<Vector2>();
			lookInputEvent.Invoke();
		}

		private void Awake() {
			Globals.CurrentPlayer = this;
		}

		public void ListenForLookInput(UnityAction callback) {
			lookInputEvent.AddListener(callback);
		}
	}
}
