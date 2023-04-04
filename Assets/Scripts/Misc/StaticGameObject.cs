using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class StaticGameObject : MonoBehaviour {
	[SerializeField] private bool m_bakedShadows = true;
	[SerializeField] private bool m_realtimeShadows = false;

	private void Awake() { // Only runs in play mode
		#if UNITY_EDITOR
		if (! EditorApplication.isPlayingOrWillChangePlaymode) return;
		#endif

		Renderer ren = GetComponent<Renderer>();
		ren.shadowCastingMode = m_realtimeShadows? UnityEngine.Rendering.ShadowCastingMode.On : UnityEngine.Rendering.ShadowCastingMode.Off;
	}

	#if UNITY_EDITOR
	private void Reset() {
		gameObject.isStatic = true;
	}

	private void Update() {
		if (EditorApplication.isPlayingOrWillChangePlaymode) return;

		Renderer ren = GetComponent<Renderer>();
		ren.shadowCastingMode = m_bakedShadows? UnityEngine.Rendering.ShadowCastingMode.On : UnityEngine.Rendering.ShadowCastingMode.Off;
	}
	#endif
}
