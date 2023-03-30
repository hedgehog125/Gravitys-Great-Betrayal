using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rendering {
	public class LightingController : MonoBehaviour {
		[SerializeField] private Light m_mainLight;

		private readonly Vector3[] LIGHT_ROTATIONS = {
			new (90, 0, 0),
			new (-90, 0, 0),
			new (0, -90, 0),
			new (0, 90, 0),
			new (0, 0, 0),
			new (0, 180, 0)
		};

		private void Start() {
			Globals.CurrentGravityController.Listen(OnGravityChange);
		}

		private void OnGravityChange() {
			m_mainLight.transform.eulerAngles = LIGHT_ROTATIONS[Globals.CurrentGravityController.Direction];
		}
	}
}
