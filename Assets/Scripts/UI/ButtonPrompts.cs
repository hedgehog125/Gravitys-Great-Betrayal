using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI {
	public class ButtonPrompts : MonoBehaviour {
		[SerializeField] private int m_showTime;

		private int showTick = -1;
		private ButtonPromptData currentPrompt;
		private bool needsRerender;

		private void OnControlSchemeChange() {
			Render();
		}

		private Animator anim;
		private TextMeshProUGUI tex;
		private void Awake() {
			anim = GetComponent<Animator>();
			tex = GetComponent<TextMeshProUGUI>();
		}
		private void Start() {
			Globals.CurrentPlayer.ListenForControlsChange(OnControlSchemeChange);
		}

		private void FixedUpdate() {
			if (showTick != -1) {
				if (needsRerender) {
					Render();
					needsRerender = false;
				}

				if (showTick == m_showTime) {
					anim.SetBool("Show", false);
					showTick = -1;
					currentPrompt = null;
				}
				else {
					showTick++;
				}
			}
		}

		private void Render() {
			if (showTick == -1) return;

			int controlSchemeID = Globals.CurrentPlayer.ControlSchemeID;
			Debug.Log(controlSchemeID);
			string displayText = "";
			for (int i = 0; i < currentPrompt.textParts.Count; i++) {
				displayText += currentPrompt.textParts[i];
				if (i != currentPrompt.textParts.Count - 1) {
					int iconID = (controlSchemeID * (currentPrompt.textParts.Count - 1)) + i;
					displayText += $" <sprite={currentPrompt.iconIDs[iconID]}>";

					if (currentPrompt.textParts[i + 1] != "" || i != currentPrompt.textParts.Count - 2) displayText += " ";
				}
			}
			tex.text = displayText;

			anim.SetBool("Show", true);
		}

		public void Display(ButtonPromptData prompt) {
			showTick = 0;
			if (currentPrompt != prompt) {
				needsRerender = true;
				currentPrompt = prompt;
			}
		}
	}
}
