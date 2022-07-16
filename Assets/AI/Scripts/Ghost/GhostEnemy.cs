using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostEnemy : MonoBehaviour
{
    public Rigidbody head;
    public Rigidbody spineControl;

    public Rigidbody rightArm;
    public Transform rightHandPos;
    public Rigidbody leftArm;
    public Transform leftHandPos;

    public Transform followTarget;
    public float stoppingDistance = 0.5f;
    bool m_isAtTarget = false;

    Vector3 m_toTarget = Vector3.zero;

    public float upStrength = 5.0f;
    public float groundRayLength = 2.0f;
    public LayerMask groundLayers = ~0;

    public float smoothTime = 0.1f;
    Vector3 m_smoothVel = Vector3.zero;

    Vector3 m_upVel = Vector3.zero;
    public float smoothUpTime = 0.1f;
    Vector3 m_smoothUpVel = Vector3.zero;

    [Range(0.0f, 1.0f)]public float rotSlerpTime = 0.1f;

    public float armStrength = 1.0f;
    public float armTowardsStrength = 2.0f;
    public float armForwardStrength = 2.0f;
    public float armUpStrength = 2.0f;

    public float smoothArmTime = 0.1f;
    Vector3 m_smoothleftArmVel = Vector3.zero;
    Vector3 m_smoothrightArmVel = Vector3.zero;

    public float smoothArmTowardsTime = 0.1f;
    Vector3 m_leftArmTowardsVel = Vector3.zero;
    Vector3 m_rightArmTowardsVel = Vector3.zero;
    Vector3 m_smoothLeftArmTowardsVel = Vector3.zero;
    Vector3 m_smoothRightArmTowardsVel = Vector3.zero;

    public float smoothHeadtime = 0.1f;
    public float headStrength = 1.0f;
    Vector3 m_headVel = Vector3.zero;
    Vector3 m_smoothHeadVel = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        m_toTarget = (followTarget.position - spineControl.transform.position + Vector3.up * groundRayLength).normalized;
    }

    private void Update()
    {
        m_toTarget = (followTarget.position - spineControl.transform.position + Vector3.up * groundRayLength);
        float distance = m_toTarget.magnitude;
        m_toTarget /= distance;

        m_isAtTarget = distance < stoppingDistance;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //ArmTowards(leftArm, leftHandPos, ref m_smoothleftArmVel, ref m_leftArmTowardsVel, ref m_smoothLeftArmTowardsVel);
        //ArmTowards(rightArm, rightHandPos, ref m_smoothrightArmVel, ref m_rightArmTowardsVel, ref m_smoothRightArmTowardsVel);

        spineControl.velocity = Vector3.SmoothDamp(spineControl.velocity, Vector3.zero, ref m_smoothVel, smoothTime);
        
        float rayDepth = 0.0f;
        if(GroundRayDown(out RaycastHit hitInfo))
        {
            rayDepth = 1.0f - hitInfo.distance / groundRayLength;
        }

        m_upVel = Vector3.SmoothDamp(m_upVel, Vector3.up * rayDepth * upStrength, ref m_smoothUpVel, smoothUpTime);

        spineControl.velocity += m_upVel;

        RotateTowardsTarget();

        if (m_isAtTarget)
        {
            return;
        }

        m_headVel = Vector3.SmoothDamp(m_headVel, m_toTarget * headStrength, ref m_smoothHeadVel, smoothHeadtime);
        head.velocity += m_headVel;
    }

    void ArmTowards(Rigidbody arm, Transform handPos, ref Vector3 smoothArm, ref Vector3 armTowardsVel, ref Vector3 smoothArmTowards)
    {
        //arm.velocity = Vector3.SmoothDamp(arm.velocity, Vector3.zero, ref smoothArm, smoothArmTime);

        Vector3 toFollow = (followTarget.position - handPos.position).normalized * armTowardsStrength;
        toFollow += spineControl.transform.forward * armForwardStrength;
        toFollow += Vector3.up * armUpStrength;
        float totalStength = armForwardStrength + armUpStrength + armTowardsStrength;
        if(totalStength != 0)
        {
            toFollow /= totalStength;
        }
        armTowardsVel = Vector3.SmoothDamp(armTowardsVel, toFollow * armStrength, ref smoothArmTowards, smoothArmTowardsTime);
        arm.AddForceAtPosition(armTowardsVel, handPos.position, ForceMode.Acceleration);
        //arm.velocity += armTowardsVel;
    }

    bool GroundRayDown(out RaycastHit hitInfo)
    {
        return Physics.Raycast(spineControl.transform.position, Vector3.down, out hitInfo, groundRayLength, groundLayers, QueryTriggerInteraction.Ignore);
    }

    void RotateTowardsTarget()
    {
        var targetRotation = Quaternion.LookRotation(m_toTarget, Vector3.up);
        spineControl.transform.rotation = Quaternion.Slerp(spineControl.transform.rotation, targetRotation, rotSlerpTime);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 position = spineControl.transform.position;

        if (GroundRayDown(out RaycastHit hitInfo))
        {
            Gizmos.DrawLine(position, hitInfo.point);
        }
        else
        {
            Gizmos.DrawLine(position, position + Vector3.down * groundRayLength);
        }
    }
}
