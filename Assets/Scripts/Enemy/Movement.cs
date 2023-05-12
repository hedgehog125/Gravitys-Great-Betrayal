using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using PhysicsTools;
using UnityEngine.EventSystems;

namespace Enemy {
	public class Movement : MonoBehaviour {
		private const float MARGIN = 0.05f;
		private const float MAX_STUCK_MOVEMENT = 0.075f;
		private const float MIN_TURN_ANGLE = 100; // It's a turn when there's more than this many degrees difference between the velocity and target angle. Turns reset stuckTick

		[SerializeField] private MovementData m_moveData;
		[SerializeField] private Collider m_rangeCollider;

		private Rigidbody rb;
		private Collider col;
		private FrictionEffector frictionEffector;
		private GravityEffector gravityEffector;
		private Player.Player player;

		private float raycastOffset;
		private Util.GroundDetector groundDetector;

		private bool playerSpotted;
		private int forgetTick = -1;
		private Vector3 lastKnownPos;

		private int jumpTick = -1;
		private Vector3 lastPos;
		private int stuckTick = -1;
		private float currentTurnAmount;

		private bool playerInRange;
		private void OnTriggerEnter(Collider collider) {
			if (collider.gameObject != player.gameObject) return;

			playerInRange = true;
		}
		private void OnTriggerExit(Collider collider) {
			if (collider.gameObject != player.gameObject) return;

			playerInRange = false;
			playerSpotted = false;
			forgetTick = -1;
		}

		private void Awake() {
			rb = GetComponent<Rigidbody>();
			col = GetComponent<Collider>();
			frictionEffector = GetComponent<FrictionEffector>();
			gravityEffector = GetComponent<GravityEffector>();

			raycastOffset = (col.bounds.size.magnitude / 2) + MARGIN;
		}

		private void Start() {
			player = Globals.CurrentPlayer;
			groundDetector = new(col.bounds.size.y, Globals.CurrentConstants.groundLayers);
		}

		private void FixedUpdate() {
			bool onGround = groundDetector.Check(transform.position, gravityEffector.InPlayerGravity);
			bool moving = false;

			if (playerInRange) {
				Vector3 playerDistance = player.transform.position - transform.position;
				Vector3 playerDirection = playerDistance.normalized;
				bool didHit = Physics.Raycast(transform.position + (playerDirection * raycastOffset), playerDirection, out RaycastHit hit, playerDistance.magnitude + MARGIN, Globals.CurrentConstants.groundLayers);
				bool playerVisible = didHit && hit.collider.gameObject == player.gameObject;

				if (playerVisible) {
					playerSpotted = true;
					forgetTick = 0;
					lastKnownPos = player.transform.position;
				}
				else {
					playerDistance = lastKnownPos - transform.position;
					playerDirection = playerDistance.normalized;
				}

				if (playerSpotted) moving = MoveTick(playerDistance, playerDirection, onGround);

				if (! playerVisible) {
					if (playerSpotted) {
						if (forgetTick == m_moveData.forgetTime) {
							playerSpotted = false;
							forgetTick = -1;
						}
						else {
							forgetTick++;
						}
					}
				}
			}

			ProcessStuckTick(moving);
			CapSpeed();
			frictionEffector.Tick(onGround);
		}

		private bool MoveTick(Vector3 playerDistance, Vector3 playerDirection, bool onGround) {
			Gravity gravityController = Globals.CurrentGravityController;
			Vector3 vel = gravityController.Adjust(rb.velocity, gravityEffector.InPlayerGravity);

			Vector3 adjustedDistanceVec = gravityController.Adjust(playerDistance, gravityEffector.InPlayerGravity);
			float horizontalDistance;
			{
				Vector3 noY = adjustedDistanceVec;
				noY.y = 0;
				horizontalDistance = noY.magnitude;
			}
			if (horizontalDistance < m_moveData.minTargetDistance) return false;

			Vector3 adjustedDirection = gravityController.Adjust(playerDirection, gravityEffector.InPlayerGravity);
			adjustedDirection.y = 0;
			adjustedDirection = adjustedDirection.normalized;

			vel.x += adjustedDirection.x * m_moveData.acceleration;
			vel.z += adjustedDirection.z * m_moveData.acceleration;

			if (jumpTick == -1) {
				if (
					stuckTick == m_moveData.stuckTime
					&& onGround
				) {
					vel.y += m_moveData.jumpPower;
					jumpTick = 0;
				}
			}
			else {
				if (
					jumpTick != 0
					|| (vel.y < 0.05f && onGround)
				) {
					if (jumpTick == m_moveData.jumpCooldown) {
						jumpTick = -1;
					}
					else {
						jumpTick++;
					}
				}
			}

			currentTurnAmount = Util.GetHorizontalTurnAmount(vel, adjustedDirection);
			rb.velocity = gravityController.Apply(vel, gravityEffector.InPlayerGravity);

			return true;
		}
		private void ProcessStuckTick(bool moving) {
			bool shouldUpdateLastPos = true;
			if (moving) {
				if ((transform.position - lastPos).magnitude < MAX_STUCK_MOVEMENT) {
					if (currentTurnAmount < MIN_TURN_ANGLE) {
						if (stuckTick != m_moveData.stuckTime) {
							stuckTick++;
							shouldUpdateLastPos = false;
						}
					}
					else {
						stuckTick = -1;
					}
				}
				else {
					stuckTick = -1;
				}
			}
			else {
				stuckTick = -1;
				currentTurnAmount = 0;
			}
			if (shouldUpdateLastPos) {
				lastPos = transform.position;
			}
		}
		private void CapSpeed() {
			Vector3 vel = Globals.CurrentGravityController.Adjust(rb.velocity, gravityEffector.InPlayerGravity);
			vel = Util.LimitXZ(vel, m_moveData.maxSpeed);
			rb.velocity = Globals.CurrentGravityController.Apply(vel, gravityEffector.InPlayerGravity);
		}
	}
}
