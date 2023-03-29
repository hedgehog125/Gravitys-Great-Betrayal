using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player {
	[CreateAssetMenu]
	public class MovementData : ScriptableObject {
		[Header("Running")]
		public float acceleration;
		public float maxSpeed;

		[Header("Jumping")]
		public float jumpPower;
		public LayerMask jumpLayer;

		[Header("Misc")]
		public float topDownThreshold;
	}
}
