using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private float time = 0f;

    void Update()
    {
        time += Time.deltaTime;
        if (time > .5f)
        {
            print(Vector3.Angle(Vector3.forward, transform.forward)+" "+Quaternion.Euler(0, Vector3.Angle(Vector3.forward, transform.forward),0) * Vector3.forward);
            time = 0;   
        }
    }
}