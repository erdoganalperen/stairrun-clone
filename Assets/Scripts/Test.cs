using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private float _time = 0f;
    private bool order =false;
    private int t;
    void Update()
    {
        _time += Time.deltaTime;
        if (_time > 1.5f)
        {
            t++;
            if (order)
            {
                RotateOverTime(transform.right*-1,1);
            }
            else
            {
                RotateOverTime(transform.right,1);
            }

            if (t==3)
            {
                t = 0;
                order = !order;
            }
            _time = 0;   
        }
    }
    void RotateOverTime(Vector3 targetRotation, float time)
    {
        StartCoroutine(RotateOverTimeCoroutine(targetRotation, time));
    }

    private IEnumerator RotateOverTimeCoroutine(Vector3 targetRotation, float duration)
    {
        var startRotation = Quaternion.LookRotation(transform.forward);
        var timePassed = 0f;
        Quaternion target=Quaternion.LookRotation(targetRotation);
        while (timePassed < duration)
        {
            var factor = timePassed / duration;
            transform.rotation = Quaternion.Lerp(startRotation, target, factor);
            timePassed += Time.deltaTime;
            yield return null;
        }
        transform.rotation= target;
    }
}