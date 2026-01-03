using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VolumeClip {
	[SerializeField] AudioClip clip;
	[SerializeField] float volume = 1f;

	public AudioClip Clip {
		get {
			return this.clip;
		}
		set {
			clip = value;
		}
	}

	public float Volume {
		get {
			return this.volume;
		}
		set {
			volume = value;
		}
	}

	public void PlayFrom(AudioSource source) {
		source.PlayOneShot (clip, volume);
	}
}
