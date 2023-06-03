using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sound {
	public class SoundPlayer : MonoBehaviour {
		[SerializeField] private int m_maxSources = 10;

		private AudioSource[] playingSources;

		private void Awake() {
			playingSources = new AudioSource[m_maxSources];
		}

		private void FixedUpdate() {
			for (int i = 0; i < playingSources.Length; i++) {
				AudioSource source = playingSources[i];
				if (source == null) continue;

				if (! source.isPlaying) {
					playingSources[i] = null;
					Destroy(source);
				}
			}
		}

		public AudioSource Play(SoundData sound, bool playManually = false) {
			AudioSource source = null;
			for (int i = 0; i < playingSources.Length; i++) {
				source = playingSources[i];
				if (source == null) {
					source = gameObject.AddComponent<AudioSource>();
					playingSources[i] = source;
					break;
				}
			}

			if (source == null) return null;

			source.clip = sound.clip;
			source.volume = sound.volume;
			source.priority = sound.priority;
			source.loop = sound.loop;
			if (! playManually) source.Play();

			return source;
		}
	}
}
