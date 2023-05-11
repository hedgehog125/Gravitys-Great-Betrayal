using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PhysicsTools {
	public class FrictionEffector : MonoBehaviour {
		[SerializeField] private bool m_preventWallFriction = true;
		[SerializeField] private bool m_manageByScript = false;
		[SerializeField] private PhysicMaterial m_frictionlessMaterial;
		[SerializeField] private LayerMask m_groundLayers;

		private PhysicMaterial frictionMaterial;
		private Util.GroundDetector groundDetector;
		private GravityEffector gravityEffector;

		private Collider col;
		private void Awake() {
			col = GetComponent<Collider>();

			frictionMaterial = col.material;
			if (! m_manageByScript) {
				groundDetector = new(col.bounds.size.y, m_groundLayers);
				gravityEffector = GetComponent<GravityEffector>();
			}
		}

		private void FixedUpdate() {
			if (! m_manageByScript) {
				Tick(groundDetector.Check(transform.position, gravityEffector == null || gravityEffector.InPlayerGravity));
			}
		}

		public void Tick(bool onGround) {
			if (m_preventWallFriction) {
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
