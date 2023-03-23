using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util {
	public static float GravityPerTick = Physics.gravity.y / 50;

	public static float AbsMax(float value, float max) {
		if (value > 0) return Mathf.Min(value, max);
		else return -Mathf.Min(-value, max);
	}

	//public static Vector3 AdjustForGravity() { // Adjust it so gravity is always "down"

	//}
}
