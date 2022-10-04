using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ColorChange : MonoBehaviour
{
    private Gradient gradient = new Gradient();
    private float time = 0.0f;
    public float duration = 15.0f;
    public Material pointMatE;
    public GameObject restObject;
    private Toggle restToggle;
    private float speed;
    public GameObject speedLimit;

    // Start is called before the first frame update
    void Start()
    {

        restToggle = restObject.GetComponent<Toggle>();

        GradientColorKey[] colorKey = new GradientColorKey[2];  //sets color gradient for transition
        GradientAlphaKey[] alphaKey = new GradientAlphaKey[2];
        colorKey[0].color = Color.green; //new Color(Color.green.r, Color.green.g, Color.green.b, 1f);
        colorKey[0].time = 0.0f;
        colorKey[1].color = Color.red; //new Color(Color.red.r, Color.red.g, Color.red.b, 1f);
        colorKey[1].time = 1.0f;

        alphaKey[0].alpha = 1.0f;
        alphaKey[0].time = 0.0f;
        alphaKey[1].alpha = 0.0f;
        alphaKey[1].time = 1.0f;

        gradient.SetKeys(colorKey, alphaKey);
    }
    void OnEnable()
    {
        gameObject.GetComponent<Renderer>().material = pointMatE;
        time = 0f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float value = Mathf.Lerp(0f, 1f, time);
        time += Time.deltaTime / duration;
        if (!restToggle.isOn)
        {
            Color color = gradient.Evaluate(value);     //finds the appropriate color at a set time
            gameObject.GetComponent<Renderer>().material.color = color;
        }
        else
        {
            if (speed <= float.Parse(speedLimit.GetComponent<TMP_InputField>().text)) gameObject.GetComponent<Renderer>().material.color = Color.blue;
            else gameObject.GetComponent<Renderer>().material.color = Color.red;
        }
    }

    public void SetSpeed(float unitSpeed)
    {
        speed = unitSpeed;
    }

    /*private void OnDisable()
    {
        gameObject.GetComponent<Renderer>().material = pointMatD;
    }*/
}
