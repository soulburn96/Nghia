using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ExtinguisherFX : MonoBehaviour
{
    private VisualEffect extinguisherVFX;
    private float currentParticles;
    private float maxParticles;
    [SerializeField] float particleSpawnSpeed;
    [SerializeField] private ParticleSoundEffect particleSFX;
    // Start is called before the first frame update
    void Start()
    {
        extinguisherVFX = GetComponent<VisualEffect>();
        maxParticles = extinguisherVFX.GetFloat("Particles");
        extinguisherVFX.SetFloat("Particles", 0);
    }

    // Update is called once per frame
    public void StartVFX()
    {
        currentParticles = extinguisherVFX.GetFloat("Particles");

        if (currentParticles < maxParticles)
        {
            extinguisherVFX.SetFloat("Particles", currentParticles + particleSpawnSpeed);
        }

        else
        {
            extinguisherVFX.SetFloat("Particles", maxParticles);
        }
        currentParticles = extinguisherVFX.GetFloat("Particles");
        particleSFX.PlaySound();
    }

    public void StopVFX()
    {
        if (currentParticles != 0)
        {
            extinguisherVFX.SetFloat("Particles", 0);
            currentParticles = 0;
        }
        particleSFX.StopSound();
    }
}
