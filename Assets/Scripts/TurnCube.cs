using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Axis
{
   X,
   Y,
   Z
}
public class TurnCube : MonoBehaviour
{
   public Axis axis;
   public float targetPosition;
}
