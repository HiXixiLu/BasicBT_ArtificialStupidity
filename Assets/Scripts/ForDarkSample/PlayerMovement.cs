using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Vector3 m_Movement;
    Animator m_Animator;
    public float turnSpeed = 20f;
    public float moveSpeed = 1f;
    Quaternion m_Rotation = Quaternion.identity;
    Rigidbody m_Rigidbody;
    Transform m_Transform;
    //AudioSource m_AudioSource;


    // Start is called before the first frame update
    void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Transform = GetComponent<Transform>();
        //m_AudioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        //MovementControl();
        RigidBodyMovementControl();
    }


    // 刚体的运动在这里捕捉
    private void RigidBodyMovementControl()
    {
        if (Input.GetKey(KeyCode.W))
        {
            m_Animator.SetBool("Walk_Anim", true);
            m_Movement = Vector3.forward * moveSpeed * Time.deltaTime;
            RigidBodyMoveOn();
        }
        else if (Input.GetKey(KeyCode.S))
        {
            m_Animator.SetBool("Walk_Anim", true);
            m_Movement = Vector3.back * moveSpeed * Time.deltaTime;
            RigidBodyMoveOn();
        }
        else if (Input.GetKey(KeyCode.A))
        {
            m_Animator.SetBool("Walk_Anim", true);
            m_Movement = Vector3.left * moveSpeed * Time.deltaTime;
            RigidBodyMoveOn();
        }
        else if (Input.GetKey(KeyCode.D))
        {
            m_Animator.SetBool("Walk_Anim", true);
            m_Movement = Vector3.right * moveSpeed * Time.deltaTime;
            RigidBodyMoveOn();
        }
        else
        {
            m_Animator.SetBool("Walk_Anim", false);
        }
    }


    private void MovementControl()
    {

        if (Input.GetKey(KeyCode.W))
        {
            m_Animator.SetBool("Walk_Anim", true);
            m_Movement = Vector3.forward * moveSpeed * Time.deltaTime;
            MoveOn();
        }
        else if (Input.GetKey(KeyCode.S))
        {
            m_Animator.SetBool("Walk_Anim", true);
            m_Movement = Vector3.back * moveSpeed * Time.deltaTime;
            MoveOn();
        }
        else if (Input.GetKey(KeyCode.A))
        {
            m_Animator.SetBool("Walk_Anim", true);
            m_Movement = Vector3.left * moveSpeed * Time.deltaTime;
            MoveOn();
        }
        else if (Input.GetKey(KeyCode.D))
        {
            m_Animator.SetBool("Walk_Anim", true);
            m_Movement = Vector3.right * moveSpeed * Time.deltaTime;
            MoveOn();
        }
        else
        {
            m_Animator.SetBool("Walk_Anim", false);
        }

    }

    // 高危，这样控制位移，会导致两个刚体穿模
    private void MoveOn() {
        Vector3 desiredForward = Vector3.RotateTowards(transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f);
        m_Rotation = Quaternion.LookRotation(desiredForward);

        transform.position += m_Movement;
        transform.rotation = m_Rotation;
    }

    private void RigidBodyMoveOn()
    {
        Vector3 desiredForward = Vector3.RotateTowards(m_Transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f);
        m_Rotation = Quaternion.LookRotation(desiredForward);

        m_Rigidbody.MovePosition(m_Transform.position + m_Movement);    // 汗！！ 需要加 Collider才能动
        //m_Rigidbody.AddForce(Vector3.back, ForceMode.Force);
        //m_Rigidbody.velocity = m_Movement;
        m_Transform.rotation = m_Rotation;
    }
}