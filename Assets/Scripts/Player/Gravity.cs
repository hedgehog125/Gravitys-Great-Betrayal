using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player {
	public class Gravity : MonoBehaviour {
		private void Awake() {
			Globals.CurrentGravityController = this;
		}
	}
}
