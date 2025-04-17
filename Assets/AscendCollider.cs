using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AscendCollider : MonoBehaviour
{
    public PlayerStateMachine playerScript;
    public Transform player;
    public GameObject thirdPersonCam;
    RaycastHit hit;

    public float ascendSpeed;
    public bool stopAscend;

    public void AscendCam()
    {
        thirdPersonCam.SetActive(false);

        Physics.Raycast(transform.position + Vector3.up, Vector3.up, out hit);

    }

    public void Ascend()
    {
        if(!stopAscend)
        {
            playerScript.Anim.SetBool("isAscending", true);

            transform.parent.Translate(Vector3.up * ascendSpeed * Time.deltaTime);
        }
        
    }

    public void AscendClimb()
    {
        playerScript.Anim.SetTrigger("Climb");

        playerScript.isAscendPressed = false;
        playerScript.isAscendUsed = false;
        stopAscend = false;

        transform.parent.DOMoveY(transform.parent.position.y + 0.1f, 1);

        playerScript.CharacterController.enabled = true;

    }

    void OnTriggerExit(Collider other) 
    {
        if(other.CompareTag("Wall"))
        {
            stopAscend = true;
            playerScript.Anim.SetBool("isAscending", false);
        }   
    }

}
