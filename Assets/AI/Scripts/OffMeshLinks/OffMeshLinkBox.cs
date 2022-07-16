using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class OffMeshLinkBox : MonoBehaviour
{
    public float startSize = 1.0f;
    public float endSize = 1.0f;

    public Vector3 endOffset = Vector3.forward;

    [Min(0.0001f)]public float frequency = 1.0f;

    [Header("Initial Values")]
    public int costOverride = -1;
    public bool biDirectional = true;
    public bool activated = true;
    public bool autoUpdatePositions = false;
    public int navigationArea = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GetGenerationData(out Vector3 offsetStart, out Vector3 offsetEnd, out float widthMod, out int toAddCount, out float segmentSize, out float endScalar)
    {
        widthMod = Mathf.Repeat(startSize, 1.0f);
        toAddCount = (int)(startSize * frequency);
        if(widthMod == 0.0f)
        {
            toAddCount -= 1;
        }

        offsetStart = transform.right * (0.5f - (startSize / 2));
        offsetEnd = -offsetStart;

        endScalar = endSize / startSize;

        if (startSize < 1.0f)
        {
            offsetStart = Vector3.zero;
        }

        Vector3 startToEnd = offsetStart - offsetEnd;
        float dist = startToEnd.magnitude;

        if(toAddCount > 0)
        {
            segmentSize = (dist / toAddCount);
        }
        else
        {
            segmentSize = 0;
        }
    }

    public void GenerateOffMeshLinks()
    {
        GetGenerationData(out Vector3 generationOffsetStart, out Vector3 generationOffsetEnd, out float widthMod, out int toAddCount, out float segmentSize, out float endScalar);

        CreateOffMeshLink(generationOffsetStart, generationOffsetStart * endScalar);
        if (widthMod >= 0.0f && startSize > 1.0f)
        {
            CreateOffMeshLink(generationOffsetEnd, generationOffsetEnd * endScalar);
        }

        Vector3 dir = transform.right;
        for (int i = 1; i < toAddCount; i++)
        {
            float segment = segmentSize * i;
            Vector3 startLocation = generationOffsetStart + dir * segment;
            Vector3 endLocation = (generationOffsetStart * endScalar + dir * segment * endScalar);
            CreateOffMeshLink(startLocation, endLocation);
        }

    }

    void CreateOffMeshLink(Vector3 generationStartOffset, Vector3 endGenerationOffset)
    {
        GameObject linkContainer = new GameObject("LinkContainer");
        linkContainer.transform.parent = transform;
        linkContainer.transform.position = transform.position + generationStartOffset;

        OffMeshLink offMeshLink = linkContainer.AddComponent<OffMeshLink>();
        Transform startTransform = new GameObject("Start").transform;
        Transform endTransform = new GameObject("End").transform;

        startTransform.parent = linkContainer.transform;
        startTransform.localPosition = Vector3.zero;
        
        endTransform.parent = linkContainer.transform;
        Vector3 localEndOffset = transform.localToWorldMatrix * endOffset;
        endTransform.position = transform.position + endGenerationOffset + localEndOffset;

        offMeshLink.startTransform = startTransform;
        offMeshLink.endTransform = endTransform;

        offMeshLink.costOverride = costOverride;
        offMeshLink.biDirectional = biDirectional;
        offMeshLink.activated = activated;
        offMeshLink.autoUpdatePositions = autoUpdatePositions;
        offMeshLink.area = navigationArea;
    }

    private void OnDrawGizmos()
    {
        void DrawCube(Vector3 offset, float size, Color colour)
        {
            Vector3 boxSize = Vector3.one;
            boxSize.x = size;

            Gizmos.matrix = transform.localToWorldMatrix;

            Gizmos.color = colour;
            Gizmos.DrawWireCube(offset, boxSize);
            colour.a *= 0.4f;
            Gizmos.color = colour;
            Gizmos.DrawCube(offset, boxSize);
        }

        Color startColour = Color.cyan;
        DrawCube(Vector3.zero, startSize, startColour);
        Gizmos.DrawLine(Vector3.zero, endOffset);
        Color endColour = startColour * 0.5f;
        endColour.r = 1.0f - endColour.g;
        endColour.a = startColour.a;
        DrawCube(endOffset, endSize, endColour);
    }

    private void OnDrawGizmosSelected()
    {
        void DrawCube(Vector3 offset, Vector3 size, Color colour)
        {
            Gizmos.color = colour;
            Gizmos.DrawWireCube(transform.position + offset, size);
            colour.a *= 0.4f;
            Gizmos.color = colour;
            Gizmos.DrawCube(transform.position + offset, size);
        }

        void DrawLinkLocation(Vector3 offset, Color colour)
        {
            DrawCube(offset, Vector3.one * 0.2f, colour);

            //offset = transform.localToWorldMatrix * offset;
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position + offset, 1.0f / frequency / 2.0f);
        }

        GetGenerationData(out Vector3 generationOffsetStart, out Vector3 generationOffsetEnd, out float widthMod, out int toAddCount, out float segmentSize, out float endScalar);

        Vector3 localEndOffset = transform.localToWorldMatrix * endOffset;

        DrawLinkLocation(generationOffsetStart, Color.red);
        DrawLinkLocation(generationOffsetStart * endScalar + localEndOffset, Color.red);

        if (widthMod >= 0.0f && startSize > 1.0f)
        {
            DrawLinkLocation(generationOffsetEnd, Color.red);
            DrawLinkLocation(generationOffsetEnd * endScalar + localEndOffset, Color.red);
        }

        Vector3 dir = transform.right;

        for (int i = 1; i < toAddCount; i++)
        {
            float segment = segmentSize * i;
            Vector3 startLocation = generationOffsetStart + dir * segment;
            Vector3 endLocation = (generationOffsetStart * endScalar + dir * segment * endScalar) + localEndOffset;
            DrawLinkLocation(startLocation, Color.red);
            DrawLinkLocation(endLocation, Color.red);
        }
    }
}
