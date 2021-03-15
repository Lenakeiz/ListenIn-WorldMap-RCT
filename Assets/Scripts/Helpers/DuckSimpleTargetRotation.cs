using UnityEngine;
using System.Collections;

public class DuckSimpleTargetRotation : MonoBehaviour {

    private bool rotate = false;
    private bool activated = false;
    private bool startCountdown = false;

    public TargetManager.Flavor duckFlavor;

    public bool finishedActivation = false;
    public bool enableBack = false;

    public float rotatingTime = 8.0f;

    public float rotationSpeed = 0.5f;
    public float threshold = 0.5f;

    public string soundFolderPath;
    public string[] DuckSounds;

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Coin" && col.relativeVelocity.magnitude > threshold && !activated)
        {
            rotate = true;
        }
    }

    // Use this for initialization
    void Start()
    {

    }

    IEnumerator ScaleRotation(bool active)
    {
        float t = 0.0f;
        Vector3 initialScale = transform.localScale;
        Vector3 finalScale = new Vector3(-1 * transform.localScale.x, transform.localScale.y, transform.localScale.z);
        while (t < 1.0f)
        {
            t += Time.deltaTime * rotationSpeed;
            transform.localScale = Vector3.Lerp(initialScale, finalScale, t);
            yield return new WaitForEndOfFrame();
        }

        finishedActivation = active;
        if (active)
        {
            enableBack = true;
        }
        else
        {
            activated = false;
        }

    }

    void BackToPos()
    {
        PlaySound();
        StartCoroutine(ScaleRotation(false));
    }

    private void PlaySound(bool useDBdefault = true, float lvl = 10.0f)
    {
        AudioClipInfo aci;
        aci.delayAtStart = 0.0f;
        aci.isLoop = false;
        aci.useDefaultDBLevel = useDBdefault;
        aci.clipTag = string.Empty;

        string strAudio = soundFolderPath + "/" + DuckSounds[Random.Range(0, DuckSounds.Length)].ToString();
        Camera.main.GetComponent<SoundManager>().SetChannelLevel(ChannelType.LevelEffects, lvl);
        Camera.main.GetComponent<SoundManager>().Play((Resources.Load(strAudio) as AudioClip), ChannelType.LevelEffects, aci);

    }

    // Update is called once per frame
    void Update()
    {

        if (!activated && rotate)
        {
            activated = true;
            rotate = false;
            PlaySound();
            StartCoroutine(ScaleRotation(true));
        }
        else if (enableBack)
        {
            enableBack = false;
            Invoke("BackToPos", rotatingTime);
        }

    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }
}
