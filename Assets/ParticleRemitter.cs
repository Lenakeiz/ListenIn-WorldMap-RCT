using UnityEngine;
using System.Collections;

public class ParticleRemitter : MonoBehaviour {

    private ParticleSystem glowParticle;
    private ParticleSystem lightParticle;
    private ParticleSystem ringParticle;  

    void Awake()
    {
        glowParticle = transform.Find("Glow").gameObject.GetComponent<ParticleSystem>();
        lightParticle = transform.Find("Light").gameObject.GetComponent<ParticleSystem>();
        ringParticle = transform.Find("Ring").gameObject.GetComponent<ParticleSystem>();
    }
    
    void OnEnable()
    {
        Invoke("InitializeParticles", 5.0f);
    }

    void OnDisable()
    {
        StopAllCoroutines();
        glowParticle.Stop();
        lightParticle.Stop();
        ringParticle.Stop();
    }

    IEnumerator EmitParticles()
    {
        glowParticle.Play();
        lightParticle.Play();
        ringParticle.Play();
        yield return new WaitForSeconds(5);
        if (gameObject.activeInHierarchy)
            StartCoroutine(StopParticles());
    }

    IEnumerator StopParticles()
    {
        glowParticle.Stop();
        lightParticle.Stop();
        ringParticle.Stop();
        yield return new WaitForSeconds(8);
        if (gameObject.activeInHierarchy)
            StartCoroutine(EmitParticles());
    }

    void InitializeParticles()
    {
        if(gameObject.activeInHierarchy)
            StartCoroutine(EmitParticles());
    }
}
