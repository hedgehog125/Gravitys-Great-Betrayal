using Sound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player {
	public class Sounds : MonoBehaviour {
		[SerializeField] private Movement m_movementController;
		[SerializeField] private PlayerSoundData m_soundData;


		private float walkSoundTick;

		private void OnJump() {
			snd.Play(Util.GetRandomItem(m_soundData.jumpSounds));
		}

		private SoundPlayer snd;
		private void Awake() {
			snd = GetComponent<SoundPlayer>();
		}

		private void Start() {
			m_movementController.ListenForJump(OnJump);
		}

		private void Update() {
			if (m_movementController.IsGrounded && m_movementController.CurrentSpeed > m_soundData.minWalkSpeed) {
				float relativeSpeed = m_movementController.CurrentSpeed / m_movementController.MovementData.maxSpeed;
				relativeSpeed = Mathf.Min(relativeSpeed, 1f);
				relativeSpeed = (relativeSpeed * (1 - m_soundData.minRelativeSpeed)) + m_soundData.minRelativeSpeed;

				if (walkSoundTick <= 0f) {
					AudioSource source = snd.Play(Util.GetRandomItem(m_soundData.stepSounds), true);
					if (source != null) {
						source.volume = relativeSpeed * m_soundData.maxStepVolume;
						source.Play();
					}

					walkSoundTick += m_soundData.minStepSoundGap;
				}

				walkSoundTick -= Time.deltaTime * relativeSpeed;
			}
			else {
				walkSoundTick = 0f;
			}
		}
	}
}
