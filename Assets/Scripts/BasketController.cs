using System;
using UnityEngine;

public class BasketController : MonoBehaviour
{
    public Transform point0, point1;
    public GameObject basketBrick;
    public int currentBrickNumber;
    private int _addOrder;
    private float _addCooldown;
    private float _addCooldownTimer;
    private float _offsetY;
    private float _currentOffsetY;
    private void Start()
    {
        currentBrickNumber = 0;
        _addOrder = 0;
        _addCooldown = 0f;
        _addCooldownTimer = 0;
        _offsetY = basketBrick.transform.localScale.y;
        _currentOffsetY = 0;
    }

    private void Update()
    {
        if (_addOrder>0&&_addCooldownTimer<=0f)
        {
            _addOrder--;
            Add();
        }
        _addCooldownTimer -= Time.deltaTime;
    }

    public void AddBrickOrder(int number=1)
    {
        _addOrder += number;
    }
    private void Add()
    {
        _addCooldownTimer = _addCooldown;
        Vector3 pos;
        if (currentBrickNumber%2==0)
        {
            pos = point0.position;
            pos.y += _currentOffsetY;
        }
        else
        {
            pos = point1.position;    
            pos.y += _currentOffsetY;
            _currentOffsetY += _offsetY;
        }
        Instantiate(basketBrick, pos, Quaternion.identity, transform);
        currentBrickNumber++;
        
    }

    private int _basketBricktoStairMultiplier = 2;
    public bool Remove()
    {
        if(currentBrickNumber==0)
            return false;
        if (_basketBricktoStairMultiplier==0)
        {
            var destroyedChild = transform.GetChild(transform.childCount - 1);
            if (currentBrickNumber%2==0)
            {
                _currentOffsetY -= _offsetY;
            }
            Destroy(destroyedChild.gameObject);
            currentBrickNumber--;
            _basketBricktoStairMultiplier = 2;
        }
        else
        {
            _basketBricktoStairMultiplier--;
        }
        return true;
    }   
}