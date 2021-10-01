using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        _scoreStairPoint = currentLevel.transform.Find("ScoreStairPoint").gameObject;
        CreateScoreStair(30,_scoreStairPoint);
    }
    void CreateScoreStair(int stairNumber,GameObject scoreStairPoint)
    {
        scoreStairPoint.transform.position += scoreStairPoint.transform.localRotation*new Vector3(0, _offsetY / 2, _offsetZ / 2);
        for (int i = 0; i < stairNumber; i++)
        {
            //score text cube
            var scoreTextCubeInstantiated = Instantiate(scoreTextCube, Vector3.zero, Quaternion.identity, scoreStairPoint.transform);
            scoreTextCubeInstantiated.transform.rotation = scoreStairPoint.transform.localRotation;
            Vector3 textCubePos = new Vector3(0, i * _offsetY, i * _offsetZ);
            scoreTextCubeInstantiated.transform.localPosition = textCubePos;
            //set texts
            var scoreTexts = scoreTextCubeInstantiated.GetComponentsInChildren<TextMesh>();
            for (int j = 0; j < scoreTexts.Length; j++)
            {
                scoreTexts[j].text = "x" + i;
            }
            //score cube fill
            var scoreCubeInstantiated = Instantiate(scoreCube, Vector3.zero, Quaternion.identity, scoreStairPoint.transform);
            scoreCubeInstantiated.transform.rotation = scoreStairPoint.transform.localRotation;
            var fillCubeScale = scoreCubeInstantiated.transform.localScale;
            fillCubeScale.z *= (stairNumber - i);
            // Vector3 fillCubePos = new Vector3(0, i * _offsetY, _offsetZ*(stairNumber -i)/2+(_offsetZ/2)+(i*_offsetZ));
            Vector3 fillCubePos = new Vector3(0, i * _offsetY, _offsetZ*(stairNumber+i+1)/2);
            scoreCubeInstantiated.transform.localScale = fillCubeScale;
            scoreCubeInstantiated.transform.localPosition = fillCubePos;
        }
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