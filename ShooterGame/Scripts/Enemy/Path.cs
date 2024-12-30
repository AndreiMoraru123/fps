using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEditor;
using UnityEngine;

public class Path : MonoBehaviour
{
    public List<Transform> waypoints;

    [SerializeField]
    private bool alwaysDrawPath;

    [SerializeField]
    private bool drawAsLoop;

    [SerializeField]
    private bool drawNumbers;
    public Color debugColor = Color.white;

#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        if (alwaysDrawPath)
        {
            DrawPath();
        }
    }

    public void DrawPath()
    {
        for (int i = 0; i < waypoints.Count; i++)
        {
            var labelStyle = new GUIStyle();
            labelStyle.fontSize = 30;
            labelStyle.normal.textColor = debugColor;

            if (drawNumbers)
            {
                Handles.Label(waypoints[i].position, i.ToString(), labelStyle);
            }

            if (i >= 1)
            {
                Gizmos.color = debugColor;
                Gizmos.DrawLine(waypoints[i - 1].position, waypoints[i].position);

                if (drawAsLoop)
                {
                    Gizmos.DrawLine(waypoints[i - 1].position, waypoints[0].position);
                }
            }
        }
    }

    public void OnDrawGizmosSelected()
    {
        if (alwaysDrawPath) return;
        else DrawPath();
    }
#endif
}
