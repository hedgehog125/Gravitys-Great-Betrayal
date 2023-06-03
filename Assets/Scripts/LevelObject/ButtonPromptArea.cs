using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

namespace LevelObject {
	public class ButtonPromptArea : MonoBehaviour {
		[SerializeField] private ButtonPromptData m_prompt;
		[SerializeField] private int m_delay;

		private bool touchingPlayer;
		private int activateTick;

		private void OnTriggerStay(Collider collider) {
			if (collider.gameObject != Globals.CurrentPlayer.gameObject) return;

			touchingPlayer = true;
		}

		private void FixedUpdate() {
			if (touchingPlayer) {
				if (activateTick == m_delay) {
					Globals.CurrentUIController.ButtonPrompts.Display(m_prompt);
				}
				else {
					activateTick++;
				}
			}
			else {
				activateTick = 0;
			}

			touchingPlayer = false;
		}
	}
}
