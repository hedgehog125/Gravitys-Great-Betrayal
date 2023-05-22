using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI {
	public class LevelButton : MonoBehaviour {
		[SerializeField] private string m_levelName;

		public void OnClick() {
			SceneManager.LoadScene(m_levelName);
		}
	}
}
