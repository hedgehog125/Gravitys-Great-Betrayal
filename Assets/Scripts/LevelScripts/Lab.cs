using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

namespace LevelScripts {
	public class Lab : MonoBehaviour {
		[SerializeField] private float m_startAreaRadius;


		private Vector3 playerStartLocation;
		private bool playerLeftStartArea;
		
		private bool playerChangedGravity;
		private void OnGravityChangedByPlayer() {
			playerChangedGravity = true;
		}


		private void Start() {
			Player.Player player = Globals.CurrentPlayer;

			playerStartLocation = player.transform.position;
			player.MoveController.ListenForGravityChangesByMe(OnGravityChangedByPlayer);
		}

		private void FixedUpdate() {
			if (! playerLeftStartArea) {
				if ((Globals.CurrentPlayer.transform.position - playerStartLocation).magnitude > m_startAreaRadius) {
					if (! playerChangedGravity) {
						Movement moveController = Globals.CurrentPlayer.MoveController;
						moveController.CanUpDownSwitch = false;
						moveController.CanPointSwitch = false;
					}
					playerLeftStartArea = true;
				}
			}
		}
	}
}
