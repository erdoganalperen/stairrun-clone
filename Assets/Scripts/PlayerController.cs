﻿using System;
using System.Collections;
using UnityEngine;

public enum PlayerState
{
    Starting,
    Running,
    Rising,
    Falling,
    FinishRun,
    Finished
}

public class PlayerController : MonoBehaviour
{
    public float speedZ;
    public float speedY;
    public bool isGrounded;
    public bool isTurning;
    public float rotateTime;
    private bool _leftClick;

    //
    public PlayerState playerState = PlayerState.Starting;

    //
    private BasketController _basketController;

    //
    private PlayerState _lastState;
    public Animator playerAnimator;
    public GameObject riseStairParent;
    public GameObject riseStair;
    private float _riseStepTime;
    private GameObject _rsp;
    public Transform finishCamPos;
    public Transform finishRunCamPos;
    public GameObject camera;
    private Vector3 defPos;
    private Quaternion defRot;
    private Vector3 camDefPos;
    private Quaternion camDefRot;
    private float finishMaxDistance;
    private bool canRun;
    private void Start()
    {
        _basketController = GetComponentInChildren<BasketController>();
        camDefPos = camera.transform.position;
        camDefRot = camera.transform.rotation;
        defPos = transform.position;
        defRot = transform.rotation;
        _lastState = playerState;
        canRun = true;
    }

    private void Update()
    {
        AnimationControl();
        GetInput();
        if (playerState == PlayerState.Finished)
        {
            camera.transform.position = Vector3.Lerp(camera.transform.position, finishCamPos.transform.position,
                Time.deltaTime * 2);
            camera.transform.rotation = Quaternion.Lerp(camera.transform.rotation, finishCamPos.transform.rotation,
                Time.deltaTime * 2);
        }else if (playerState==PlayerState.FinishRun)
        {
            camera.transform.position = Vector3.Lerp(camera.transform.position, finishRunCamPos.transform.position,
                Time.deltaTime * 2);
            camera.transform.rotation = Quaternion.Lerp(camera.transform.rotation, finishRunCamPos.transform.rotation,
                Time.deltaTime * 2);
        }
    }

    private void FixedUpdate()
    {
        if (playerState == PlayerState.Finished || playerState == PlayerState.Starting)
        {
            return;
        }

        if (_leftClick && _basketController.currentBrickNumber != 0)
        {
            if (playerState == PlayerState.FinishRun)
            {
                if (_rsp == null)
                {
                    _rsp = new GameObject("riseStepParent");
                    _rsp.transform.position = riseStairParent.transform.position;
                }
            }
            else if (playerState != PlayerState.Rising)
            {
                if (_rsp != null)
                    Destroy(_rsp.gameObject);
                _rsp = new GameObject("riseStepParent");
                _rsp.transform.position = riseStairParent.transform.position;
                playerState = PlayerState.Rising;
            }
        }
        else
        {
            if (playerState == PlayerState.Rising)
            {
                riseStairParent.transform.DetachChildren();
                playerState = PlayerState.Falling;
            }else if (playerState == PlayerState.FinishRun)
            {
                playerState = PlayerState.Falling;
            }

            if (isGrounded)
            {
                playerState = PlayerState.Running;
            }
        }

        if (canRun)
        {
            Run();
        }
        switch (playerState)
        {
            case PlayerState.Running:
                break;
            case PlayerState.Rising:
                Rise();
                break;
            case PlayerState.Falling:
                Fall();
                break;
            case PlayerState.FinishRun:
                FinishRise();
                break;
            default:
                break;
        }
    }

    void GetInput()
    {
        if (Input.GetMouseButton(0))
        {
            _leftClick = true;
        }
        else
        {
            _leftClick = false;
        }
    }

    void Run()
    {
        transform.Translate(Vector3.forward * (speedZ * Time.fixedDeltaTime));
    }

    void Rise()
    {
        if (_basketController.Remove())
        {
            transform.Translate(Vector3.up * (Time.fixedDeltaTime * speedY));
            var position = riseStairParent.transform.position;
            var brick = Instantiate(riseStair, position, Quaternion.identity, _rsp.transform);
            brick.transform.localRotation = riseStairParent.transform.rotation;
            brick.transform.position = position;
        }
        else
        {
            if (playerState == PlayerState.Rising)
                playerState = PlayerState.Falling;
        }
    }

    void FinishRise()
    {
        // transform.Translate(Vector3.up * (Time.fixedDeltaTime * speedY));
        finishMaxDistance += (Vector3.forward * (speedZ * Time.fixedDeltaTime)).z;
        if (finishMaxDistance > 29) // max score stairs - one cube half
        {
            canRun = false;
            Fall();
        }
        else
        {
            Rise();
        }
    }

    public void TranslateToScoreCube(Transform other)
    {
        if (_rsp!=null)
        {
            Destroy(_rsp.gameObject);
        }
        transform.position = other.position + new Vector3(0, other.localScale.y, 0);
    }
    void Fall()
    {
        if (isGrounded)
        {
            playerState = PlayerState.Running;
        }

        print("fall");
        transform.Translate(Vector3.down * (Time.fixedDeltaTime * speedY));
    }

    public void TurnLeft(Axis ax,float axValue)
    {
        Rotate(transform.right*-1,rotateTime,ax,axValue);
    }
    public void TurnRight(Axis ax,float axValue)
    {
        
        Rotate(transform.right,rotateTime,ax,axValue);
    }
    private void Rotate(Vector3 rot, float time,Axis ax,float axValue)
    {
        isTurning = true;
        RotateOverTime(rot, time,ax,axValue);
    }

    void AnimationControl()
    {
        if (_lastState != playerState)
        {
            _lastState = playerState;
            switch (playerState)
            {
                case PlayerState.Running:
                    playerAnimator.SetBool("falling", false);
                    playerAnimator.SetBool("dancing", false);
                    break;
                case PlayerState.Rising:
                    playerAnimator.SetBool("falling", false);
                    break;
                case PlayerState.Falling:
                    playerAnimator.SetBool("falling", true);
                    break;
                case PlayerState.Finished:
                    playerAnimator.SetBool("dancing", true);
                    break;
            }
        }
    }

    void RotateOverTime(Vector3 targetRotation, float time,Axis ax,float axValue)
    {
        StartCoroutine(RotateOverTimeCoroutine(targetRotation, time,ax,axValue));
    }

    private IEnumerator RotateOverTimeCoroutine(Vector3 targetRotation, float duration,Axis ax,float axValue)
    {
        var startRotation = Quaternion.LookRotation(transform.forward);
        var timePassed = 0f;
        Quaternion targetRot=Quaternion.LookRotation(targetRotation);
        var currentPosition = transform.position;
        while (timePassed < duration)
        {
            var factor = timePassed / duration;
            transform.rotation = Quaternion.Lerp(startRotation, targetRot, factor);
            //
            // if (ax==Axis.X)
            // {
            //     currentPosition.x = axValue;
            //     var targetPos=Vector3.Lerp(transform.position, currentPosition , factor);
            //     transform.position = targetPos;
            // }else if (ax==Axis.Z)
            // {
            //     currentPosition.z = axValue;
            //     var targetPos=Vector3.Lerp(transform.position, currentPosition , factor);
            //     transform.position = targetPos;
            // }
            //
            timePassed += Time.fixedDeltaTime;
            yield return null;
        }
        transform.rotation= targetRot;
        //
        // if (ax==Axis.X)
        // {
        //     var pos = transform.position;
        //     pos.x = axValue;
        //     transform.position=pos;
        // }else if (ax==Axis.Z)
        // {
        //     var pos = transform.position;
        //     pos.z = axValue;
        //     transform.position=pos;
        // }
        //
        isTurning = false;
    }

    public void Reset()
    {
        playerState = PlayerState.Starting;
        transform.position = defPos;
        transform.rotation = defRot;
        camera.transform.position = camDefPos;
        camera.transform.rotation = camDefRot;
    }
}