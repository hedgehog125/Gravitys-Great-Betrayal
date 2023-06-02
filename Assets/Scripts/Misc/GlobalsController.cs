using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalsController : MonoBehaviour {
	[SerializeField] private ConstantData m_constants;

	private void Awake() {
		if (Globals.CurrentController != null) {
			Globals.CurrentConstants = m_constants;

			Destroy(gameObject);
			return;
		}

		Globals.CurrentController = this;
		Globals.CurrentConstants = m_constants;

		PlayerConfig config = PlayerConfig.Load();
		if (config == null) {
			config = new PlayerConfig();
			PlayerConfig.Save(config);
		}
		Globals.CurrentConfig = config;

		DontDestroyOnLoad(gameObject);
	}
}
