using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPush : MonoBehaviour
{
    public PlayerController player;

    public float pushStrength = 5.0f;
    public float pushSmoothTime = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Vector3 toPlayer = player.transform.position - transform.position;

            float distance = toPlayer.magnitude;
            toPlayer = toPlayer / distance;
            toPlayer.y = 0;

            player.PushPlayer(toPlayer * pushStrength, pushSmoothTime);
        }
    }
}
