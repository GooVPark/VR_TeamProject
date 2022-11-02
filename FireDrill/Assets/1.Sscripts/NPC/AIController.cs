using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    public NavMeshAgent agent;

    [Range(0, 100)] public float speed;

    System.Random rand;
    int index;

    private void Start()
    {
        rand = new System.Random();

        agent = GetComponent<NavMeshAgent>();

        if (agent  != null)
        {
            agent.speed = speed;
            agent.SetDestination(WayPointCollections.list[RandIndex()].transform.position);

        }
    }
    private void Update()
    {
        if(agent != null)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
                agent.SetDestination(WayPointCollections.list[RandIndex()].transform.position);
        }
    }

    int RandIndex()
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

        return index;
    }


}
