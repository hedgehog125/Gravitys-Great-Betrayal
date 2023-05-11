using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalsController : MonoBehaviour {
	[SerializeField] private ConstantData m_constants;

	private void Awake() {
		if (Globals.CurrentController != null) {
			Destroy(gameObject);
			return;
		}

		Globals.CurrentController = this;
		Globals.CurrentConstants = m_constants;
		DontDestroyOnLoad(gameObject);
	}
}
