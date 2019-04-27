using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class WorldDayNightCycle : MonoBehaviour
{
    public float xSunDirection = 0.0f;
    public float ySunDirection = 0.2f;
    public float zSunDirection = 0.2f;
    public AnimationCurve xDirectionCurve = new AnimationCurve();
    public AnimationCurve zDirectionCurve = new AnimationCurve();
    public AnimationCurve yDirectionCurve = new AnimationCurve();
    public AnimationCurve temperatureCurve = new AnimationCurve();
    public AnimationCurve tintCurve = new AnimationCurve();
    public AnimationCurve ambientR = new AnimationCurve();
    public AnimationCurve ambientG = new AnimationCurve();
    public AnimationCurve ambientB = new AnimationCurve();
    public float timeOfDay = 0.0f;
    public FloatParameter temperatur = new FloatParameter();
    public FloatParameter tint = new FloatParameter();
    public PostProcessVolume globalPostProcessing;
    public ColorGrading grading;
    public Vignette vignete;
    public float startVignete;

    public GameManager gameManager;
    public GameWorld gameWorld;
    public CloudController cloudController;

    // Start is called before the first frame update
    void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        globalPostProcessing = GameObject.Find("PostProcessingLayer").GetComponent<PostProcessVolume>();
        gameWorld = transform.parent.gameObject.GetComponent<GameWorld>();
        globalPostProcessing.profile.TryGetSettings(out grading);
        globalPostProcessing.profile.TryGetSettings(out vignete);
        temperatur = grading.temperature;
        tint = grading.tint;
        startVignete = vignete.intensity.value;
        cloudController = GameObject.Find("cloudMask").GetComponent<CloudController>();

        if (timeOfDay > 0 && timeOfDay < 3)
        {
            startVignete = timeOfDay / 3.0f * 0.119f + 0.382f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        RenderSettings.ambientLight = new Color(ambientR.Evaluate(timeOfDay), ambientG.Evaluate(timeOfDay), ambientB.Evaluate(timeOfDay));
        //temperatur.Override(temperatureCurve.Evaluate(timeOfDay));
        //tint.Override(tintCurve.Evaluate(timeOfDay));
        xSunDirection = xDirectionCurve.Evaluate(timeOfDay);
        zSunDirection = zDirectionCurve.Evaluate(timeOfDay);
        ySunDirection = yDirectionCurve.Evaluate(timeOfDay);
        timeOfDay += Time.deltaTime * gameManager.timeMultiplier / 2.0f;
        if (timeOfDay > 60)
            timeOfDay = 0;
        if (timeOfDay < 30)
        {
            gameWorld.isNight = false;
            var em = transform.parent.GetChild(1).GetComponent<ParticleSystem>().emission;
            em.rateOverTime = 0;
        }
        else
        {
            gameWorld.isNight = true;
            var em = transform.parent.GetChild(1).GetComponent<ParticleSystem>().emission;
            em.rateOverTime = ( 1 - Mathf.Abs(timeOfDay - 30 - 15) / 15.0f) * 20.0f;
        }
        if (timeOfDay > 27 && timeOfDay < 30)
        {
            startVignete = Mathf.Clamp(startVignete + Time.deltaTime * 0.018f, 0.382f, 0.5f);
        }
        else if (timeOfDay > 0 && timeOfDay < 3)
        {
            startVignete = Mathf.Clamp(startVignete - Time.deltaTime * 0.018f, 0.382f, 0.5f);
        }
        vignete.intensity.Override(startVignete);
        cloudController.SetSunDir(new Vector3(xSunDirection, ySunDirection, zSunDirection));
        GetComponent<MeshRenderer>().material.SetVector("_SunDir", new Vector4(xSunDirection, ySunDirection, zSunDirection));
        grading.tint = tint;
        grading.temperature = temperatur;
    }
}
