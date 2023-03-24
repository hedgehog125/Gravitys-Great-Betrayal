using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player {
	public class Gravity : MonoBehaviour {
		[SerializeField] private GameObject m_areaObject;

		[HideInInspector] public LayerMask Layer { get; private set; }
		[HideInInspector] public float AmountPerTick { get; private set; } = Physics.gravity.y / 50;
		[HideInInspector] public Vector3 AccelerationPerTick { get; private set; }

		public Gravity() {
			AccelerationPerTick = new Vector3(0, -AmountPerTick, 0);
		}
		private void Awake() {
			Layer = m_areaObject.layer;
			Globals.CurrentGravityController = this;
		}
	}
}
