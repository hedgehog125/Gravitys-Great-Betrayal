using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy {
	[CreateAssetMenu(menuName = "Data/Enemy/Movement")]
	public class MovementData : ScriptableObject {
		public float acceleration;
		public float maxSpeed;

		public int forgetTime;
	}
}
