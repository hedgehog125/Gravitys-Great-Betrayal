using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player {
	public class Player : MonoBehaviour {
		private void Awake() {
			Globals.CurrentPlayerObject = gameObject;
		}
	}
}
