using UnityEngine;
using DG.Tweening;

public class Candle : Interactable
{
    [Header("Candle Settings")]
    [SerializeField] private Light[] candleLights;
    [SerializeField] private ParticleSystem flameEffect;

    public bool isLit;
    public float[] originalIntensity;

    public System.Action OnExtinguished;
    //public bool IsLit { get; private set; } = true;

    protected override void Awake()
    {
        originalIntensity = new float[candleLights.Length];
        base.Awake();        
        for (int i = 0; i < candleLights.Length; i++)
        {
            originalIntensity[i] = candleLights[i].intensity;
        }
        if(!isLit)
        {
            foreach (Light candleLight in candleLights)
            {
                candleLight.intensity = 0;
            }  
            flameEffect.Stop();
        }
        else
        {

            FindAnyObjectByType<AudioManager>().Play("LitCandleLoop");
        }

    }

    public override void Interact(PlayerController player)
    {
        if (!isLit)
        {
            LightCandle();
        }
        else
        {
            ExtinguishCandle();
        }


    }

    private void LightCandle()
    {
        isLit = true; 
        FindAnyObjectByType<AudioManager>().Play("FireCandle");
        FindAnyObjectByType<AudioManager>().Play("LitCandleLoop");
        for (int i = 0; i < candleLights.Length; i++)
        {
            candleLights[i].DOIntensity(originalIntensity[i], 0.5f);
        }

        flameEffect.Play();
    }

    private void ExtinguishCandle()
    {
        isLit = false;
        OnExtinguished?.Invoke();
        FindAnyObjectByType<AudioManager>().Play("BlowingOutCandle");
        FindAnyObjectByType<AudioManager>().Stop("LitCandleLoop");
        foreach (Light candleLight in candleLights)
        {
            candleLight.DOIntensity(0f, 0.5f);
        }       

        flameEffect.Stop();
    }
}