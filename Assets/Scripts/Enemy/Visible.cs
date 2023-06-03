using PhysicsTools;
using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy {
	public class Visible : MonoBehaviour {
		[SerializeField] private GravityEffector m_gravityEffector;
		[SerializeField] private Transform m_modelRoot;

		[HideInInspector] public float LookAngle;

		private void FixedUpdate() {
			transform.eulerAngles = Gravity.Rotations[Globals.CurrentGravityController.GetEffectiveDirection(m_gravityEffector)];
			m_modelRoot.localEulerAngles = new Vector3(0, LookAngle, 0);
		}
	}
}
