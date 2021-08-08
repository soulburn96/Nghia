using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FireEmitter
{
    public ParticleSystem.EmissionModule emitter;
    public float amountOfParticles;

    public FireEmitter(ParticleSystem.EmissionModule e, float a)
    {
        emitter = e;
        amountOfParticles = a;
    }
}

public class Fire : MonoBehaviour
{
    [Header("Fire Stats")]
    [SerializeField] private float startHealth = 100f;
    [SerializeField] private float recoverRate = 10f;
    [SerializeField] private ParticleSystem fireParticles;

    [Header("Events")]
    public UnityEvent onFireExtinguished;

    private List<FireEmitter> fireEmitters = new List<FireEmitter>();
    private bool fireAlive;
    private float currentHealth;
    private float[] defaultAmountOfParticles;

    void Start()
    {        
        StartFire();
    }

    public void StartFire()
    {
        if (fireParticles == null || fireAlive == true)
            return;

        fireAlive = true;
        StartCoroutine(FireRecoverLoop());

        List<ParticleSystem> allFireParticleSystems = new List<ParticleSystem>(fireParticles.GetComponentsInChildren<ParticleSystem>());

        if (fireEmitters == null)
        {
            foreach (ParticleSystem subParticles in allFireParticleSystems)
                fireEmitters.Add(new FireEmitter(subParticles.emission, subParticles.emission.rateOverTimeMultiplier));

            for (int i = 0; i < fireEmitters.Count; i++)
            {
                defaultAmountOfParticles[i] = fireEmitters[i].amountOfParticles;
            }
        }

        else
        {
            for (int i = 0; i < fireEmitters.Count; i++)
                {
                    fireEmitters[i].amountOfParticles = defaultAmountOfParticles[i];
                }
            fireParticles.Play();
        }


        currentHealth = startHealth;

    }

    void StopFire()
    {
        if (fireParticles == null || fireAlive == false)
            return;

        fireAlive = false;
        fireParticles.Stop();

        if (onFireExtinguished != null)
            onFireExtinguished.Invoke();
    }

    void UpdateParticleAmounts()
    {
        foreach (FireEmitter emitter in fireEmitters)
            emitter.emitter.rateOverTimeMultiplier = (currentHealth / startHealth) * emitter.amountOfParticles;
    }

    public void ReduceFireHealth(float healthReduction)
    {
        if (fireParticles == null || fireAlive == false)
            return;

        currentHealth -= healthReduction;
        UpdateParticleAmounts();

        if (currentHealth < 0f)
        {
            currentHealth = 0f;
            StopFire();
        }
    }

    IEnumerator FireRecoverLoop()
    {
        while (fireAlive == true)
        {
            if (currentHealth < startHealth)
            {
                currentHealth += recoverRate * Time.deltaTime;
                
                if (currentHealth > startHealth)
                    currentHealth = startHealth;
            }
            UpdateParticleAmounts();
            yield return null;
        }
    }
}
