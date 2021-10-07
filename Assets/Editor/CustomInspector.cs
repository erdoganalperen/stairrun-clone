using UnityEditor;
using UnityEngine;

// [CustomEditor(typeof(BasketController))]
public class CustomInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var basketController = (BasketController) target;
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add"))
        {
            basketController.AddBrickOrder();
        }
        if (GUILayout.Button("Remove"))
        {
            basketController.Remove();
        }
        GUILayout.EndHorizontal();
    }
}
