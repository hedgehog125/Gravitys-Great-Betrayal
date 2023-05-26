using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PauseController : MonoBehaviour {
	[SerializeField] private bool m_initiallyAllowPauseInput = true;

	private Player.Player player;
	private readonly UnityEvent pauseChangeEvent = new();
	private float originalTimeScale;

	[HideInInspector] public bool IsPaused { get; private set; }
	[HideInInspector] public bool AllowPauseInput { get; private set; } = true;

	private void OnPause() {
		if (! player.PauseInput) return;
		if (! AllowPauseInput) return;

		TogglePause();
	}
	private void PauseStateChanged() {
		pauseChangeEvent.Invoke();

		if (IsPaused) {
			originalTimeScale = Time.timeScale;
			Time.timeScale = 0f;
		}
		else {
			Time.timeScale = originalTimeScale;
		}
	}

	private void Awake() {
		Globals.CurrentPauseController = this;
		AllowPauseInput = m_initiallyAllowPauseInput;
	}

	private void Start() {
		player = Globals.CurrentPlayer;
		player.ListenForPauseInput(OnPause);
	}

	public void TogglePause() {
		IsPaused = ! IsPaused;
		PauseStateChanged();
	}

	public void Listen(UnityAction callback) {
		pauseChangeEvent.AddListener(callback);
	}
}
