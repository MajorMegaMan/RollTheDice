using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugArcGizmo : MonoBehaviour
{
    public Transform start;
    public Transform end;

    public float timeLength = 5.0f;
    public int curveCount = 5;
    public float deltaTime = 0.01666667f;

    public int startIndex = 0;

    [Header("Key Componenets")]
    public float gravity = -Physics.gravity.y;

    [Header("Other")]
    public float controlTime = 0.0f;
    public float timePow = 2.0f;

    void DrawArc(Vector3 origin, Vector3 launchDir, float timeStep)
    {
        Vector3 grav = Physics.gravity * timeStep;
        Vector3 currentPos = origin;
        // target
        for (int i = startIndex; i < curveCount; i++)
        {
            Vector3 next = launchDir + grav * i;
            Gizmos.DrawLine(currentPos, currentPos + next * timeStep);
            currentPos += next * timeStep;
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 origin = start.position;
        Vector3 destination = end.position;
        Vector3 dir = destination - origin;
        float distance = dir.magnitude;

        Gizmos.color = Color.blue;

        Gizmos.DrawLine(origin, destination);

        Gizmos.color = Color.red;


        float timeStep = (1.0f / curveCount) * timeLength;

        //Vector3 thing = origin + (next * timeStep) * i;

        Gizmos.color = Color.yellow;
        timeStep = (1.0f / curveCount) * GetDynamicTime();
        Vector3 launchVec = CalcLaunchVec();
        DrawArc(origin, launchVec, timeStep);
    }

    public Vector3 CalcLaunchVec()
    {
        return CalcLaunchVec(end.position - start.position, GetDynamicTime());
    }

    Vector3 CalcLaunchVec(Vector3 displacement, float t)
    {
        return CalcLaunchVec(displacement.x, displacement.y, displacement.z, t);
    }

    Vector3 CalcLaunchVec(float x, float y, float z, float t)
    {
        //float u = (gravity * (t * t) + 2 * y) / 2 * t;
        float u = y / t + gravity * t / 2;

        Vector3 result = new Vector3(x / t, u, z / t);
        return result;
    }

    float GetDynamicTime()
    {
        float result = (end.position - start.position).magnitude;
        result /= Mathf.Pow(controlTime, timePow);
        return result;
    }

    private void OnValidate()
    {

    }
}