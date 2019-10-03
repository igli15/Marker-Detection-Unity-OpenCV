using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glitch : MonoBehaviour
{
    public ChromaticEffect chromticEffect;
    public float timeBetweenFrames = 0.1f;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            DoGlitch();
        }
    }

    public void DoGlitch()
    {
        chromticEffect.effectDistance = 0;
        Invoke("StopEffect", timeBetweenFrames);
    }

    private void StopEffect()
    {
        chromticEffect.effectDistance = 2000000;
    }
}
