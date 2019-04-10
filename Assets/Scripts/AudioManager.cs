using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	#region Singleton

	public static AudioManager Instance { get; private set; }

	private void Awake()
	{
		AudioManager.Instance = this;
	}

	#endregion

	[SerializeField] private AudioSource bgmAudioSource;
	[SerializeField] private AudioSource sfxAudioSource;

	[System.Serializable]
	private struct AudioClipInfo
	{
		public string name;
		public AudioClip clip;
	}

	[SerializeField] private AudioClipInfo[] sfxClipInfos;
	private Dictionary<string, AudioClip> sfxDictionary;

	private void Start()
	{
		this.BuildDictionary();
	}

	private void BuildDictionary()
	{
		// Assert there are no duplicate entries
		Debug.Assert(this.sfxClipInfos.Length == this.sfxClipInfos.Distinct().Count());

		// Build the dictionary
		this.sfxDictionary = new Dictionary<string, AudioClip>();
		foreach (AudioClipInfo info in this.sfxClipInfos)
			this.sfxDictionary[info.name] = info.clip;
	}

	public void PlayBGM(AudioClip bgmClip)
	{
		this.bgmAudioSource.clip = bgmClip;
		this.bgmAudioSource.Play();
	}

	public void PlaySFX(string name)
	{
		AudioClip clip = this.sfxDictionary[name];

		this.sfxAudioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
		this.sfxAudioSource.PlayOneShot(clip);
	}
}
