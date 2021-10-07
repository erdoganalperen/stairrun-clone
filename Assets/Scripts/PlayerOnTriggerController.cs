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
            GetComponent<PlayerController>().isGrounded = false;
            print("qwe");
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
            _playerController.Rotate(Vector3.left, 1);
        }
        else if (other.CompareTag("TurnRight"))
        {
            _playerController.Rotate(Vector3.right, 1);
        }
        else if (other.CompareTag("StairBrick"))
        {
            Destroy(other.gameObject);
            _basketController.AddBrickOrder();
        }
        else if (other.CompareTag("ScoreCube"))
        {
            GetComponent<PlayerController>().playerState = PlayerState.Finished;
            print(other.transform.position + new Vector3(0, other.transform.localScale.y / 2, 0));
            transform.position = other.transform.position + new Vector3(0, other.transform.localScale.y / 2, 0);
            uiManager.NextLevel();
        }
        else if (other.CompareTag("Finish"))
        {
            GetComponent<PlayerController>().playerState = PlayerState.FinishRun;
        }
        
    }
    
}