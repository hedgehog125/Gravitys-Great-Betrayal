using PhysicsTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player { 
	public class Movement : MonoBehaviour {
		private static Vector3 ADJUSTED_FORWARD = new(0, 0, 1);


		[SerializeField] private MovementData m_moveData;


		private Vector2 moveInput;
		private void OnMove(InputValue input) {
			moveInput = input.Get<Vector2>();
		}
		private bool jumpInput;
		private void OnJump(InputValue input) {
			jumpInput = input.isPressed;
			bufferedJumpInput = jumpInput;
			if (jumpInput && jumpBufferHoldTick != -1) jumpBufferHoldTick = 0; // Reset the hold time when a jump is buffered again in midair
		}
		private bool switchGravityInput;
		private void OnSwitchGravity(InputValue input) {
			switchGravityInput = input.isPressed;
		}

		private Util.GroundDetector groundDetector;
		private int jumpBufferTick = -1;
		private int jumpBufferHoldTick = -1; // Records how long jump was held in midair
		private bool bufferedJumpInput; // Like jumpInput but it's not set to false when a jump is first buffered or when a jump is started

		private int jumpHoldTick = -1;
		private int midairJumpCount;
		private int coyoteTick = -1;


		private Rigidbody rb;
		private FrictionEffector frictionEffector;
		private Camera cam;
		private void Awake() {
			rb = GetComponent<Rigidbody>();
			frictionEffector = GetComponent<FrictionEffector>();
			cam = Camera.main;

			Collider col = GetComponent<Collider>();
			groundDetector = new(col.bounds.size.y, m_moveData.jumpLayer, m_moveData.nearGroundDistance);
		}

		private void FixedUpdate() {
			Vector3 vel = Globals.CurrentGravityController.Adjust(rb.velocity);
			bool onGround = groundDetector.Check(transform.position, out float groundDistance);
			bool nearGround = groundDistance < m_moveData.nearGroundDistance;
			

			MoveTick(ref vel);
			JumpTick(ref vel, onGround, nearGround);
			CapVelocity(ref vel);

			GravityTick(ref vel);
			rb.velocity = Globals.CurrentGravityController.Apply(vel);

			frictionEffector.Tick(onGround);
		}


		private void MoveTick(ref Vector3 vel) {
			float moveAmount = Mathf.Sqrt(Mathf.Pow(moveInput.x, 2) + Mathf.Pow(moveInput.y, 2));

			Vector3 moveDirection;
			{
				Vector3 forward = cam.transform.forward;
				forward = Globals.CurrentGravityController.Adjust(forward);
				forward.y = 0;

				if (forward.magnitude < m_moveData.topDownThreshold) { // Somewhat of a niche situation, just use the use the gravity remappings
					moveDirection = ADJUSTED_FORWARD;
				}
				else {
					moveDirection = forward.normalized;
				}
			}

			float inputAngle = Mathf.Atan2(moveInput.x, moveInput.y); // In radians
			{
				Vector2 asVec2 = Util.RotateAroundOrigin(new Vector2(moveDirection.x, moveDirection.z), inputAngle);
				moveDirection = new Vector3(asVec2.x, 0, asVec2.y);
			}

			vel += moveDirection * (moveAmount * m_moveData.acceleration);
		}
		private void JumpTick(ref Vector3 vel, bool onGround, bool nearGround) {
			if (onGround) {
				midairJumpCount = 0;
				coyoteTick = 0;
			}
			else {
				if (coyoteTick != -1) {
					if (coyoteTick == m_moveData.coyoteTime) {
						coyoteTick = -1;
					}
					else {
						coyoteTick++;
					}
				}
			}

			bool jumpBuffered = false;
			if (jumpBufferTick != -1) {
				if (jumpBufferTick == m_moveData.maxJumpBufferTime) {
					jumpBufferTick = -1;
					jumpBufferHoldTick = -1;
				}
				else {
					jumpBuffered = true;
					jumpBufferTick++;
				}
			}

			if (jumpBufferHoldTick != 0 && bufferedJumpInput) {
				jumpBufferHoldTick++;
			}

			if (jumpHoldTick == -1) {
				if (jumpInput || jumpBuffered) {
					bool canMidairJump = midairJumpCount < m_moveData.maxMidairJumps;

					if (onGround || coyoteTick != -1) {
						vel.y = 0;
						jumpHoldTick = 0;
						jumpBufferTick = -1;
						jumpBufferHoldTick = -1;
						coyoteTick = -1;

						ActiveJumpTick(ref vel);
					}
					else if (jumpInput) {
						if ((nearGround || (! canMidairJump)) && vel.y < m_moveData.maxJumpBufferVelocity) {
							jumpBufferTick = 0; // Buffer the jump
							jumpBufferHoldTick = 0;
						}
						else if (canMidairJump) {
							/*
							 * Because physics, Pythagoras is the starting point as it means double tapping results in slightly less height than holding and then double jumping at the peak. 
							*/
							vel.y = Mathf.Pow(
								Mathf.Pow(Mathf.Max(vel.y, 0), m_moveData.midairJumpYVelReduction)
								+ Mathf.Pow(m_moveData.midairJumpPower, m_moveData.midairJumpYVelReduction)
							, 1 / m_moveData.midairJumpYVelReduction);

							midairJumpCount++;
							jumpBufferTick = -1;
							// jumpHoldTick isn't reset so the player can still get that height
						}
					}
					jumpInput = false;
				}
			}
			else {
				bool jumpHeld = bufferedJumpInput || jumpBufferHoldTick > 0;
				if (jumpHoldTick == m_moveData.maxJumpHoldTime || (! jumpHeld)) {
					jumpHoldTick = -1;
					if (! jumpInput) bufferedJumpInput = false;
				}
				else {
					jumpHoldTick++;
					if (jumpBufferHoldTick != -1) jumpBufferHoldTick--;
					ActiveJumpTick(ref vel);
				}
			}
		}
		private void ActiveJumpTick(ref Vector3 vel) {
			float multiplierFromHold = 1 / (Mathf.Sqrt(jumpHoldTick * m_moveData.jumpHoldCurveSteepness) + 1);

			Vector2 speed2 = new(vel.x, vel.z); // Note: this might have increased for this frame but it hasn't been capped yet
			float multiplierFromSpeed = jumpHoldTick == 0? Mathf.Min(speed2.magnitude / m_moveData.speedForMaxJumpSpeedIncrease, 1) : 0;
			vel.y += (m_moveData.jumpPower * multiplierFromHold) + (m_moveData.maxJumpSpeedIncrease * multiplierFromSpeed);
		}

		private void CapVelocity(ref Vector3 vel) {
			vel = Util.LimitXZ(vel, m_moveData.maxSpeed);
		}

		private void GravityTick(ref Vector3 vel) {
			if (switchGravityInput) {
				Globals.CurrentGravityController.ChangeDirection(Random.Range(0, 6));
				switchGravityInput = false;
			}

			vel.y += Gravity.AmountPerTick;
		}

		#region Tests
		#if UNITY_EDITOR
		private void Tests() {
			Debug.Log("Testing");

			TestRotateAndReverseBatch(new Vector2(0, 1));
			TestRotateAndReverseBatch(new Vector2(0.71f, -0.71f));
		}
		private void TestRotateAndReverseBatch(Vector2 point) {
			TestRotateAndReverse(point, 0);
			TestRotateAndReverse(point, 90);
			TestRotateAndReverse(point, 180);
			TestRotateAndReverse(point, -90);
			TestRotateAndReverse(point, 45);
			TestRotateAndReverse(point, -135);
			TestRotateAndReverse(point, -45);
		}
		private void TestRotateAndReverse(Vector2 point, float deg) {
			float rad = deg * Mathf.Deg2Rad;
			Vector2 rotated = Util.RotateAroundOrigin(point, rad);
			Vector2 rotatedBack = Util.RotateAroundOrigin(rotated, -rad);

			if (rotatedBack != point) {
				throw new System.Exception($"Test failed. Angle (in degrees): {deg}, {rotatedBack} doesn't match expected {point}. Rotated: {rotated}.");
			}
		}
		#endif
		#endregion
	}
}