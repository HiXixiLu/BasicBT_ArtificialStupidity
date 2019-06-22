using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Guard : MonoBehaviour
{
    public GameObject player;
    private NavMeshAgent m_Navmesh;
    public bool isPatroling = true;

    // Start is called before the first frame update
    void Start()
    {
        m_Navmesh = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isPatroling)
        {

        }
        else {
            m_Navmesh.destination = player.transform.position;
        }
    }
}
