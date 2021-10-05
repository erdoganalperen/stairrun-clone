using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utility;

public class LevelManager : MonoBehaviour
{
    public GameObject scoreTextCube;
    public GameObject scoreCube;
    private float _offsetY;
    private float _offsetZ;
    [HideInInspector]public GameObject _scoreStairPoint;

    private int level = 1;
    public List<GameObject> levels;
    private GameObject currentLevel;
    void Start()
    {
        _offsetZ =scoreTextCube.transform.localScale.z;
        _offsetY =scoreTextCube.transform.localScale.y;
        
        CreateLevel(level);
    }

    void CreateLevel(int l)
    {
        currentLevel = Instantiate(levels[l-1], Vector3.zero, Quaternion.identity);
        currentLevel.transform.position += new Vector3(0, -10,10);
    }

    public bool NextLevel()
    {
        level++;
        if(level>levels.Count)
            return false;
        Destroy(currentLevel.gameObject);
        CreateLevel(level);
        return true;
    }
}