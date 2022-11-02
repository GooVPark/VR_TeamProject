using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{

    public enum Status
    {
        Idle,
        Walk,
        Talk,
    }
    public Status status;
    public TalkInteractable talkInteractable;

    [SerializeField]
    GameObject[] objGender;

    [SerializeField]
    Animator animator;

    [Range(0, 100)]
    [SerializeField]
    float speed;

    int index;
    float timer;

    System.Random rand;
    NavMeshAgent agent;
    Dictionary<Status, string> triggerKey;


    private void Start()
    {
        rand = new System.Random();
        triggerKey = new Dictionary<Status, string>();
        agent = GetComponent<NavMeshAgent>();

        talkInteractable.self = this;

        foreach (GameObject item in objGender)
        {
            if (item.activeSelf)
            {
                animator = item.GetComponent<Animator>();
            }
        }

        triggerKey.Add(Status.Idle, "Idle");
        triggerKey.Add(Status.Walk, "Walk");
        triggerKey.Add(Status.Talk, "Talk");

        if (agent != null)
        {
            agent.speed = speed;
        }

        timer = ResetTimer();
    }
    private void Update()
    {
        if(agent != null)
        {
            switch (status)
            {
                case Status.Idle:

                    if (WayPointCollections.list[index].target != null)
                    {
                        transform.LookAt(WayPointCollections.list[index].target.position);
                    }
                    timer -= Time.deltaTime;

                    if(timer < 0)
                    {
                        timer = ResetTimer();
                        Vector3 temp = RandPosition();
                        temp = new Vector3(transform.position.x, transform.position.y, temp.z);
                        transform.LookAt(temp);
                        agent.SetDestination(temp);
                        agent.isStopped = false;
                        ChangeStatus(Status.Walk);
                    }
                    break;
                case Status.Walk:
                    if (agent.remainingDistance <= agent.stoppingDistance)
                    {
                        ChangeStatus(Status.Idle);
                        agent.isStopped = true;
                    }
                    break;
                case Status.Talk:

                    timer -= Time.deltaTime;
                    transform.LookAt(new Vector3(transform.position.x , transform.position.y, talkInteractable.interlocutor.transform.position.z));

                    if(timer < 0)
                    {
                        agent.enabled = true;
                        timer = ResetTimer();
                        Vector3 temp = RandPosition();
                        temp = new Vector3(transform.position.x, transform.position.y, temp.z);
                        transform.LookAt(temp);
                        agent.SetDestination(temp);
                        agent.isStopped = false;
                        ChangeStatus(Status.Walk);
                    }
                    break;
            }

            if (talkInteractable.isTalk)
            {
                ChangeStatus(Status.Talk);
                timer = 5;
                //agent.isStopped = true;
                agent.enabled = false;
                talkInteractable.isTalk = false;
            }

        }
    }

    void ChangeStatus(Status status)
    {
        this.status = status;
        animator.SetTrigger(triggerKey[status]);
    }

    int ResetTimer(int min = 3, int max = 6)
    {
        return rand.Next(min, max);
    }
    Vector3 RandPosition()
    {

        while (true)
        {
            int temp = rand.Next(WayPointCollections.list.Count);

            if (index != temp)
            {
                index = temp;
                break;
            }
        }

        Vector3 pos = WayPointCollections.list[index].transform.position + new Vector3((float)rand.NextDouble(), 0, (float)rand.NextDouble());

        return pos;
    }


}
