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
	public static Vector3 MultiplyXZMagnitude(Vector3 vec, float multiplier) {
		Vector2 asVec2 = new(vec.x, vec.z);
		asVec2 = asVec2.normalized * (asVec2.magnitude * multiplier);

		return new(asVec2.x, vec.y, asVec2.y);
	}
	public static Vector3Int RoundVector(Vector3 vec) {
		return new(Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y), Mathf.RoundToInt(vec.z));
	}
	public static float GetHorizontalTurnAmount(Vector3 adjustedVel, Vector3 moveDirection) {
		Vector3 withoutY = adjustedVel;
		withoutY.y = 0;
		withoutY = withoutY.normalized;

		return Vector3.Angle(withoutY, moveDirection);
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

	public class GroundDetector {
		private const float OFFSET = 0.025f;
		private const float HIT_DISTANCE = 0.05f;

		private Vector3 raycastOriginOffset;
		private LayerMask groundLayers;
		private float maxDistance;

		public GroundDetector(float height, LayerMask groundLayers, float maxDistance = HIT_DISTANCE) {
			raycastOriginOffset = new(0, -(height / 2) + OFFSET, 0);
			this.groundLayers = groundLayers;
			this.maxDistance = maxDistance;
		}

		public bool Check(Vector3 pos, out float distance, bool inPlayerGravity = true) {
			bool didHit = Physics.Raycast(pos + Globals.CurrentGravityController.Apply(raycastOriginOffset, inPlayerGravity), inPlayerGravity? Globals.CurrentGravityController.Down : Vector3.down, out RaycastHit hit, maxDistance, groundLayers);
			distance = didHit? Mathf.Max(hit.distance - OFFSET, 0) : Mathf.Infinity;

			return didHit && hit.distance < HIT_DISTANCE;
		}
		public bool Check(Vector3 pos, bool inPlayerGravity = true) {
			return Check(pos, out float _, inPlayerGravity);
		}
	}
}
