using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkInteractable : MonoBehaviour
{

    public AIController interlocutor;
    public bool isTalk;
    public AIController self;
    SphereCollider sphereCollider;

    System.Random rand;
    

    private void Start()
    {
        rand = new System.Random();
        sphereCollider = GetComponent<SphereCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            interlocutor = other.GetComponentInParent<AIController>();
            if (interlocutor.status == AIController.Status.Talk ||
                self.status == AIController.Status.Talk)
                return;
            if (rand.NextDouble() > 0.5f)
            {
                isTalk = true;
                interlocutor.talkInteractable.isTalk = true;
                interlocutor.talkInteractable.interlocutor = self;
                StartCoroutine(ResetCollider());
            }
        }
    }


    IEnumerator ResetCollider()
    {
        sphereCollider.enabled = false;
        yield return new WaitForSeconds(20f);
        sphereCollider.enabled = true;
    }
}
