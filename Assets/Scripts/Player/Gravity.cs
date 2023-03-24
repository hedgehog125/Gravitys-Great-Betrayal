using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

namespace Player {
	public class Gravity : MonoBehaviour {
		#region Serialised fields
		[SerializeField] private GameObject m_areaObject;
		#endregion

		#region Public variables
		public static readonly Vector3Int[] Directions = {
			new(0, -1, 0), // Down
			new(0, 1, 0), // Up
			new(-1, 0, 0), // Left
			new(1, 0, 0), // Right
			new(0, 0, 1), // Away
			new(0, 0, -1) // Towards
		};
		[HideInInspector] public static readonly float AmountPerTick = Physics.gravity.y / 50; // Negative

		[HideInInspector] public LayerMask Layer { get; private set; }
		[HideInInspector] public Vector3 AccelerationPerTick { get; private set; }
		[HideInInspector] public int Direction;
		#endregion


		/*
		 * Maps each index in the original to an index in the new, they start at 1 since there's no -0. Negative number means the sign is flipped from the original 
		 * Example:
		 * { 3, 2, -1 }
		 * Means the original x value becomes the z value in the new vector. The y is left unchanged. And the original z becomes the x value, but its sign is flipped.
		 * 
		 * It doesn't work the other way around because of the signs
		*/
		private static readonly int[][] DIRECTIONS_TO_ADJUSTED = {
			new int[] { 1, 2, 3 }, // Down, so no changes
			new int[] { -1, -2, 3 }, // Up
			new int[] { -2, 1, 3 }, // Left
			new int[] { 2, -1, 3 }, // Right
			new int[] { 1, -3, 2 }, // Away
			new int[] { 1, 3, -2 } // Towards
		};
		private static readonly int[][] ADJUSTED_TO_DIRECTIONS = CalculateInvertDirections(DIRECTIONS_TO_ADJUSTED);

		private void Awake() {
			ChangeDirection(1);

			Layer = m_areaObject.layer;
			Globals.CurrentGravityController = this;
		}

		public void ChangeDirection(int _direction) {
			Direction = _direction;
			AccelerationPerTick = (Vector3)Directions[Direction] * -AmountPerTick;
		}

		// These can be done in different ways, but it doesn't matter as long as it can be reversed. e.g rotating from upwards gravity to downwards gravity can be done by rotating on 2 different axes (but not both)
		public Vector3 Adjust(Vector3 original) { // Changes the order and signs so negative y is always down
			return AdjustFromDirection(original, Direction);
		}
		public Vector3 Apply(Vector3 original) { // Reverses the changes in adjust
			return ApplyToDirection(original, Direction);
		}
		public static Vector3 AdjustFromDirection(Vector3 original, int originalDirectionID) {
			return AdjustOrApply(original, originalDirectionID, false);
		}
		public static Vector3 ApplyToDirection(Vector3 original, int targetDirectionID) {
			return AdjustOrApply(original, targetDirectionID, true);
		}


		// To convert from an adjusted back to a direction, just do the same method but flip the signs on the constants if they're a different index to that one.
		// e.g left becomes { 2, -1, 3 }
		private static int[][] CalculateInvertDirections(int[][] toAdjusted) {
			int[][] toDirections = new int[toAdjusted.Length][];

			for (int i = 0; i < toAdjusted.Length; i++) {
				int[] toDirection = new int[3];
				for (int c = 0; c < 3; c++) {
					int newIndex = toAdjusted[i][c]; // 1 based
					int absIndex = Mathf.Abs(newIndex) - 1; // 0 based
					if (absIndex != c) { // Only flip the sign if it gets moved
						newIndex *= -1;
					}

					toDirection[c] = newIndex;
				}

				toDirections[i] = toDirection;
			}

			return toDirections;
		}
		private static Vector3 AdjustOrApply(Vector3 original, int directionID, bool isApply) {
			// If isApply is true, directionID is the target direction, otherwise it's the direction of the original

			Vector3 outputVector = Vector3.zero;
			int[][] lookupTable = isApply? ADJUSTED_TO_DIRECTIONS : DIRECTIONS_TO_ADJUSTED;
			int[] adjustment = lookupTable[directionID];
			for (int i = 0; i < 3; i++) {
				bool isNegative = adjustment[i] < 0;
				int newIndex = Mathf.Abs(adjustment[i]) - 1;

				outputVector[newIndex] = isNegative? -original[i] : original[i];
			}
			return outputVector;
		}


		#region Tests
		private void Tests() {
			Debug.Log("Testing");

			{
				Vector3 adjusted = AdjustFromDirection(new Vector3(0, -2, 0), 1);
				Vector3Int rounded = new(Mathf.RoundToInt(adjusted.x), Mathf.RoundToInt(adjusted.y), Mathf.RoundToInt(adjusted.z));

				if (rounded != new Vector3Int(0, 2, 0)) {
					throw new System.Exception($"Upside down test failed. {adjusted} should be {{ 0, 2, 0 }}.");
				}
			}
			for (int i = 0; i < 6; i++) {
				TestAdjustAndApply(new Vector3(1, 2, 3), i);
			}
		}
		private void TestAdjustAndApply(Vector3 original, int directionID) {
			Vector3 adjusted = AdjustFromDirection(original, directionID);
			Vector3 convertedBack = ApplyToDirection(adjusted, directionID);

			if (convertedBack != original) {
				throw new System.Exception($"Test failed. Direction: {directionID}, {convertedBack} doesn't match expected {original}");
			}
		}
		#endregion
	}
}
