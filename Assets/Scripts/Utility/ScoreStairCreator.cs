using System;
using System.Collections;
using UnityEngine;

namespace Utility
{
    public class ScoreStairCreator : MonoBehaviour
    {
        public GameObject stairTextCube;
        public GameObject stairFillCube;
        private Vector3 _pos;
        private Quaternion _rot;

        private Vector3 _positionOffset;

        //
        public int numberOfStairs;
        //
        public float positionOffsetMultiplier; //scale difference for imports

        //
        private Vector2 _uvPos;
        public Vector2 uvOffset;
        public Color stairColor;

        void Start()
        {
            _pos = Vector3.zero;
            _rot = transform.localRotation;
            _positionOffset = new Vector3(0, stairTextCube.transform.localScale.y, stairTextCube.transform.localScale.z) * positionOffsetMultiplier;
            //
            _uvPos = Vector2.zero;
            //
            CreateScoreStair(numberOfStairs);
        }

        void CreateScoreStair(int stairNumber)
        {
            //first offset will be half 
            _pos += _pos + new Vector3(0, _positionOffset.y / 2, _positionOffset.z / 2);
            for (int i = 0; i < stairNumber; i++)
            {
                var instantiated = Instantiate(stairTextCube, transform);
                instantiated.transform.localPosition = _pos;
                //position offset
                _pos += _positionOffset;
                //Creating new material and setting its own uv position
                Material mat = new Material(Shader.Find("Specular"))
                {
                    mainTexture = stairTextCube.GetComponent<Renderer>().sharedMaterial.mainTexture,
                    color = stairColor
                };
                mat.SetTextureOffset("_MainTex", _uvPos);
                instantiated.GetComponent<Renderer>().material = mat;
                // uv offset
                _uvPos.x += uvOffset.x;
       
            }
        }
    }
}