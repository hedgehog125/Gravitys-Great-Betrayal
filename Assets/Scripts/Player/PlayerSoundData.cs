using Sound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player {
	[CreateAssetMenu(menuName = "Data/Player/Sounds")]
	public class PlayerSoundData : ScriptableObject {
		[Header("Walking")]
		public List<SoundData> stepSounds;
		public float minWalkSpeed;
		public float minRelativeSpeed; // Scale the minimum speed so it's always at least this much
		public float minStepSoundGap;
		public float maxStepVolume;

		[Header("Jumping")]
		public List<SoundData> jumpSounds;

		[Header("Landing")]
		public List<SoundData> landSounds;
		public int landSoundCooldown;
		public float minLandVelocity;
		public float maxLandVolume;
		public float minRelativeLandSpeed;
		public float speedForMaxLandSound;
	}
}
