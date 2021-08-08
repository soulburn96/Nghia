using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using UnityEngine.UI;
public class PowderExtinguisher : MonoBehaviour
{
    private Image powderMeter = null;
    private InteractableInputs input;
    [SerializeField] ExtinguisherFX efx;
    [SerializeField] float lifeTime = 5f;
    [SerializeField] float time = 0f;
    bool isPlaying ;
    bool updatingPowderMeter = false;
    bool outOfPowder;
    private void Start()
    {
        powderMeter = GetComponentInChildren<Image>();
        powderMeter.GetComponentInParent<Canvas>().worldCamera = Camera.main;
        input = GetComponent<InteractableInputs>();
        isPlaying = false;
        outOfPowder = false;
    }
    private void Update()
    {
        if (outOfPowder)
            return;
        if (time > lifeTime)
        {
            input.SetInputPressEventLocked(true);
            efx.StopVFX();
            outOfPowder = true;
        }
        if (!isPlaying)
            return;
        if (isPlaying)
        {
            time += Time.deltaTime;
            if (powderMeter.rectTransform.localScale.x > 0)
            {
                powderMeter.rectTransform.localScale = new Vector3(powderMeter.rectTransform.localScale.x - (Time.deltaTime / lifeTime /** powderMeter.rectTransform.localScale.x*/), powderMeter.rectTransform.localScale.y, powderMeter.rectTransform.localScale.z);

            }
            else
            {
                powderMeter.rectTransform.localScale = new Vector3(0, powderMeter.rectTransform.localScale.y, powderMeter.rectTransform.localScale.z);
            }
        }
    }

    private IEnumerator UpdatePowderMeter()
    {
        if (updatingPowderMeter)
        {
            yield return null;
        }
        else
        {
            updatingPowderMeter = true;
            if (powderMeter.rectTransform.localScale.x > 0)
            {
                powderMeter.rectTransform.localScale = new Vector3(powderMeter.rectTransform.localScale.x - (time / lifeTime * powderMeter.rectTransform.localScale.x), powderMeter.rectTransform.localScale.y, powderMeter.rectTransform.localScale.z);

            }
            else
            {
                powderMeter.rectTransform.localScale = new Vector3(0, powderMeter.rectTransform.localScale.y, powderMeter.rectTransform.localScale.z);
            }
            yield return new WaitForSeconds(0.1f);
            updatingPowderMeter = false;
        }

    }

    public void LifeTimerStart()
    {
        isPlaying = true;
    }
    public void LifeTimerStop()
    {
        isPlaying = false;
    }
}
