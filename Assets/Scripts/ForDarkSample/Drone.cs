using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BTFramework;

public class Drone : MonoBehaviour
{
    NavMeshAgent m_Agent;
    BaseNode AITreeRoot;    // 这里保存的是引用吧？？？？？？
    int m_CurrentWaypointIndex = 0;
    public bool m_PlayerAtSight;   //默认值 false

    public Transform goal;
    [SerializeField] public Transform[] points;

    // Start is called before the first frame update
    void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        AITreeRoot = GetHardcodedBT();
    }

    // test only
    BaseNode GetHardcodedBT()
    {
        BaseNode root = new Fallback();
        BaseNode seq1 = new Sequence();
        BaseNode seq2 = new Sequence();
        BaseNode con1 = new Condtion(hasNoPlayer);
        BaseNode con2 = new Condtion(hasPlayer);
        BaseNode ac1 = new Action(Patroling);
        BaseNode ac2 = new Action(Trace);
        root.children.Add(seq1);
        root.children.Add(seq2);
        seq1.children.Add(con1);
        seq1.children.Add(ac1);
        seq2.children.Add(con2);
        seq2.children.Add(ac2);

        return root;
    }

    // 每个 Tick 都会检查
    void Patroling() {
        if (m_Agent.remainingDistance < m_Agent.stoppingDistance)
        {
            m_CurrentWaypointIndex = (m_CurrentWaypointIndex + 1) % points.Length;   //循环使用一个数组索引的公式
            m_Agent.SetDestination(points[m_CurrentWaypointIndex].position);
        }
    }

    // 每个tick都会检查
    void Trace() {
        m_Agent.SetDestination(goal.position);
    }

    bool hasPlayer() {
        return m_PlayerAtSight;
    }
    bool hasNoPlayer() {
        return !m_PlayerAtSight;
    }


    // Update is called once per frame
    void Update()
    {
        AITreeRoot.Execute();
    }
}
