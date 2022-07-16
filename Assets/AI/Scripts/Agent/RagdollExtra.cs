using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollExtra : MonoBehaviour
{
    public enum RagdollType
    {
        head,
        upperTorso,
        lowerTorso,
        limb
    }

    public RagdollType type;

    public RagdollType GetRagdollType()
    {
        return type;
    }
}
