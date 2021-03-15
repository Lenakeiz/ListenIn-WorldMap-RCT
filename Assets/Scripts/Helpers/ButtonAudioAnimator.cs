using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonAudioAnimator : MonoBehaviour {

    [SerializeField]
    private Image m_image;

    [SerializeField]
    private Sprite[] m_SpriteForAnimation;

    [SerializeField]
    private float SingleSpriteAnimationDuration = 0.4f;

    [SerializeField]
    private AudioClip m_audioClip;
    private float m_audioLength;

    private bool isPlaying = false;

    private float lowerAlpha = 0.2f;
    private float upperAlpha  = 1.0f;
    bool Updirection = false;
    [SerializeField]
    float speedAlpha = 1.0f;

    [SerializeField]
    private float rotationTimeStep = 0.3f;
    bool UpdirectionRot = false;

    [SerializeField]
    private SoundManager m_soundManager;
	// Use this for initialization
	void Start () {
        //StartCoroutine(AudioSpriteAnimation(10.0f));
        m_soundManager = Camera.main.GetComponent<SoundManager>();
        m_audioLength = m_audioClip.length;
        StartCoroutine(AnimateTransparency());
        StartCoroutine(AnimateRotation());
    }

    void OnDisable()
    {
        StopAllCoroutines();
        ResetValues();
        m_soundManager.Stop(ChannelType.VoiceText);
    }

    void OnEnable()
    {
        StopAllCoroutines();
        ResetValues();
        StartCoroutine(AnimateTransparency());
        StartCoroutine(AnimateRotation());
    }

	// Update is called once per frame
	void Update () {
	
	}
    
    private IEnumerator AudioSpriteAnimation(float duration)
    {
        float t = 0;
        float st = 0;
        int s_count = 0;
        while (t < duration)
        {
            st = 0;
            while (st < SingleSpriteAnimationDuration)
            {
                yield return new WaitForEndOfFrame();
                t += Time.deltaTime;
                st += Time.deltaTime;
            }
            s_count = (s_count + 1) % m_SpriteForAnimation.Length;
            m_image.sprite = m_SpriteForAnimation[s_count];
        }

        m_image.sprite = m_SpriteForAnimation[0];
        isPlaying = false;
        yield return new WaitForEndOfFrame();
        StartCoroutine(AnimateTransparency());
        StartCoroutine(AnimateRotation());
    }

    private IEnumerator AnimateTransparency()
    {
        Color ta = m_image.color;
        Color ca = ta;
        ta.a = Updirection ? upperAlpha : lowerAlpha;

        float t = 0;
        do
        {
            t += Time.deltaTime * speedAlpha;
            Color lerpCol = Color.Lerp(ca, ta, t);
            m_image.color = lerpCol;
            yield return new WaitForEndOfFrame();

        } while (t <= 1.0f);

        Updirection = !Updirection;
        StartCoroutine(AnimateTransparency());

    }

    private IEnumerator AnimateRotation()
    {
        Quaternion targetRotation = Quaternion.AngleAxis(10.0f, Vector3.forward);
        transform.rotation = targetRotation;
        yield return new WaitForSeconds(rotationTimeStep);
        transform.rotation = Quaternion.identity;
        yield return new WaitForSeconds(rotationTimeStep);
        targetRotation = Quaternion.AngleAxis(-10.0f, Vector3.forward);
        transform.rotation = targetRotation;
        yield return new WaitForSeconds(rotationTimeStep);
        transform.rotation = Quaternion.identity;
        yield return new WaitForSeconds(rotationTimeStep);
        StartCoroutine(AnimateRotation());
    }

    private void ResetValues()
    {
        Color c = m_image.color;
        c.a = 1.0f;
        m_image.color = c;
        transform.rotation = Quaternion.identity;
        m_image.sprite = m_SpriteForAnimation[0];
    }

    public void PlayInstructions()
    {
        if (isPlaying)
            return;

        isPlaying = true;
        StopAllCoroutines();
        ResetValues();

        AudioClipInfo aci;
        aci.isLoop = false;
        aci.delayAtStart = 0.0f;
        aci.useDefaultDBLevel = true;
        aci.clipTag = string.Empty;

        m_soundManager.Play(m_audioClip, ChannelType.VoiceText, aci);
        StartCoroutine(AudioSpriteAnimation(m_audioLength));
    }

}
