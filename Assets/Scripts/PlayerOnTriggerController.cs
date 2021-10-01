using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore;

public class PlayerOnTriggerController : MonoBehaviour
{
    public GameObject step;
    public GameObject stepParent;
    public Transform sp1, sp2;
    private float heightOffset = 0;
    private bool order = true;

    private float turnCd = 1f;
    private bool turned = false;
    private Stack<GameObject> _basketStairs = new Stack<GameObject>();
    public UIManager _uiManager;
    private void FixedUpdate()
    {
        if (turned)
        {
            turnCd -= Time.fixedDeltaTime;
            if (turnCd <= 0)
            {
                turnCd = 1f;
                turned = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            bool ground = false;
            var c=Physics.OverlapSphere(transform.position, .5f);
            foreach (var col in c)
            {
                if (col.CompareTag("Ground"))
                    ground = true;
            }
            if(!ground)
                GetComponent<PlayerController>().isGrounded =false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            GetComponent<PlayerController>().isGrounded = true;
        }

        if (other.CompareTag("TurnLeft"))
        {
            StartCoroutine(RotateOverTime(Quaternion.Euler(0, transform.localEulerAngles.y - 90, 0), .2f));
        }
        else if (other.CompareTag("TurnRight"))
        {
            StartCoroutine(RotateOverTime(Quaternion.Euler(0, transform.localEulerAngles.y + 90, 0), .2f));
        }
        else if (other.CompareTag("Step"))
        {
            Vector3 pos;
            if (order)
            {
                order = false;
                pos = sp1.position;
                pos.y += heightOffset;
            }
            else
            {
                order = true;
                pos = sp2.position;
                pos.y += heightOffset;
                heightOffset += step.transform.localScale.y;
            }

            GetComponent<PlayerController>().stairCount += 5;
            var instantiated = Instantiate(step, pos, Quaternion.identity, stepParent.transform);
            instantiated.transform.rotation = transform.localRotation;
            if (_basketStairs != null) _basketStairs.Push(instantiated);
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("ScoreCube"))
        {
            GetComponent<PlayerController>().playerState = PlayerState.Finished;

            print(other.transform.position + new Vector3(0, other.transform.localScale.y / 2, 0));
            transform.position = other.transform.position + new Vector3(0, other.transform.localScale.y / 2, 0);
            _uiManager.NextLevel();
            heightOffset = 0;
            _basketStairs.Clear();
        }
        else if (other.CompareTag("Finish"))
        {
            GetComponent<PlayerController>().playerState = PlayerState.FinishRun;
        }

        if (other.CompareTag("Obstacle"))
        {
            if (bounceTime==0)
            {
                StartCoroutine(Bounce());
            }
        }
    }

    private float bounceTime = 0;
    private IEnumerator Bounce()
    {

        while (bounceTime<1)
        {
            bounceTime += Time.deltaTime;
            transform.Translate(new Vector3(0, 1, -1) * (Time.fixedDeltaTime * 6));
            yield return null;
        }
        bounceTime = 0;
    }
    public void PopBasket()
    {
        heightOffset -= step.transform.localScale.y;

        var destroyed = _basketStairs.Peek();
        _basketStairs.Pop();
        Destroy(destroyed.gameObject);
    }

    private IEnumerator RotateOverTime(Quaternion targetRotation, float duration)
    {
        if (turned)
            yield break;
        turned = true;

        var startRotation = transform.rotation;

        var timePassed = 0f;
        while (timePassed < duration)
        {
            var factor = timePassed / duration;
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, factor);
            timePassed += Time.fixedDeltaTime;
            yield return null;
        }

        transform.rotation = targetRotation;
    }
}