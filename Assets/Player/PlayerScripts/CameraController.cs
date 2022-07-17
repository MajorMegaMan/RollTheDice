using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
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
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(Time.timeScale == 0.0f)
        {
            return;
        }

        Vector2 mouseDelta = Vector2.zero;
        mouseDelta.x = Input.GetAxis("Mouse X");
        mouseDelta.y = Input.GetAxis("Mouse Y");

        bool shootInput = Input.GetAxisRaw("Fire1") > 0.0f;

        m_targetCamRotation.x -= mouseDelta.y * m_cameraSensitivity;
        m_targetCamRotation.y += mouseDelta.x * m_cameraSensitivity;

        m_targetCamRotation.x = Mathf.Clamp(m_targetCamRotation.x, -89.0f, 89.0f);

        m_smoothCam.x = Mathf.SmoothDampAngle(m_smoothCam.x, m_targetCamRotation.x, ref m_smoothCamVelocity.x, m_smoothCamTime);
        m_smoothCam.y = Mathf.SmoothDampAngle(m_smoothCam.y, m_targetCamRotation.y, ref m_smoothCamVelocity.y, m_smoothCamTime);

        if(shootInput)
        {
            Shoot();
        }
    }

    public void PushPlayer(Vector3 pushStrength, float pushSmoothTime)
    {

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
        transform.rotation = rot;
    }
}
