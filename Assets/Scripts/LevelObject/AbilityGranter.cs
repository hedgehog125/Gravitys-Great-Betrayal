using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

namespace LevelObject {
	public class AbilityGranter : MonoBehaviour {
		[SerializeField] private Ability m_abilityToGrant;
		private enum Ability {
			SwitchUpDown,
			PointSwitch
		}

		private void OnTriggerEnter(Collider collider) {
			Player.Player player = Globals.CurrentPlayer;
			if (collider.gameObject != player.gameObject) return;

			if (m_abilityToGrant == Ability.SwitchUpDown) {
				player.MoveController.CanUpDownSwitch = true;
			}
			else if (m_abilityToGrant == Ability.PointSwitch) {
				player.MoveController.CanPointSwitch = true;
			}
		}
	}
}
