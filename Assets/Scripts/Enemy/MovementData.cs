using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy {
	[CreateAssetMenu(menuName = "Data/Enemy/Movement")]
	public class MovementData : ScriptableObject {
		public float acceleration;
		public float maxSpeed;

		public float jumpPower;
		public int jumpCooldown;

		public int forgetTime;
		public float minTargetDistance; // The enemy can get closer than this, but it'll start to stop after this point
		public int stuckTime;
	}
}
