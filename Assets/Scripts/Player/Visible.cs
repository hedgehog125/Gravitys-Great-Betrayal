using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player {
	public class Visible : MonoBehaviour {
		private void Update() {
			transform.eulerAngles = Gravity.Rotations[Globals.CurrentGravityController.Direction];
		}
	}
}
