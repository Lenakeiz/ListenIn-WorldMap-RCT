using UnityEngine;
using System.Collections;
[RequireComponent(typeof(Animation))]
public class JigsawAnim : MonoBehaviour {
	public Animator[] anim = null;

	private bool loop = false;
	// Use this for initialization
	void Start () {
	
	}

	public void PlaySound()
	{
		AudioClipInfo aci;
		aci.delayAtStart = 0.0f;
		aci.isLoop = false;
		aci.useDefaultDBLevel = false;
		aci.clipTag = string.Empty;
		
		string strAudio = "Sounds/Intro/LI_SFX_Intro";
		Camera.main.GetComponent<SoundManager>().SetChannelLevel(ChannelType.LevelMusic, 1.0f);
		Camera.main.GetComponent<SoundManager>().Play((Resources.Load(strAudio) as AudioClip), ChannelType.LevelMusic,aci);
	}

	public void GoToMainGame()
	{
		StateSplash.Instance.ActivateStartGame();
	}

	// Update is called once per frame
	void Update () {
		if (!loop) {
			for(int i =0; i < anim.Length; i++)
			{
				//anim[i].Play(0);
				anim[i].speed = 0.5f;
			}
		}
		
	}
}
