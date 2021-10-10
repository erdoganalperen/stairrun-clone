using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore;

public class PlayerOnTriggerController : MonoBehaviour
{
    private bool _canTurn;
    private BasketController _basketController;
    public UIManager uiManager;
    private PlayerController _playerController;

    private int _overlaps;

    private void Start()
    {
        _playerController = GetComponent<PlayerController>();
        _basketController = GetComponentInChildren<BasketController>();

        _canTurn = true;
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

        if (other.CompareTag("TurnLeft") || other.CompareTag("TurnRight"))
        {
            _canTurn = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            _overlaps++;
            GetComponent<PlayerController>().isGrounded = true;
        }

        if (_canTurn)
        {
            if (other.CompareTag("TurnLeft"))
            {
                var c = other.GetComponent<TurnCube>();
                _playerController.TurnLeft(c.axis, c.targetPosition);
                _canTurn = false;
            }
            else if (other.CompareTag("TurnRight"))
            {
                var c = other.GetComponent<TurnCube>();
                _playerController.TurnRight(c.axis, c.targetPosition);
                _canTurn = false;
            }
        }

        if (other.CompareTag("StairBrick"))
        {
            Destroy(other.gameObject);
            _basketController.AddBrickOrder();
        }

        if (other.CompareTag("ScoreCube"))
        {
            GetComponent<PlayerController>().playerState = PlayerState.Finished;
            _playerController.TranslateToScoreCube(other.transform);
            uiManager.NextLevel();
        }
        else if (other.CompareTag("Finish"))
        {
            GetComponent<PlayerController>().playerState = PlayerState.FinishRun;
        }
    }
}