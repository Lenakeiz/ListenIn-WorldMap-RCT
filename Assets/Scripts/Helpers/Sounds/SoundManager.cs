using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

public enum SoundsType{Ambient = 0, Effect = 1	};

[System.Serializable]
public class CustomSound
{
	public string Name;
	public SoundsType Type;
	public AudioClip Clip;
}

public enum ChannelType
{
	VoiceText = 0,
	PhoneVoice,
	BackgroundNoise,
	SoundFx,
	LevelMusic,
	LevelEffects,
	CoinEffects
};

public class ChannelInfo
{
	public float defaultDBLevel;
	public float settedDBLevel;
	public AudioMixerGroup audioMixerGroup;
	//Can add more parameters from channel here
}

public struct AudioClipInfo
{
	public bool useDefaultDBLevel;
	public bool isLoop;
	public float delayAtStart;
	public string clipTag;
}

public class SoundManager : MonoBehaviour {
	
	bool sourceFound;

	public AudioMixer audioMixer;

	public AudioMixerGroup[] audioMixerGroups;

	public List<CustomSound> sounds;
    private List<GameObject> playingList;

	private Dictionary<string,ChannelInfo> channelInfos;
	private Dictionary<string,AudioClip> playedSounds;

	// Use this for initialization
	void Start () {
		playingList = new List<GameObject>();
		playedSounds = new Dictionary<string,AudioClip>();
		channelInfos = new Dictionary<string, ChannelInfo>();

		channelInfos.Add(ChannelType.VoiceText.ToString(), new ChannelInfo(){defaultDBLevel = 10.0f, settedDBLevel = 10.0f, audioMixerGroup = audioMixerGroups[0]});
		channelInfos.Add(ChannelType.PhoneVoice.ToString(), new ChannelInfo(){defaultDBLevel = 10.0f, settedDBLevel = 10.0f, audioMixerGroup = audioMixerGroups[1]});
		channelInfos.Add(ChannelType.BackgroundNoise.ToString(), new ChannelInfo(){defaultDBLevel = 0.0f, settedDBLevel = 0.0f, audioMixerGroup = audioMixerGroups[2]});
		channelInfos.Add(ChannelType.SoundFx.ToString(), new ChannelInfo(){defaultDBLevel = -5.0f, settedDBLevel = -5.0f, audioMixerGroup = audioMixerGroups[3]});

		channelInfos.Add(ChannelType.LevelMusic.ToString(), new ChannelInfo(){defaultDBLevel = -5.0f, settedDBLevel = 0.0f, audioMixerGroup = audioMixerGroups[5]});
		channelInfos.Add(ChannelType.LevelEffects.ToString(), new ChannelInfo(){defaultDBLevel = -2.0f, settedDBLevel = -2.0f, audioMixerGroup = audioMixerGroups[6]});
		channelInfos.Add(ChannelType.CoinEffects.ToString(), new ChannelInfo(){defaultDBLevel = 7.0f, settedDBLevel = 10.0f, audioMixerGroup = audioMixerGroups[7]});
	}
	
	// Update is called once per frame
	void Update () {
		for (int i = 0; i < playingList.Count; i++)
		{
			if(!playingList[i].GetComponent<AudioSource>().isPlaying)
			{
				Destroy(playingList[i]);
				playingList.RemoveAt(i);
				i--;
			}
		}

	}

	void Awake(){

	}

	/// <summary>
	/// Sets the DB attenuation of the channel.
	/// </summary>
	/// <param name="channeltype">Channeltype.</param>
	/// <param name="value">Value.</param>
	public void SetChannelLevel(ChannelType channeltype, float value)
	{
		ChannelInfo chInfo = null;
		channelInfos.TryGetValue(channeltype.ToString(), out chInfo);
		
		if(chInfo != null)
		{
			chInfo.settedDBLevel = value;
		}
		else
		{
			Debug.LogError("Channelinfo not found");
		}
	}

	/// <summary>
	/// Get the DB attenuation of the channel.
	/// </summary>
	/// <param name="channeltype">Channeltype.</param>
	public float GetChannelLevel(ChannelType channeltype)
	{
		ChannelInfo chInfo = null;
		channelInfos.TryGetValue(channeltype.ToString(), out chInfo);
		
		if(chInfo != null)
		{
			// !!! check with Andrea whether to return defaultDBLevel OR settedDBLevel
			return (chInfo.settedDBLevel);
		}
		else
		{
			Debug.LogError("Channelinfo not found");
			return 0.0f;
		}
	}

	/// <summary>
	/// Play the specified audioClip on specified channel type. This function is meant to be used for single sound occurences.
	/// Use default level will set the attenuation to its default level.
	/// </summary>
	/// <param name="audioClip">Audio clip.</param>
	/// <param name="clipName">Clip name.</param>
	/// <param name="channeltype">Channeltype.</param>
	public void Play(AudioClip audioClip, ChannelType channeltype, AudioClipInfo clipInfo)
	{
		GameObject go = new GameObject("GameSound");
		go.tag = channeltype.ToString();
		go.transform.SetParent(gameObject.transform, false);
		AudioSource source = go.AddComponent<AudioSource>();
		source.playOnAwake = false;

		source.clip = audioClip;

		float lvl = 0.0f;
		ChannelInfo chInfo = null;
		channelInfos.TryGetValue(channeltype.ToString(), out chInfo);
		
		if(chInfo != null)
		{
			lvl = clipInfo.useDefaultDBLevel ? chInfo.defaultDBLevel : chInfo.settedDBLevel;
		}
		else
		{
			Debug.LogError("Channel info not found");
		}

		audioMixer.SetFloat(channeltype.ToString(),lvl);
		source.outputAudioMixerGroup = chInfo.audioMixerGroup;

		source.loop = clipInfo.isLoop;

		source.PlayDelayed(clipInfo.delayAtStart);
		playingList.Add(go);

	}

	/// <summary>
	/// Play the specified clip inside specified folder on given channel type. This is meant to be used for sounds played often as the clips created are saved into a dictionary.
	/// Clip name must be unique. Use default level will set the attenuation to its default level.
	/// </summary>
	/// <param name="folderPath">Folder path.</param>
	/// <param name="clipname">Clipname.</param>
	/// <param name="channeltype">Channeltype.</param>
	public void Play(string folderPath, string clipname, ChannelType channeltype, AudioClipInfo clipInfo, bool saveInDictionary)
	{
		AudioClip clip = null;
		GameObject go = new GameObject("GameSound");
		go.tag = channeltype.ToString();
		go.transform.SetParent(gameObject.transform, false);
		AudioSource source = go.AddComponent<AudioSource>();
		source.playOnAwake = false;

		if(playedSounds.ContainsKey(clipname))
		{
			playedSounds.TryGetValue(clipname, out clip);
		}
		else 
		{
			clip = Resources.Load(folderPath + "/" + clipname) as AudioClip;
			if(saveInDictionary)
				playedSounds.Add(clipname,clip);
		}

		float lvl = 0.0f;
		ChannelInfo chInfo = null;
		channelInfos.TryGetValue(channeltype.ToString(), out chInfo);
		
		if(chInfo != null)
		{
			lvl = clipInfo.useDefaultDBLevel ? chInfo.defaultDBLevel : chInfo.settedDBLevel;
		}
		else
		{
			Debug.LogError("Channelinfo not found");
		}

		audioMixer.SetFloat(channeltype.ToString(),lvl);
		source.outputAudioMixerGroup = chInfo.audioMixerGroup;

		source.loop = clipInfo.isLoop;

		source.PlayDelayed(clipInfo.delayAtStart);
		playingList.Add(go);

	}

	public void Play(string folderPath, string clipname)
	{
		AudioClip clip = null;
		GameObject go = new GameObject("GameSound");
		//go.transform.SetParent(gameObject.transform, false);
		AudioSource source = go.AddComponent<AudioSource>();
		source.playOnAwake = false;
		
		if(playedSounds.ContainsKey(clipname))
		{
			playedSounds.TryGetValue(clipname, out clip);
		}
		else 
		{
			clip = Resources.Load(folderPath + "/" + clipname) as AudioClip;
			playedSounds.Add(clipname,clip);
		}

		//last element is master (must set a proper channel Info)
		//source.outputAudioMixerGroup = audioMixerGroups[audioMixerGroups.Length - 1];
		
		source.loop = false;
		
		source.Play();
		playingList.Add(go);

	}

	/// <summary>
	/// Stop all the clips being played in a particular channel.
	/// </summary>
	/// <param name="channel">Channel.</param>
	public void Stop(ChannelType channel)
	{
		GameObject[] soundsInChannel = GameObject.FindGameObjectsWithTag(channel.ToString());

		for (int i = 0; i < soundsInChannel.Length; i++) {
			if(soundsInChannel[i].GetComponent<AudioSource>().isPlaying)
			{
				soundsInChannel[i].GetComponent<AudioSource>().Stop();
			}			
		}
	}

	/// <summary>
	/// Play a sound given his name, clip must be exposed into the object.
	/// </summary>
	/// <param name="clipName">Clip name.</param>
	public void Play(string clipName, float delay = 0.0f)
	{
		CustomSound cs = sounds.Find(x => x.Name == clipName);
		if (cs != null)
		{
			GameObject go = new GameObject("GameSound");
			AudioSource source = go.AddComponent<AudioSource>();
			source.clip = cs.Clip;
			source.playOnAwake = true;
			source.PlayDelayed(delay);
			go.transform.SetParent(gameObject.transform, false);
			sourceFound = true;
			playingList.Add(go);
		}
		else
		{
			//Try to add to custom sounds
		}
		if (sourceFound == false) 
		{
			Debug.Log("Audio Clip has not been found");
		}
		
		sourceFound = false;
	}
}

