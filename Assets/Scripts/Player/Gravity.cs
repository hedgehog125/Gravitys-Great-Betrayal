using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

namespace Player {
	public class Gravity : MonoBehaviour {
		[SerializeField] private GameObject m_areaObject;

		public static readonly Vector3Int[] Directions = {
			new(0, -1, 0), // Down
			new(0, 1, 0), // Up
			new(-1, 0, 0), // Left
			new(1, 0, 0), // Right
			new(0, 0, 1), // Away
			new(0, 0, -1) // Towards
		};
		[HideInInspector] public static readonly float AmountPerTick = Physics.gravity.y / 50;

		[HideInInspector] public LayerMask Layer { get; private set; }
		[HideInInspector] public Vector3 AccelerationPerTick { get; private set; }
		[HideInInspector] public int Direction;

		public Gravity() {
			ChangeDirection(0);
		}
		private void Awake() {
			Layer = m_areaObject.layer;
			Globals.CurrentGravityController = this;
		}

		public void ChangeDirection(int _direction) {
			Direction = _direction;
			AccelerationPerTick = (Vector3)Directions[Direction] * AmountPerTick;
		}

		public Vector3 Adjust(Vector3 original) { // Changes the order and signs so negative y is always down
			return AdjustForDirection(original, Direction, 0);
		}
		public Vector3 Apply(Vector3 original) { // Reverses the changes in adjust
			return AdjustForDirection(original, 0, Direction);
		}
		public static Vector3 AdjustForDirection(Vector3 original, int originalDirection, int targetDirection) {
			// 1,-2,0
			// 0 -> 0,-1,0
			// 2 -> -1,0,0

			// Output: -2,-1,0

			// Get angle between the directions
			// Rotate around 0,0,0 by that angle
			// TODO: Lookup table?
		}
	}
}
