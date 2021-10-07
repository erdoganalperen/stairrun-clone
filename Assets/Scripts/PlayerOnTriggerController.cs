using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore;

public class PlayerOnTriggerController : MonoBehaviour
{
    private float _turnCd = 1f;
    private bool _turned = false;
    private BasketController _basketController;
    public UIManager uiManager;
    private PlayerController _playerController;

    private int _overlaps ;
    private void Start()
    {
        _playerController = GetComponent<PlayerController>();
        _basketController = GetComponentInChildren<BasketController>();
    }

    private void FixedUpdate()
    {
        if (_turned)
        {
            _turnCd -= Time.fixedDeltaTime;
            if (_turnCd <= 0)
            {
                _turnCd = 1f;
                _turned = false;
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            _overlaps--;
            if (_overlaps == 0)
            {
                GetComponent<PlayerController>().isGrounded = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            _overlaps++;
            GetComponent<PlayerController>().isGrounded = true;
        }

        if (other.CompareTag("TurnLeft"))
        {
            _playerController.Rotate(Vector3.left, _playerController.rotateTime);
        }
        else if (other.CompareTag("TurnRight"))
        {
            _playerController.Rotate(Vector3.right, _playerController.rotateTime);
        }
        else if (other.CompareTag("StairBrick"))
        {
            Destroy(other.gameObject);
            _basketController.AddBrickOrder(2);
        }
        else if (other.CompareTag("ScoreCube"))
        {
            GetComponent<PlayerController>().playerState = PlayerState.Finished;
            _playerController.TranslateToScoreCube(other.transform);
        }
        else if (other.CompareTag("Finish"))
        {
            GetComponent<PlayerController>().playerState = PlayerState.FinishRun;
        }
        
    }
    
}