using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI {
	public class ButtonPrompts : MonoBehaviour {
		[SerializeField] private int m_showTime;
		[SerializeField] private int m_animationDuration;

		private int showTick = -1;
		private int hideAnimationTick = -1;
		private ButtonPromptData currentPrompt;
		private ButtonPromptData queuedPrompt;
		private bool needsRerender;
		private bool isChangingPrompt;

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
			if (showTick == -1) {
				if (hideAnimationTick == m_animationDuration) {
					hideAnimationTick = -1;
				}
				else {
					hideAnimationTick++;
				}

				if (hideAnimationTick == -1) {
					isChangingPrompt = false;

					currentPrompt = queuedPrompt;
					queuedPrompt = null;
				}
			}
			else {
				if (!isChangingPrompt) {
					if (needsRerender) {
						Render();
						needsRerender = false;
					}
				}

				if (showTick == m_showTime) {
					anim.SetBool("Show", false);
					showTick = -1;
					currentPrompt = null;

					hideAnimationTick = 0;
				}
				else {
					showTick++;
				}
			}
		}

		private void Render() {
			if (showTick == -1) return;

			int controlSchemeID = Globals.CurrentPlayer.ControlSchemeID;
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
			if (! isChangingPrompt) showTick = 0;
			if (currentPrompt != prompt && queuedPrompt != prompt) {
				if (currentPrompt == null) {
					currentPrompt = prompt;
				}
				else {
					showTick = m_showTime;
					isChangingPrompt = true;

					queuedPrompt = prompt;
				}
				needsRerender = true;
			}
		}
	}
}
