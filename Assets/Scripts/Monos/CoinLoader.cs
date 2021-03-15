using UnityEngine;
using System.Collections;

public class CoinLoader : MonoBehaviour {

	public float rotation;
	public float strenghtYComponent;
	public float strenghtXComponent;
	public bool chargeInCannon = false;

	public float checkRotation;
	
	private Rigidbody2D body;

	public void ChargeIntoCannon(Vector3[] positions)
	{
        iTween.MoveTo(this.gameObject, iTween.Hash("path", positions, "time", 2, "easetype", iTween.EaseType.easeOutCubic,"oncomplete", "FinishedTransition"));
        
        //PREVIOUS CODE
		//if(!chargeInCannon)
		//{
		//	chargeInCannon = true;
		//}
	}

    public void FinishedTransition()
    {
        GameObject obj = this.gameObject.transform.Find("2D_LightCharge").gameObject;
        Rigidbody2D body = GetComponent<Rigidbody2D>();
        body.simulated = false;
        obj.SetActive(true);
        Invoke("SelfDestruction", 1.0f);
    }

    public void SelfDestruction()
    {
        GameObject.DestroyImmediate(this.gameObject);
    }

    private void PlaySound(string soundResource, bool useDefault = true, float DBValue = 12.0f, ChannelType chType = ChannelType.CoinEffects)
    {
        AudioClipInfo aci;
        aci.delayAtStart = 0.0f;
        aci.isLoop = false;
        aci.useDefaultDBLevel = useDefault;
        aci.clipTag = string.Empty;

        Camera.main.GetComponent<SoundManager>().SetChannelLevel(chType, DBValue);
        Camera.main.GetComponent<SoundManager>().Play((Resources.Load(soundResource) as AudioClip), chType, aci);

    }

    // Use this for initialization
    void Start () {
		body = gameObject.GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
		checkRotation = body.angularVelocity;
	}

	void FixedUpdate()
	{
		if (chargeInCannon)
		{
			body.gravityScale = 0.0f;
			if(checkRotation > 1500)
			{
				body.AddTorque(rotation * 2, ForceMode2D.Force);
			}
			else
			{
				body.AddTorque(rotation, ForceMode2D.Force);
			}

			if(checkRotation > 3000)
			{
				//chargeInCannon = false;
				body.AddTorque(rotation, ForceMode2D.Force);
				body.AddForce(Vector2.up * strenghtYComponent + Vector2.right * strenghtXComponent, ForceMode2D.Impulse);
			}

		}
	}
}
