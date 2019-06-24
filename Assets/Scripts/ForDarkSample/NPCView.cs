using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCView : MonoBehaviour
{
    [SerializeField] public Drone m_droneAi;

    // 当选中 isTrigger属性后，该 Collider被物理系统忽略，即不会发生碰撞检测了
    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.collider == m_droneAi.goal)
    //    {
    //        m_droneAi.m_PlayerAtSight = true;
    //    }
    //    else {
    //        m_droneAi.m_PlayerAtSight = false;
    //    }
    //}
    //private void OnCollisionExit(Collision collision)
    //{
    //    if (collision.collider == m_droneAi.goal)
    //    {
    //        m_droneAi.m_PlayerAtSight = false;
    //    }
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            m_droneAi.m_PlayerAtSight = true;
        }
        else
        {
            m_droneAi.m_PlayerAtSight = false;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            m_droneAi.m_PlayerAtSight = false;
        }
    }
}
