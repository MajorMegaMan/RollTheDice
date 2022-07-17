using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextFollow : MonoBehaviour
{
    CamEvents m_camEvents;
    Transform m_camTransform;
    [SerializeField] ObjectiveDie m_die;
    [SerializeField] TMP_Text m_dieText;
    Vector3 m_offset = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        m_camEvents = Camera.main.GetComponent<CamEvents>(); // Dumb, but is being a short cut for easy adding to scene.
        m_camTransform = m_camEvents.transform;
        m_offset = transform.position - m_die.transform.position;
        m_camEvents.onPreRenderEvent.AddListener(FollowTransform);
    }

    // Update is called once per frame
    void Update()
    {
        SetText();
    }

    void FollowTransform()
    {
        transform.position = m_die.transform.position + m_offset;
        Vector3 toCam = m_camTransform.position - transform.position;
        // Reversed here, cause I think I set it up wrong.
        transform.LookAt(transform.position - toCam);
    }

    void SetText()
    {
        int num = m_die.GetUpSideIndex() + 1;
        m_dieText.text = num.ToString();
    }

    private void OnDestroy()
    {
        m_camEvents.onPreRenderEvent.RemoveListener(FollowTransform);
    }
}
