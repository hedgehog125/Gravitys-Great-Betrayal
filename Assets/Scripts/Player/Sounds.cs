using Sound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player {
	public class Sounds : MonoBehaviour {
		[SerializeField] private Movement m_movementController;
		[SerializeField] private PlayerSoundData m_soundData;


		private float walkSoundTick;
		private SoundData lastGroundedJumpSound;
		private int landCooldownTick = -1; // Prevents the sound from playing multiple times

		private int fallTick;
		private AudioSource fallSource;

		private void OnJump() {
			SoundData sound;
			if (m_movementController.IsDoubleJump) {
				int tries = 0;
				do {
					sound = Util.GetRandomItem(m_soundData.jumpSounds);
					tries++;
				} while (sound == lastGroundedJumpSound && tries != 30);
			}
			else {
				sound = Util.GetRandomItem(m_soundData.jumpSounds);
				lastGroundedJumpSound = sound;
			}

			snd.Play(sound);
		}
		private void OnLand() {
			if (landCooldownTick == -1) {
				if (m_movementController.LandSpeed > m_soundData.minLandVelocity) {
					float relativeSpeed = m_movementController.LandSpeed / m_soundData.speedForMaxLandSound;
					relativeSpeed = Mathf.Min(relativeSpeed, 1f);
					relativeSpeed = (relativeSpeed * (1 - m_soundData.minRelativeLandSpeed)) + m_soundData.minRelativeLandSpeed;

					AudioSource source = snd.Play(Util.GetRandomItem(m_soundData.landSounds), true);
					if (source != null) {
						source.volume = relativeSpeed * m_soundData.maxLandVolume;
						source.Play();
					}
					landCooldownTick = 0;
				}
			}
		}

		private void OnGravityChangedByPlayer() {
			snd.Play(m_soundData.gravityChangeSound);
		}

		private SoundPlayer snd;
		private void Awake() {
			snd = GetComponent<SoundPlayer>();
		}

		private void Start() {
			m_movementController.ListenForJump(OnJump);
			m_movementController.ListenForLand(OnLand);
			m_movementController.ListenForGravityChangesByMe(OnGravityChangedByPlayer);
		}

		private void FixedUpdate() {
			if (landCooldownTick != -1) {
				if (landCooldownTick == m_soundData.landSoundCooldown) {
					landCooldownTick = -1;
				}
				else {
					landCooldownTick++;
				}
			}
		}

		private void Update() {
			if (
				m_movementController.IsGrounded
				&& landCooldownTick == -1
				&& m_movementController.CurrentSpeed > m_soundData.minWalkSpeed
			) {
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

			float currentFallVel = -m_movementController.AdjustedVelocity.y;
			if (m_movementController.IsGrounded || currentFallVel < m_soundData.minFallSpeed) {
				if (fallSource != null) {
					fallSource.Stop();
					fallSource = null;
					fallTick = 0;
				}
			}
			else {
				if (fallTick == m_soundData.minFallTime) {
					float volume = (currentFallVel - m_soundData.minFallSpeed) / (m_soundData.speedForMaxLandSound - m_soundData.minFallSpeed);
					volume = Mathf.Min(volume, 1f) * m_soundData.maxFallSoundVolume;

					if (fallSource == null) {
						fallSource = snd.Play(m_soundData.fallSound, true);
						fallSource.volume = volume;
						fallSource.Play();
					}
					fallSource.volume = volume;
				}
				else {
					fallTick++;
				}
			}
		}
	}
}
