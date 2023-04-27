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
		}
		private bool switchGravityInput;
		private void OnSwitchGravity(InputValue input) {
			switchGravityInput = input.isPressed;
		}

		private Util.GroundDetector groundDetector;
		private int jumpBufferTick = -1;
		private bool bufferedJumpInput; // Like jumpInput but it's not set to false when a jump is first buffered
		private int jumpHoldTick = -1;
		private int midairJumpCount;


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
			}

			bool jumpBuffered = false;
			if (jumpBufferTick != -1) {
				if (jumpBufferTick == m_moveData.maxJumpBufferTime) {
					jumpBufferTick = -1;
				}
				else {
					jumpBuffered = true;
					jumpBufferTick++;
				}
			}

			if (jumpHoldTick == -1) {
				if (jumpInput || jumpBuffered) {
					bool canMidairJump = midairJumpCount < m_moveData.maxMidairJumps;

					if (onGround) {
						if (vel.y < 0) vel.y = 0;
						jumpHoldTick = 0;
						jumpBufferTick = -1;

						ActiveJumpTick(ref vel);
					}
					else if (nearGround && vel.y < m_moveData.maxJumpBufferVelocity) {
						jumpBufferTick = 0;
					}
					else if (canMidairJump) {
						if (vel.y < 0) vel.y = 0;
						vel.y += m_moveData.midairJumpPower;

						midairJumpCount++;
						jumpBufferTick = -1;
						// jumpHoldTick isn't reset so the player can still get that height
					}
					jumpInput = false;
				}
			}
			else {
				if (jumpHoldTick == m_moveData.maxJumpHoldTime || bufferedJumpInput) { // bufferedJumpInput is dropped in less situations
					jumpHoldTick = -1;
					if (! jumpInput) bufferedJumpInput = false;
				}
				else {
					ActiveJumpTick(ref vel);
					jumpHoldTick++;
				}
			}
		}
		private void ActiveJumpTick(ref Vector3 vel) {
			float adjustedForHoldTime = 1 / (Mathf.Sqrt(jumpHoldTick * m_moveData.jumpHoldCurveSteepness) + 1);
			float multiplierFromSpeed = 1;
			vel.y += m_moveData.jumpPower * adjustedForHoldTime * multiplierFromSpeed;
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