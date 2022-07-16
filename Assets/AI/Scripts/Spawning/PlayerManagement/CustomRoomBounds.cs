using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomRoomBounds : RoomBounds
{
    public Vector3 position = Vector3.zero;
    public Vector3 size = Vector3.one;

    protected override void FindMinAndMax(out Vector3 min, out Vector3 max)
    {
        min = transform.position + position - size / 2;
        max = transform.position + position + size / 2;
    }

    private void OnDrawGizmos()
    {
        Color colour = Color.red;
        colour.a *= 0.4f;
        Gizmos.color = colour;

        Gizmos.DrawCube(transform.position + position, size);

        colour = Color.green;
        colour.a *= 0.4f;

        Gizmos.color = colour;
        Vector3 exitSize = size;
        exitSize.x += exitExpansion;
        exitSize.y += exitExpansion;
        exitSize.z += exitExpansion;
        Gizmos.DrawCube(transform.position + position, exitSize);
    }
}
