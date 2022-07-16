using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    public Animator animator;
    public List<Rigidbody> rigidBodies = new List<Rigidbody>();

    public bool RagdollOn 
    { 
        get 
        { 
            return !animator.enabled; 
        } 
        set 
        { 
            animator.enabled = !value;
            foreach(Rigidbody body in rigidBodies)
            {
                body.isKinematic = !value;
            }
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        RagdollOn = !animator.enabled;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ToggleRagdoll()
    {
        RagdollOn = !RagdollOn;
    }

    public static void TriggerTargetRagdollWithCollider(Collider target, LayerMask triggerLayer)
    {
        //if (Utilities.CompareLayers(target.gameObject.layer, triggerLayer))
        //{
        //    Debug.Log("hit");
        //    BotController bot = target.gameObject.GetComponentInParent<BotController>();
        //    if (bot != null)
        //    {
        //        bot.TriggerRagdoll(true);
        //    }
        //}
    }
}
