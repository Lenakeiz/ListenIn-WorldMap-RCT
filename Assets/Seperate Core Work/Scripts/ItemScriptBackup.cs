using UnityEngine;
using System.Collections;

public class ItemScript : MonoBehaviour {

	GameObject m_goImage;
	Animator m_anim;

	//----------------------------------------------------------------------------------------------------
	// Use this for initialization
	//----------------------------------------------------------------------------------------------------
	void Start () 
	{
		m_goImage = transform.Find("Image").gameObject;
		m_anim = GetComponent<Animator> ();
	}	

	//----------------------------------------------------------------------------------------------------
	// Update is called once per frame
	//----------------------------------------------------------------------------------------------------
	void Update () {
	
	}

	//----------------------------------------------------------------------------------------------------
	// ShowItem: show / hide item using animation
	//----------------------------------------------------------------------------------------------------
	public void ShowItem(bool bShow)
	{
		if (!m_goImage)
			m_goImage = transform.Find("Image").gameObject;

		m_goImage.GetComponent<SpriteRenderer> ().enabled = bShow;

		if (!m_anim)
			m_anim = GetComponent<Animator> ();			
		
		m_anim.SetBool ("bSpawn", bShow); 
	}
}
