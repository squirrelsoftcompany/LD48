using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightIntensity : MonoBehaviour
{

    public GameObject mPlayer;
    public float maxIntensity = 100000.0f;
    public float minIntensity = 0.0f;
    public float maxDistance = 1000.0f;
    public float minDistance = 20.0f;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        float dist = Vector3.Distance(mPlayer.transform.position, transform.position - new Vector3(0,100,0));

        float distanceRange = Mathf.InverseLerp(minDistance, maxDistance, dist);
        float intensity = Mathf.Lerp(Mathf.Sqrt(minIntensity), Mathf.Sqrt(maxIntensity), distanceRange);
        GetComponent<Light>().intensity = intensity * intensity;
    }
}
