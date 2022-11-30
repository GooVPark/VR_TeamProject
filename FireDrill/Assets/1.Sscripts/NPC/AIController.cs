using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum BehaviourState
{
    Idle,
    Walk,
    Talk,
}

public class AIController : MonoBehaviour
{
    public BehaviourState currentBehaviour;
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

    private float randomAnimationindex = 0f;
    [SerializeField] private int randomAnimationCount;

    System.Random rand;
    NavMeshAgent agent;
    Dictionary<BehaviourState, string> triggerKey;

    [SerializeField] private int idleAnimationCount;
    [SerializeField] private int walkAnimationCount;
    [SerializeField] private int talkAnimationCount;


    private void Start()
    {
        rand = new System.Random();
        triggerKey = new Dictionary<BehaviourState, string>();
        agent = GetComponent<NavMeshAgent>();

        talkInteractable.self = this;

        foreach (GameObject item in objGender)
        {
            if (item.activeSelf)
            {
                animator = item.GetComponent<Animator>();
                item.GetComponent<AnimationEventHandler>().animationEndEvent += AnimationTransition;
            }
        }

        triggerKey.Add(BehaviourState.Idle, "Idle");
        triggerKey.Add(BehaviourState.Walk, "Walk");
        triggerKey.Add(BehaviourState.Talk, "Talk");

        if (agent != null)
        {
            agent.speed = speed;
        }

        AnimationTransition();

        timer = ResetTimer();
    }
    private void Update()
    {
        if(agent != null)
        {
            switch (currentBehaviour)
            {
                case BehaviourState.Idle:

                    if (WayPointCollections.list[index].target != null)
                    {
                        //transform.LookAt(WayPointCollections.list[index].target.position);
                        Vector3 target = new Vector3(WayPointCollections.list[index].target.position.x, 0, WayPointCollections.list[index].target.position.z);
                        transform.LookAt(target);
                    }
                    timer -= Time.deltaTime;

                    if(timer < 0)
                    {
                        timer = ResetTimer();
                        Vector3 temp = RandPosition();
                        temp = new Vector3(temp.x, transform.position.y, temp.z);
                        transform.LookAt(temp);
                        agent.SetDestination(temp);
                        agent.isStopped = false;
                        ChangeStatus(BehaviourState.Walk);
                    }
                    break;
                case BehaviourState.Walk:
                    if (agent.remainingDistance <= agent.stoppingDistance)
                    {
                        ChangeStatus(BehaviourState.Idle);
                        agent.isStopped = true;
                    }
                    break;
                case BehaviourState.Talk:

                    timer -= Time.deltaTime;
                    if (talkInteractable.interlocutor != null)
                    {
                        transform.LookAt(new Vector3(transform.position.x, transform.position.y, talkInteractable.interlocutor.transform.position.z));
                    }
                    else
                    {
                        timer = -1;
                    }

                    if(timer < 0)
                    {
                        agent.enabled = true;
                        timer = ResetTimer();
                        Vector3 temp = RandPosition();
                        temp = new Vector3(temp.x, transform.position.y, temp.z);
                        transform.LookAt(temp);
                        agent.SetDestination(temp);
                        agent.isStopped = false;
                        ChangeStatus(BehaviourState.Walk);
                    }
                    break;
            }

            if (talkInteractable.isTalk)
            {
                ChangeStatus(BehaviourState.Talk);
                timer = 5;
                //agent.isStopped = true;
                agent.enabled = false;
                talkInteractable.isTalk = false;
            }

        }
    }

    public void AnimationTransition()
    {
        switch (currentBehaviour)
        {
            case BehaviourState.Idle:

                randomAnimationindex = (float)Random.Range(0, idleAnimationCount - 1);
                animator.SetFloat("AnimationIndex", randomAnimationindex);

                break;
            case BehaviourState.Walk:

                randomAnimationindex = (float)Random.Range(0, walkAnimationCount - 1);
                animator.SetFloat("AnimationIndex", randomAnimationindex);

                break;
            case BehaviourState.Talk:

                randomAnimationindex = (float)Random.Range(0, talkAnimationCount - 1);
                animator.SetFloat("AnimationIndex", randomAnimationindex);

                break;
        }
    }

    void ChangeStatus(BehaviourState status)
    {
        currentBehaviour = status;

        AnimationTransition();

        int animationState = (int)status;
        animator.SetInteger("AnimationState", animationState);
    }

    int ResetTimer(int min = 10, int max = 60)
    {
        return rand.Next(min, max);
    }
    Vector3 wayPointPos;
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
