using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy {
	public class Attack : MonoBehaviour {
		[SerializeField] private int m_damage;

		private void OnCollisionStay(Collision collision) {
			if (collision.gameObject != Globals.CurrentPlayer.gameObject) return;

			Globals.CurrentPlayer.HealthController.TakeDamage(m_damage);
		}
	}
}
