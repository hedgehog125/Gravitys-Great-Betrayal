using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Player {
	public class Player : MonoBehaviour {
		[SerializeField] private Visible m_visibleController;

		private Movement moveController;
		private readonly UnityEvent lookInputEvent = new();
		private readonly UnityEvent pauseInputEvent = new();

		[HideInInspector] public Visible VisibleController { get => m_visibleController; }
		[HideInInspector] public Health HealthController { get; private set; }

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

		[HideInInspector] public bool PauseInput { get; private set; }
		private void OnPause(InputValue input) {
			PauseInput = input.isPressed;
			pauseInputEvent.Invoke();
		}

		private void OnDeath() {
			moveController.ResetFacingDirection();
			Globals.CurrentGravityController.ChangeDirection(0);
		}

		private void Awake() {
			HealthController = GetComponent<Health>();
			moveController = GetComponent<Movement>();
			Globals.CurrentPlayer = this;

			HealthController.ListenForDeath(OnDeath);
		}

		public void ListenForLookInput(UnityAction callback) {
			lookInputEvent.AddListener(callback);
		}
		public void ListenForPauseInput(UnityAction callback) {
			pauseInputEvent.AddListener(callback);
		}
	}
}
