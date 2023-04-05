using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting;

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class AdjustForDeltaTime : InputProcessor<Vector2> {
	#if UNITY_EDITOR
	static AdjustForDeltaTime() {
		Initialize();
	}
	#endif

	[RuntimeInitializeOnLoadMethod]
	private static void Initialize() {
		InputSystem.RegisterProcessor<AdjustForDeltaTime>();
	}

	public override Vector2 Process(Vector2 unscaledInput, InputControl control) {
		return unscaledInput * (Time.deltaTime * 50);
	}
}