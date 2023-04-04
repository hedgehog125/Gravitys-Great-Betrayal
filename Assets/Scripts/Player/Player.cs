using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Player {
	public class Player : MonoBehaviour {
		private readonly UnityEvent lookInputEvent = new();

		[HideInInspector] public Vector2 LookInput { get; private set; }
		private void OnLook(InputValue input) {
			LookInput = input.Get<Vector2>();
			Debug.Log(LookInput);
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
