using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI {
	public class UI : MonoBehaviour {
		[SerializeField] private HealthBar m_healthBar;

		private Player.Player player;
		private void Start() {
			player = Globals.CurrentPlayer;

			m_healthBar.Init(player.HealthController.MaxHealth);
			UpdateUI();
		}

		private void FixedUpdate() {
			UpdateUI();
		}

		private void UpdateUI() {
			m_healthBar.SetHearts(player.HealthController.CurrentHealth);
		}
	}
}
