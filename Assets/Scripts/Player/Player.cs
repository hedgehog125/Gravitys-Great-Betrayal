using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Player {
	public class Player : MonoBehaviour {
		private static readonly Dictionary<string, int> INPUT_SCHEME_IDS = new() {
			{ "Keyboard", 0 },
			{ "Controller", 1 },
			{ "Xbox", 2 },
			{ "PS5", 3 },
			{ "Switch", 4 }
		};

		[SerializeField] private Visible m_visibleController;

		private int lastCheckpointID = -1;
		private string lastControlScheme;
		private readonly UnityEvent lookInputEvent = new();
		private readonly UnityEvent pauseInputEvent = new();
		private readonly UnityEvent mainMenuInputEvent = new();
		private readonly UnityEvent controlChangeEvent = new();

		[HideInInspector] public Visible VisibleController { get => m_visibleController; }
		[HideInInspector] public Movement MoveController { get; private set; }
		[HideInInspector] public Health HealthController { get; private set; }
		[HideInInspector] public int ControlSchemeID { get; private set; }


		// Note that these are independent. Scaled is from inputs that are already scaled like mice and unscaled is for controllers
		[HideInInspector] public Vector2 ScaledLookInput { get; private set; }
		[HideInInspector] public Vector2 UnscaledLookInput { get; private set; }
		private void OnScaledLook(InputValue input) {
			Vector2 unscaledBySensitivity = input.Get<Vector2>();

			ScaledLookInput = unscaledBySensitivity.normalized * (unscaledBySensitivity.magnitude * Globals.CurrentConfig.mouseSensitivity);
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
		[HideInInspector] public bool MainMenuInput { get; private set; }
		private void OnMainMenuInput(InputValue input) {
			MainMenuInput = input.isPressed;
			mainMenuInputEvent.Invoke();
		}

		private void OnDeath() {
			MoveController.ResetFacingDirection();
			Globals.CurrentGravityController.ChangeDirection(0);
		}


		private PlayerInput playerInput;
		private void Awake() {
			HealthController = GetComponent<Health>();
			MoveController = GetComponent<Movement>();
			playerInput = GetComponent<PlayerInput>();
			Globals.CurrentPlayer = this;

			HealthController.ListenForDeath(OnDeath);
			ControlSchemeChanged();			
		}

		private void FixedUpdate() {
			if (playerInput.currentControlScheme != lastControlScheme) {
				ControlSchemeChanged();
			}
		}

		private void ControlSchemeChanged() {
			string was = lastControlScheme;
			lastControlScheme = playerInput.currentControlScheme;

			ControlSchemeID = INPUT_SCHEME_IDS[playerInput.currentControlScheme];

			if (was != null) controlChangeEvent.Invoke(); // Don't invoke for the init call
		}

		public void ListenForLookInput(UnityAction callback) {
			lookInputEvent.AddListener(callback);
		}
		public void ListenForPauseInput(UnityAction callback) {
			pauseInputEvent.AddListener(callback);
		}
		public void ListenForMainMenuInput(UnityAction callback) {
			mainMenuInputEvent.AddListener(callback);
		}
		public void ListenForControlsChange(UnityAction callback) {
			controlChangeEvent.AddListener(callback);
		}

		public void HandleCheckpoint(int id, Vector3 position) {
			if (! (id > lastCheckpointID || id == -1)) return;

			lastCheckpointID = id;
			HealthController.RespawnLocation = position;
		}
	}
}
