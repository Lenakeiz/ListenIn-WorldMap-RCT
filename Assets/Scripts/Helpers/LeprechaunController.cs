using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;



public class LeprechaunController : MonoBehaviour, AnimationInterface {

	private Animator animator;

	[System.Serializable]
	public struct AnimationClipStat
	{
		public string name;
		public AnimationClip clip;
	}
	public AnimationClipStat[] clipStats;

    public SpriteRenderer globe;

	private Dictionary<string,AnimationClip> clips;

	public void Play (string triggerName)
	{
		if(animator != null)
		{
			animator.SetTrigger(triggerName);
		}
	}

	public void ThrowCards()
	{

	}

    public void AdjustLayers()
    {
        globe.sortingOrder = 30;
    }

	public float AnimationLength (string animationName)
	{
		AnimationClip ac;
		float length = 0;
		if(clips != null && clips.ContainsKey(animationName))
		{
			clips.TryGetValue(animationName, out ac);
			length = ac.length;
		}

		return length;

	}

	void Start () {

		animator = GetComponent<Animator>();
		clips = new Dictionary<string,AnimationClip>();

		if (clipStats != null && clipStats.Length != 0)
		{
			for (int i = 0; i < clipStats.Length; i++) {
				clips.Add(clipStats[i].name,clipStats[i].clip);
			}
		}

	}

	void Update () {
	
	}
}
