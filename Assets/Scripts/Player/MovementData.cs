using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player {
	[CreateAssetMenu]
	public class MovementData : ScriptableObject {
		[Header("Running")]
		public float acceleration;
		public float maxSpeed;

		[Header("Jumping")]
		public float jumpPower;
		public LayerMask jumpLayer;
		public float nearGroundDistance;

		public int maxJumpBufferTime;
		public float maxJumpBufferVelocity; // Double jump if the player's upwards velocity is equal or more than this

		public int maxMidairJumps;
		public float midairJumpPower;

		public int maxJumpHoldTime;
		public float jumpHoldCurveSteepness;

		public float maxJumpSpeedIncrease;
		public float speedForMaxJumpSpeedIncrease; // The speed needed to get the maximum increase due to speed

		[Header("Misc")]
		public float topDownThreshold;
	}
}
