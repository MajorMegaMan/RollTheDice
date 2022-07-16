using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Health))]
public class Player : MonoBehaviour
{
    Rigidbody m_rigidBody;
    Health m_health;

    [SerializeField] float m_moveForce = 50.0f;
    [SerializeField] float m_moveSpeed = 5.0f;

    [SerializeField] float m_jumpForce = 10.0f;
    [SerializeField] GroundChecker m_groundChecker = null;
    [SerializeField] int m_jumpCount = 1;

    bool m_isGrounded = false;
    int m_currentJumpCount = 0;

    Vector2 m_moveInput = Vector2.zero;

    [SerializeField] Transform m_camTransform = null;
    [SerializeField] float m_cameraSensitivity = 100.0f;
    // This is pitch for x, and yaw for y, measured in degrees
    Vector2 m_targetCamRotation = Vector2.zero;

    Vector2 m_smoothCam = Vector2.zero;
    Vector2 m_smoothCamVelocity = Vector2.zero;
    [SerializeField] float m_smoothCamTime = 0.1f;

    [SerializeField] PlayerShoot m_playerShoot;
    [SerializeField] string m_enemyTag = "Enemy";

    private void Awake()
    {
        m_rigidBody = GetComponent<Rigidbody>();
        m_health = GetComponent<Health>();

        Cursor.lockState = CursorLockMode.Locked;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (m_groundChecker.CheckGround())
        {
            m_groundChecker.EnterRadius();
        }
    }

    // Update is called once per frame
    void Update()
    {
        m_moveInput.x = Input.GetAxis("Horizontal");
        m_moveInput.y = Input.GetAxis("Vertical");

        Vector2 mouseDelta = Vector2.zero;
        mouseDelta.x = Input.GetAxis("Mouse X");
        mouseDelta.y = Input.GetAxis("Mouse Y");

        bool jumpInput = Input.GetAxisRaw("Jump") > 0.0f;
        bool shootInput = Input.GetAxisRaw("Fire1") > 0.0f;

        m_targetCamRotation.x -= mouseDelta.y * m_cameraSensitivity;
        m_targetCamRotation.y += mouseDelta.x * m_cameraSensitivity;

        m_smoothCam.x = Mathf.SmoothDampAngle(m_smoothCam.x, m_targetCamRotation.x, ref m_smoothCamVelocity.x, m_smoothCamTime);
        m_smoothCam.y = Mathf.SmoothDampAngle(m_smoothCam.y, m_targetCamRotation.y, ref m_smoothCamVelocity.y, m_smoothCamTime);

        if(jumpInput && m_isGrounded)
        {
            Jump();
        }

        if(shootInput)
        {
            Shoot();
        }
    }

    private void FixedUpdate()
    {
        Vector3 movementForce = Vector3.zero;
        movementForce.x = m_moveInput.x;
        movementForce.z = m_moveInput.y;

        movementForce = transform.localToWorldMatrix * movementForce;

        Vector3 velocity = m_rigidBody.velocity;
        velocity.x = movementForce.x * m_moveSpeed;
        velocity.z = movementForce.z * m_moveSpeed;
        m_rigidBody.velocity = velocity;
    }

    public bool CanJump()
    {
        return m_currentJumpCount > 0;
    }

    void Jump()
    {
        if(CanJump())
        {
            m_currentJumpCount--;
            Vector3 velocity = m_rigidBody.velocity;
            velocity.y = 0.0f;
            m_rigidBody.velocity = velocity;
            m_rigidBody.AddForce(m_jumpForce * Vector3.up, ForceMode.Impulse);
        }
    }

    public void PushPlayer(Vector3 pushStrength, float pushSmoothTime)
    {

    }

    // Called in the groundchecker event
    public void EnterGround()
    {
        m_isGrounded = true;
        m_currentJumpCount = m_jumpCount;
    }

    // Called in the groundchecker event
    public void ExitGround()
    {
        m_isGrounded = false;
    }

    public void Shoot()
    {
        m_playerShoot.Shoot(m_camTransform);
    }

    public void UpdateLook()
    {
        Vector3 camEuler = m_camTransform.localEulerAngles;
        camEuler.x = m_smoothCam.x;
        m_camTransform.localEulerAngles = camEuler;

        Vector3 bodyEuler = transform.eulerAngles;
        bodyEuler.y = m_smoothCam.y;
        Quaternion rot = Quaternion.Euler(bodyEuler);
        m_rigidBody.MoveRotation(rot);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(!m_isGrounded)
        {
            if (m_groundChecker.CheckGround())
            {
                m_groundChecker.EnterRadius();
            }
        }
    }
}
