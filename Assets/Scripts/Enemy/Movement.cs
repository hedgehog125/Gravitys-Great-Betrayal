using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using PhysicsTools;

namespace Enemy {
	public class Movement : MonoBehaviour {
		private const float MARGIN = 0.05f;

		[SerializeField] private MovementData m_moveData;
		[SerializeField] private Collider m_rangeCollider;

		private Rigidbody rb;
		private FrictionEffector frictionEffector;
		private GravityEffector gravityEffector;
		private Player.Player player;

		private float raycastOffset;
		private Util.GroundDetector groundDetector;

		private bool playerSpotted;
		private int forgetTick = -1;

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
			frictionEffector = GetComponent<FrictionEffector>();
			gravityEffector = GetComponent<GravityEffector>();

			Collider col = GetComponent<Collider>();
			raycastOffset = (col.bounds.size.magnitude / 2) + MARGIN;

			groundDetector = new(col.bounds.size.y, Globals.CurrentConstants.groundLayers);
		}

		private void Start() {
			player = Globals.CurrentPlayer;
		}

		private void FixedUpdate() {
			bool onGround = groundDetector.Check(transform.position, gravityEffector.InPlayerGravity);

			if (playerInRange) {
				Vector3 playerDistance = player.transform.position - transform.position;
				Vector3 playerDirection = playerDistance.normalized;
				bool didHit = Physics.Raycast(transform.position + (playerDirection * raycastOffset), playerDirection, out RaycastHit hit, playerDistance.magnitude + MARGIN, Globals.CurrentConstants.groundLayers);
				bool playerVisible = didHit && hit.collider.gameObject == player.gameObject;

				if (playerVisible) {
					playerSpotted = true;
					forgetTick = 0;
				}

				if (playerSpotted && playerVisible) MoveTick(playerDirection);

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

			frictionEffector.Tick(onGround);
		}

		private void MoveTick(Vector3 playerDirection) {
			Gravity gravityController = Globals.CurrentGravityController;
			Vector3 vel = gravityController.Adjust(rb.velocity, gravityEffector.InPlayerGravity);

			Vector3 adjustedDirection = gravityController.Adjust(playerDirection, gravityEffector.InPlayerGravity);
			adjustedDirection.y = 0;
			adjustedDirection = adjustedDirection.normalized;

			vel.x += adjustedDirection.x * m_moveData.acceleration;
			vel.z += adjustedDirection.z * m_moveData.acceleration;

			rb.velocity = gravityController.Apply(vel, gravityEffector.InPlayerGravity);
		}
	}
}
