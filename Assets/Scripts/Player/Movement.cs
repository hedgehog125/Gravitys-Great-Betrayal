using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player { 
	public class Movement : MonoBehaviour {
		[SerializeField] private MovementData m_moveData;

		private Vector2 moveInput;
		private void OnMove(InputValue input) {
			moveInput = input.Get<Vector2>();
		}
		private bool jumpInput;
		private void OnJump(InputValue input) {
			jumpInput = input.isPressed;
		}
		private bool switchGravityInput;
		private void OnSwitchGravity(InputValue input) {
			switchGravityInput = input.isPressed;
		}


		private Rigidbody rb;
		private Camera cam;
		private void Awake() {
			rb = GetComponent<Rigidbody>();
			cam = Camera.main;
		}

		private void FixedUpdate() {
			Vector3 vel = Globals.CurrentGravityController.Adjust(rb.velocity);

			MoveTick(ref vel);
			JumpTick(ref vel);
			CapVelocity(ref vel);

			GravityTick(ref vel);
			rb.velocity = Globals.CurrentGravityController.Apply(vel);
		}


		private void MoveTick(ref Vector3 vel) {
			float rad = Mathf.Atan2(moveInput.x, moveInput.y) + (cam.transform.eulerAngles.y * Mathf.Deg2Rad);
			float distance = Mathf.Sqrt(Mathf.Pow(moveInput.x, 2) + Mathf.Pow(moveInput.y, 2));
			vel.x += Mathf.Sin(rad) * distance * m_moveData.acceleration;
			vel.z += Mathf.Cos(rad) * distance * m_moveData.acceleration;
		}
		private void JumpTick(ref Vector3 vel) {
			if (jumpInput) {
				vel.y += m_moveData.jumpPower;
				jumpInput = false;
			}
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
	}
}