using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sound {
	[CreateAssetMenu]
	public class SoundData : ScriptableObject {
		public AudioClip clip;
		public float volume = 1;
		public int priority = 128;
		public bool loop;
	}
}
