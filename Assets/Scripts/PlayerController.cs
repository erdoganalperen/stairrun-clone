using System;
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
    private bool _leftClick;

    public PlayerState playerState = PlayerState.Starting;

//
    private BasketController _basketController;

    //
    private PlayerState lastState;
    public Animator playerAnimator;
    public GameObject riseStepParent;
    public GameObject riseStair;
    private float riseStepTime;
    private GameObject rsp;
    private int basketStairReset;
    public Transform finishCamPos;
    public GameObject camera;
    private Vector3 defPos;
    private Quaternion defRot;
    private Vector3 camDefPos;
    private Quaternion camDefRot;

    private void Start()
    {
        _basketController = GetComponentInChildren<BasketController>();
        camDefPos = camera.transform.position;
        camDefRot = camera.transform.rotation;
        defPos = transform.position;
        defRot = transform.rotation;
        lastState = playerState;
        basketStairReset = 0;
    }

    private void Update()
    {
        AnimationControl();
        GetInput();
        if (playerState == PlayerState.Finished)
        {
            camera.transform.position = Vector3.Lerp(camera.transform.position, finishCamPos.transform.position,
                Time.deltaTime * 3);
            camera.transform.rotation = Quaternion.Lerp(camera.transform.rotation, finishCamPos.transform.rotation,
                Time.deltaTime * 3);
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
            if (playerState != PlayerState.Rising)
            {
                if (rsp != null)
                    Destroy(rsp.gameObject);
                rsp = new GameObject("riseStepParent");
                rsp.transform.position = riseStepParent.transform.position;
                playerState = PlayerState.Rising;
            }

            if (playerState == PlayerState.FinishRun)
            {
                if (rsp == null)
                {
                    rsp = new GameObject("riseStepParent");
                    rsp.transform.position = riseStepParent.transform.position;
                }
            }
        }
        else
        {
            if (playerState == PlayerState.Rising)
            {
                riseStepParent.transform.DetachChildren();
                playerState = PlayerState.Falling;
            }
            if (isGrounded)
            {
                playerState = PlayerState.Running;
            }
         
        }
        
        Run();
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
            var position = riseStepParent.transform.position;
            var brick = Instantiate(riseStair, position, Quaternion.identity, rsp.transform);
            brick.transform.localRotation = riseStepParent.transform.rotation;
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
        transform.Translate(Vector3.up * (Time.fixedDeltaTime * speedY));
        Rise();
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

    public void Rotate(Vector3 rot, float time)
    {
        var quaternion = Quaternion.Euler(rot);
        RotateOverTime(quaternion, time);
    }

    void AnimationControl()
    {
        if (lastState != playerState)
        {
            lastState = playerState;
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

    void RotateOverTime(Quaternion targetRotation, float time)
    {
        StartCoroutine(RotateOverTimeCoroutine(targetRotation, time));
    }

    private IEnumerator RotateOverTimeCoroutine(Quaternion targetRotation, float duration)
    {
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

    public void Reset()
    {
        playerState = PlayerState.Starting;
        transform.position = defPos;
        transform.rotation = defRot;
        camera.transform.position = camDefPos;
        camera.transform.rotation = camDefRot;
    }
}