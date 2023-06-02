using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
	public class MouseSensitivity : MonoBehaviour {
		[SerializeField] private Slider m_slider;
		[SerializeField] private float m_min;
		[SerializeField] private float m_max;
		[SerializeField] private int m_saveAfter = 15;

		private int saveTick;
		private float savedValue;
		private float inputValueWas;

		private void Start() {
			savedValue = Globals.CurrentConfig.mouseSensitivity;
			m_slider.value = (savedValue - m_min) / (m_max - m_min);
		}
		private void FixedUpdate() {
			float inputValue = (m_slider.value * (m_max - m_min)) + m_min;
			Globals.CurrentConfig.mouseSensitivity = inputValue;

			if (Mathf.Approximately(inputValue, savedValue)) {
				saveTick = 0;
			}
			else {
				if (saveTick == m_saveAfter) {
					PlayerConfig.Save(Globals.CurrentConfig);

					saveTick = 0;
				}
				else {
					saveTick++;
				}
			}

			if (! Mathf.Approximately(inputValue, inputValueWas)) { // Wait until it stops changing
				saveTick = 0;
			}
			inputValueWas = inputValue;
		}
	}
}
