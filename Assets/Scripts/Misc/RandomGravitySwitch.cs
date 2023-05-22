using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomGravitySwitch : MonoBehaviour {
	[SerializeField] private int m_minSwitchDelay;
	[SerializeField] private int m_maxSwitchDelay;

	private int switchDelayTick;
	private void Awake() {
		ResetSwitchTime();
	}

	private void FixedUpdate() {
		if (switchDelayTick == 0) {
			int newDirection;
			do {
				newDirection = Random.Range(0, 6);
			} while (newDirection == Globals.CurrentGravityController.Direction);

			Globals.CurrentGravityController.ChangeDirection(newDirection);
			ResetSwitchTime();
		}
		else {
			switchDelayTick--;
		}
	}

	private void ResetSwitchTime() {
		switchDelayTick = Random.Range(m_minSwitchDelay, m_maxSwitchDelay + 1);
	}
}
