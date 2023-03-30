using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class Util {
	public static float GravityPerTick = Physics.gravity.y / 50;

	public static float AbsMax(float value, float max) {
		if (value > 0) return Mathf.Min(value, max);
		else return -Mathf.Min(-value, max);
	}
	public static Vector3 LimitXZ(Vector3 vec, float limit) {
		Vector2 vec2 = new(vec.x, vec.z);
		if (vec2.magnitude > limit) vec2 = vec2.normalized * limit;

		return new Vector3(vec2.x, vec.y, vec2.y);
	}

	public static Vector2 RotateAroundOrigin(Vector2 point, float radians) {
		float sin = Mathf.Sin(-radians);
		float cos = Mathf.Cos(-radians);

		Vector2 newPoint = new(
			(point.x * cos) - (point.y * sin),
			(point.y * cos) + (point.x * sin)
		);
		return newPoint;
	}
}
