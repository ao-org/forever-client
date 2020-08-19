using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WiggleScript : MonoBehaviour
{
    private float _nextActionTime = 0.1f;
    private Light _light;
    private float _period = 0.1f;

    void Start()
    {
        StartCoroutine("lightWiggle");
        _light = GetComponent<Light>();
    }

    IEnumerator lightWiggle()
    {
        for (; ; )
        {
            float _intensity = Mathf.Lerp(Random.Range(1.5f, 3f), Random.Range(1.5f, 3f), Time.time);
            _light.intensity = _intensity;

            yield return new WaitForSeconds(0.1f);
        }
    }
}
