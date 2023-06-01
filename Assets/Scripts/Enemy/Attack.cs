using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy {
	public class Attack : MonoBehaviour {
		[SerializeField] private int m_damage = 1;
		[SerializeField] private bool m_includeNonPlayers = false;
		[SerializeField] private bool m_softRespawn = false;

		private void OnCollisionStay(Collision collision) {
			if (collision.gameObject == Globals.CurrentPlayer.gameObject) {
				Globals.CurrentPlayer.HealthController.TakeDamage(m_damage, m_softRespawn);
			}
			else {
				if (m_includeNonPlayers) {
					if (collision.gameObject.TryGetComponent(out Health healthController)) {
						healthController.TakeDamage(m_damage, m_softRespawn);
					}
				}
			}
		}
	}
}
