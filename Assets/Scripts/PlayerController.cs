using System;
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
    public bool isGrounded = true;
    public PlayerState playerState = PlayerState.Starting;
    private PlayerState lastState;
    public Animator playerAnimator;
    public GameObject riseStepParent;
    public GameObject riseStair;
    private float riseStepTime;
    private GameObject rsp;
    public int stairCount;
    private int basketStairReset;
    public Transform finishCamPos;
    public GameObject camera;
    private Vector3 defPos;
    private Quaternion defRot;
    private Vector3 camDefPos;
    private Quaternion camDefRot;
    private void Start()
    {
        camDefPos = camera.transform.position;
        camDefRot = camera.transform.rotation;
        defPos = transform.position;
        defRot = transform.rotation;
        lastState = playerState;
        basketStairReset = 0;
        stairCount = 0;
    }

    private void Update()
    {
        AnimationControl();
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

        transform.Translate(Vector3.forward * (speedZ * Time.fixedDeltaTime));

        if (stairCount > 0 && playerState != PlayerState.FinishRun)
        {
            if (Input.GetMouseButton(0))
            {
                if (playerState != PlayerState.Rising)
                {
                    if (rsp != null)
                        Destroy(rsp.gameObject);
                    rsp = new GameObject("riseStepParent");
                    rsp.transform.position = riseStepParent.transform.position;
                }

                isGrounded = false;
                playerState = PlayerState.Rising;
                transform.Translate(Vector3.up * (Time.fixedDeltaTime * speedY));
                RiseStep();
            }
            else
            {
                playerState = isGrounded ? PlayerState.Running : PlayerState.Falling;
            }
        }

        if (playerState == PlayerState.Falling)
        {
            transform.Translate(Vector3.down * (Time.fixedDeltaTime * speedY));
            riseStepParent.transform.DetachChildren();
        }

        if (playerState == PlayerState.FinishRun)
        {
            if (Input.GetMouseButton(0) && stairCount > 0)
            {
                if (rsp == null)
                {
                    rsp = new GameObject("riseStepParent");
                    rsp.transform.position = riseStepParent.transform.position;
                }

                isGrounded = false;
                transform.Translate(Vector3.up * (Time.fixedDeltaTime * speedY));
                RiseStep();
            }
        }

        if (transform.position.y < -1)
        {
            playerState = PlayerState.Finished;
            camera.transform.parent = null;
        }
    }

    void RiseStep()
    {
        var i = Instantiate(riseStair, riseStepParent.transform.position, Quaternion.identity, rsp.transform);
        i.transform.localRotation = riseStepParent.transform.rotation;
        i.transform.position = riseStepParent.transform.position;
        stairCount--;
        if (stairCount == 0)
        {
            playerState = PlayerState.Falling;
        }

        // basket stair decrease
        if (stairCount % 5 == 0)
        {
            GetComponent<PlayerOnTriggerController>().PopBasket();
        }
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

    public void Reset()
    {
        playerState = PlayerState.Starting;
        transform.position = defPos;
        transform.rotation = defRot;
        camera.transform.position = camDefPos;
        camera.transform.rotation = camDefRot;
        stairCount = 0;
    }
}