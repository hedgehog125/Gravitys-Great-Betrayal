using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityEffector : MonoBehaviour {
	[HideInInspector] public bool InPlayerGravity { get; private set; } = false;

	private Rigidbody rb;
	private void Awake() {
		rb = GetComponent<Rigidbody>();
	}

	private void OnTriggerEnter(Collider other) {
		if (other.gameObject.layer != Globals.CurrentGravityController.Layer) return;

		UpdateGravityPhysics(true);
	}
	private void OnTriggerExit(Collider other) {
		if (other.gameObject.layer != Globals.CurrentGravityController.Layer) return;

		UpdateGravityPhysics(false);
	}

	private void FixedUpdate() {
		Vector3 vel = rb.velocity;

		if (InPlayerGravity) {
			vel += Globals.CurrentGravityController.AccelerationPerTick;
		}

		rb.velocity = vel;
	}

	private void UpdateGravityPhysics(bool _inPlayerGravity) {
		InPlayerGravity = _inPlayerGravity;

		rb.useGravity = ! InPlayerGravity;
	}
}
