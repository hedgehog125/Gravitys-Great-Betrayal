using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player {
	[CreateAssetMenu(menuName = "Data/Player/Movement")]
	public class MovementData : ScriptableObject {
		[Header("Running")]
		public float acceleration;
		public float airAcceleration;
		public float maxSpeed;

		[Header("Jumping")]
		public float jumpPower;
		public float nearGroundDistance; // When the player is close to the ground, jumps are buffered instead of triggering double jumps
		public int coyoteTime;

		public int maxJumpBufferTime;
		public float maxJumpBufferVelocity; // Double jump if the player's upwards velocity is equal or more than this

		public int maxMidairJumps;
		public float midairJumpPower;
		public float midairJumpYVelReduction;

		public int maxJumpHoldTime;
		public float jumpHoldCurveSteepness;

		public float maxJumpSpeedIncrease;
		public float speedForMaxJumpSpeedIncrease; // The speed needed to get the maximum increase due to speed

		[Header("Gravity Switching")]
		public int gravitySwitchCooldown;
		public int maxMidairGravitySwitches;
		public int gravitySwitchFloatTime;

		[Header("Momentum")]
		public float neutralSpeedMultiplier;
		public float airNeutralSpeedMultiplier;

		public float turnMinChange;
		public float turnAccelerationMultiplier; // The multiplier for the existing speed when turning while on the ground
		public float airTurnAccelerationMultiplier;

		public float doubleJumpSubtractiveMinTurn;
		// ^ The minimum difference between in degrees between the inputted direction and the player's momentum to get the subtractive amount
		public float doubleJumpAdditiveMultiplier; // When the control stick is roughly in the same direction as your momentum
		public float doubleJumpSubtractiveMultiplier; // Neutral or holding back

		[Header("Misc")]
		public float topDownThreshold;
	}
}
