using PhysicsTools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Player { 
	public class Movement : MonoBehaviour {
		private static Vector3 ADJUSTED_FORWARD = new(0, 0, 1);
		private static Vector3 ADJUSTED_UP = new(0, 1, 0);


		[SerializeField] private MovementData m_moveData;



		[HideInInspector] public bool IsGrounded { get; private set; }
		[HideInInspector] public float CurrentSpeed { get; private set; }
		[HideInInspector] public Vector3 AdjustedVelocity { get; private set; }
		[HideInInspector] public float LandSpeed { get; private set; }
		[HideInInspector] public MovementData MovementData { get => m_moveData; }
		[HideInInspector] public bool IsDoubleJump { get; private set; }

		[NonSerialized] public bool CanUpDownSwitch = true;
		[NonSerialized] public bool CanPointSwitch = true;


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
		private bool switchGravityUpInput;
		private void OnSwitchGravityUp(InputValue input) {
			switchGravityUpInput = input.isPressed;
		}

		private	void OnSoftRespawn() {
			Globals.CurrentGravityController.ChangeDirection(gravityHistoryWhileSafe[^1]);
		}

		private readonly UnityEvent jumpEvent = new();
		private readonly UnityEvent landEvent = new();
		private readonly UnityEvent gravityChangeEvent = new();

		private Util.GroundDetector groundDetector;
		private bool wasOnGround = true;
		private int stuckTick;

		private int jumpBufferTick = -1;
		private int jumpBufferHoldTick = -1; // Records how long jump was held in midair
		private bool bufferedJumpInput; // Like jumpInput but it's not set to false when a jump is first buffered or when a jump is started

		private int jumpHoldTick = -1;
		private int midairJumpCount;
		private int coyoteTick = -1;
		private float[] speedHistory; // Used for the jump increase with speed

		private Vector3 facingDirection = ADJUSTED_FORWARD;
		private float currentTurnAmount;
		private int midairGravitySwitchCount;
		private int gravitySwitchCooldownTick = -1;
		private int gravitySwitchFloatTick = -1;
		private int[] gravityHistoryWhileSafe; // Used for soft respawns
		private Vector3[] adjustedVelocityHistory;


		private Rigidbody rb;
		private FrictionEffector frictionEffector;
		private Health healthController;
		private Player player;
		private Camera cam;
		private void Awake() {
			rb = GetComponent<Rigidbody>();
			frictionEffector = GetComponent<FrictionEffector>();
			healthController = GetComponent<Health>();
			player = GetComponent<Player>();
			cam = Camera.main;

			speedHistory = new float[m_moveData.jumpSpeedHistoryOffset];
			gravityHistoryWhileSafe = new int[healthController.SafeTime];
			adjustedVelocityHistory = new Vector3[10];
		}
		private void Start() {
			Collider col = GetComponent<Collider>();
			groundDetector = new(col.bounds.size.y, Globals.CurrentConstants.groundLayers, m_moveData.nearGroundDistance);

			healthController.ListenForSoftRespawn(OnSoftRespawn);
		}

		private void FixedUpdate() {
			Vector3 vel = Globals.CurrentGravityController.Adjust(rb.velocity);
			bool onGround = groundDetector.Check(transform.position, out float groundDistance);
			bool nearGround = groundDistance < m_moveData.nearGroundDistance;
			

			bool inputIsNeutral = MoveTick(ref vel, onGround);
			SpeedHistoryTick(vel); // Uncapped speed is used
			CoyoteTick(onGround);
			JumpTick(ref vel, onGround, nearGround, inputIsNeutral);
			VelocityTick(ref vel, onGround, inputIsNeutral);

			rb.velocity = Globals.CurrentGravityController.Apply(vel);
			StuckTick(vel, onGround);
			GravityTick(onGround);

			frictionEffector.Tick(onGround);
			if (healthController.IsSafe && onGround) {
				healthController.SoftRespawnLocation = transform.position;
				Util.InsertAtStartAndShift(Globals.CurrentGravityController.Direction, gravityHistoryWhileSafe);
			}
			IsGrounded = onGround;
			CurrentSpeed = vel.magnitude;
			AdjustedVelocity = vel;
			if (onGround != wasOnGround) {
				if (onGround) {
					LandSpeed = Mathf.Max(adjustedVelocityHistory.Select(vel => -vel.y).ToArray());
					landEvent.Invoke();
				}
				wasOnGround = onGround;
			}
		}


		private bool MoveTick(ref Vector3 vel, bool onGround) {
			float moveAmount = Mathf.Sqrt(Mathf.Pow(moveInput.x, 2) + Mathf.Pow(moveInput.y, 2));
			bool inputIsNeutral = moveAmount < 0.01f; // If it's less than the deadzone then it'll have already been set to 0
			if (inputIsNeutral) {
				currentTurnAmount = 0;
				return true;
			}

			Vector3 moveDirection;
			{
				Vector3 forward = cam.transform.forward;
				forward = Globals.CurrentGravityController.Adjust(forward);
				forward.y = 0;

				if (forward.magnitude < m_moveData.topDownThreshold) { // Somewhat of a niche situation, just use the gravity remappings
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

			bool turning = currentTurnAmount > m_moveData.turnMinChange;
			float acceleration = onGround? m_moveData.acceleration : m_moveData.airAcceleration;
			if (turning) acceleration *= onGround? m_moveData.turnAccelerationMultiplier : m_moveData.airTurnAccelerationMultiplier;

			vel += moveDirection * (moveAmount * acceleration);

			SetVisibleLookAngle(moveDirection);

			currentTurnAmount = Util.GetHorizontalTurnAmount(vel, moveDirection);
			facingDirection = moveDirection;


			return false;
		}
		private void SetVisibleLookAngle(Vector3 moveDirection) {
			player.VisibleController.LookAngle = Util.GetLookAngle(moveDirection, Globals.CurrentGravityController.Direction);
		}
		private void CoyoteTick(bool onGround) {
			if (onGround) {
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
		}
		private void JumpTick(ref Vector3 vel, bool onGround, bool nearGround, bool inputIsNeutral) {
			if (onGround) {
				midairJumpCount = 0;
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
						IsDoubleJump = false;

						ActiveJumpTick(ref vel);
						jumpEvent.Invoke();
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

							float multiplier = (inputIsNeutral || currentTurnAmount < m_moveData.doubleJumpSubtractiveMinTurn)?
								m_moveData.doubleJumpAdditiveMultiplier
								: m_moveData.doubleJumpSubtractiveMultiplier
							;
							vel = Util.MultiplyXZMagnitude(vel, multiplier);

							midairJumpCount++;
							jumpBufferTick = -1;
							// jumpHoldTick doesn't need to be reset as it'll be -1 here anyway and can't be restarted
							IsDoubleJump = true;
							jumpEvent.Invoke();
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

			float speed = Mathf.Min(speedHistory);
			float multiplierFromSpeed = jumpHoldTick == 0? Mathf.Min(speed / m_moveData.speedForMaxJumpSpeedIncrease, 1) : 0;
			vel.y += (m_moveData.jumpPower * multiplierFromHold) + (m_moveData.maxJumpSpeedIncrease * multiplierFromSpeed);
		}

		private void VelocityTick(ref Vector3 vel, bool onGround, bool inputIsNeutral) {
			vel = Util.LimitXZ(vel, m_moveData.maxSpeed);

			if (inputIsNeutral) {
				float multiplier = onGround? m_moveData.neutralSpeedMultiplier : m_moveData.airNeutralSpeedMultiplier;
				vel = Util.MultiplyXZMagnitude(vel, multiplier);
			}
		}
		private void StuckTick(Vector3 vel, bool onGround) {
			bool isStuck = vel.y > -0.05f && (! onGround);

			if (isStuck) {
				if (stuckTick == m_moveData.stuckTime) {
					healthController.SoftRespawn();
					stuckTick = 0;
				}
				else {
					stuckTick++;
				}
			}
			else {
				stuckTick = 0;
			}
		}

		private void GravityTick(bool onGround) {
			if ((onGround || coyoteTick != -1) && gravitySwitchCooldownTick == -1) {
				midairGravitySwitchCount = 0;
			}

			if (gravitySwitchCooldownTick != -1) {
				if (gravitySwitchCooldownTick == m_moveData.gravitySwitchCooldown) {
					gravitySwitchCooldownTick = -1;
				}
				else {
					gravitySwitchCooldownTick++;
				}
			}
			if (gravitySwitchFloatTick != -1) {
				if (gravitySwitchFloatTick == m_moveData.gravitySwitchFloatTime) {
					gravitySwitchFloatTick = -1;
				}
				else {
					gravitySwitchFloatTick++;
				}
			}

			if (
				gravitySwitchCooldownTick == -1
				&& midairGravitySwitchCount != m_moveData.maxMidairGravitySwitches
			) {
				if (switchGravityInput || switchGravityUpInput) {
					Gravity gravityController = Globals.CurrentGravityController;

					int newDirectionID = -1;
					if (switchGravityInput) {
						if (CanPointSwitch) {
							Vector3 appliedFacing = gravityController.Apply(facingDirection);

							int largestID = Util.GetAbsLargestAxis(appliedFacing);
							Vector3Int axisDirection = Vector3Int.zero;
							axisDirection[largestID] = appliedFacing[largestID] > 0? 1 : -1;

							newDirectionID = Gravity.DirectionToID(axisDirection);
						}
					}
					else {
						if (CanUpDownSwitch) {
							newDirectionID = Gravity.DirectionToID(
								Util.RoundVector(gravityController.Apply(ADJUSTED_UP))
							);
						}
					}

					if (newDirectionID != -1) {
						if (newDirectionID != gravityController.Direction) {
							Vector3 vel = gravityController.Adjust(rb.velocity);
							if (vel.y < 0) vel.y = 0;
							rb.velocity = gravityController.Apply(vel);

							gravityController.ChangeDirection(newDirectionID);
							

							gravitySwitchCooldownTick = 0;
							gravitySwitchFloatTick = 0;
							midairGravitySwitchCount++;
							midairJumpCount = 0;

							gravityChangeEvent.Invoke();
						}
					} 
				}
			}

			if (gravitySwitchFloatTick == -1) {
				Vector3 vel = Globals.CurrentGravityController.Adjust(rb.velocity);
				vel.y += Gravity.AmountPerTick;
				rb.velocity = Globals.CurrentGravityController.Apply(vel);
			}
			

			switchGravityInput = false;
			switchGravityUpInput = false;
		}
		private void SpeedHistoryTick(Vector3 vel) {
			Vector2 speed2 = new(vel.x, vel.z);
			float currentSpeed = speed2.magnitude;

			Util.InsertAtStartAndShift(currentSpeed, speedHistory);
			Util.InsertAtStartAndShift(vel, adjustedVelocityHistory);
		}



		// Public methods
		public void ResetFacingDirection() {
			facingDirection = ADJUSTED_FORWARD;
			SetVisibleLookAngle(ADJUSTED_FORWARD);
		}

		public void ListenForJump(UnityAction callback) {
			jumpEvent.AddListener(callback);
		}
		public void ListenForLand(UnityAction callback) {
			landEvent.AddListener(callback);
		}
		public void ListenForGravityChangesByMe(UnityAction callback) {
			gravityChangeEvent.AddListener(callback);
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