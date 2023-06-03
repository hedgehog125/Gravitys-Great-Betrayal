using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

namespace LevelObject {
	public class ButtonPromptArea : MonoBehaviour {
		[SerializeField] private ButtonPromptData m_prompt;

		private void OnTriggerStay(Collider collider) {
			if (collider.gameObject != Globals.CurrentPlayer.gameObject) return;

			Globals.CurrentUIController.ButtonPrompts.Display(m_prompt);
		}
	}
}
