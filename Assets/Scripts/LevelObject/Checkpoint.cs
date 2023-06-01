using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace LevelObject {
	public class Checkpoint : MonoBehaviour {
		[SerializeField] private int m_id = -1;

		private void OnTriggerEnter(Collider collider) {
			if (collider.gameObject != Globals.CurrentPlayer.gameObject) return;

			Globals.CurrentPlayer.HandleCheckpoint(m_id, transform.position);
		}
	}
}
