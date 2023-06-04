using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
		if (GetComponent<Renderer>() == null) {
			for (int i = 0; i < transform.childCount; i++) {
				Transform child = transform.GetChild(i);
				if (child.GetComponent<StaticGameObject>() != null) continue;
				if (child.TryGetComponent<Rigidbody>(out var childRb)) {
					if (! childRb.isKinematic) continue;
				}

				child.AddComponent<StaticGameObject>();
			}

			DestroyImmediate(this);
			return;
		}
	}

	private void Update() {
		if (EditorApplication.isPlayingOrWillChangePlaymode) return;

		Renderer ren = GetComponent<Renderer>();
		ren.shadowCastingMode = m_bakedShadows? UnityEngine.Rendering.ShadowCastingMode.On : UnityEngine.Rendering.ShadowCastingMode.Off;
	}
	#endif
}
