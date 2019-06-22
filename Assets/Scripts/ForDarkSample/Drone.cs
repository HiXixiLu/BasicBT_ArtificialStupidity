using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BTFramework;

public class Drone : MonoBehaviour
{
    NavMeshAgent m_Agent;
    BaseNode AITreeRoot;    // 这里保存的是引用吧？？？？？？
    public Transform goal;

    // Start is called before the first frame update
    void Start()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
    }

    //BaseNode GetHardcodedBT() {

    //}



    // Update is called once per frame
    void Update()
    {
        
    }
}
