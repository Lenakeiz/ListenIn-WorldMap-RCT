using UnityEngine;
using System.Collections;

using ListenIn;

public class ILevel : MonoBehaviour {

	public bool startingLevel = false;
	public bool endingLevel = false;

	public string musicSound = string.Empty;
	public string musicEnd = string.Empty;

    public LevelDifficulty currDifficulty = LevelDifficulty.Easy;

	public void PlayMusic(string musicToPlay, bool loop )
	{
        if (loop)
        {
            Debug.Log("ILevel: play level music");
        }
        else
        {
            Debug.Log("ILevel: play end music");
        }

		if(musicSound != string.Empty)
		{
			AudioClipInfo aci;
			aci.delayAtStart = 0.0f;
			aci.isLoop = loop;
			aci.useDefaultDBLevel = true;
			aci.clipTag = string.Empty;
			Camera.main.GetComponent<SoundManager>().Play((Resources.Load(musicToPlay) as AudioClip), ChannelType.LevelMusic, aci);
		}
	}

	public void StopMusic()
	{
		Camera.main.GetComponent<SoundManager>().Stop(ChannelType.LevelMusic);
	}

	public virtual void InitLevel()
	{
		PlayMusic(musicSound, true);
	}

	public  virtual void EndLevel()
	{
		StopMusic();
		PlayMusic(musicEnd, false);
	}

	public virtual void UpdateLevel()
	{

	}
    protected virtual void PrepareEasyLevel()
    {
        GameObject go = GameObject.FindGameObjectWithTag("MediumLevel");
        if (go != null)
        {
            go.SetActive(false);
            go = GameObject.FindGameObjectWithTag("HardLevel");
            go.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Easy Level game objects not found");
        }

    }

    protected virtual void PrepareMediumLevel()
    {
        
        GameObject go = GameObject.FindGameObjectWithTag("EasyLevel");
        if (go != null)
        {
            go.SetActive(false);
            go = GameObject.FindGameObjectWithTag("HardLevel");
            go.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Medium Level game objects not found");
        }
    }

    protected virtual void PrepareHardLevel()
    {
        GameObject go = GameObject.FindGameObjectWithTag("EasyLevel");
        if (go != null)
        {
            go.SetActive(false);
            go = GameObject.FindGameObjectWithTag("MediumLevel");
            go.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Hard Level game objects not found");
        }
    }

    protected virtual void Start()
	{
        switch (currDifficulty)
        {
            case LevelDifficulty.Easy:
                PrepareEasyLevel();
                break;
            case LevelDifficulty.Medium:
                PrepareMediumLevel();
                break;
            case LevelDifficulty.Hard:
                PrepareHardLevel();
                break;
            default:
                break;
        }
    }

	// Update is called once per frame
	protected virtual void Update () {

		if(startingLevel)
		{
			startingLevel = false;
			InitLevel ();
		}
		else if(endingLevel)
		{
			endingLevel = false;
			EndLevel();
		}

		UpdateLevel();

	}
}
