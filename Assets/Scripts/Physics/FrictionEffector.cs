using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PhysicsTools {
	public class FrictionEffector : MonoBehaviour {
		private const float MARGIN = 0.05f;

		[SerializeField] private bool m_preventWallFriction = true;
		[SerializeField] private bool m_manageByScript = false;
		[SerializeField] private PhysicMaterial m_frictionlessMaterial;
		[SerializeField] private LayerMask m_groundLayers;

		private Vector3 raycastOriginOffset;
		private PhysicMaterial frictionMaterial;

		private Collider col;
		private void Awake() {
			col = GetComponent<Collider>();

			raycastOriginOffset = new(0, -((col.bounds.size.y / 2) - (MARGIN / 2)), 0);
			frictionMaterial = col.material;
		}

		private void FixedUpdate() {
			if (m_preventWallFriction) {
				bool onGround = Physics.Raycast(transform.position + Globals.CurrentGravityController.Apply(raycastOriginOffset), Globals.CurrentGravityController.Down, MARGIN, m_groundLayers);
				if (onGround) {
					col.material = frictionMaterial;
				}
				else {
					col.material = m_frictionlessMaterial;
				}
			}
		}
	}
}
