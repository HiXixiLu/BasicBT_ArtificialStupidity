using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody m_Rigidbody;
    private Vector3 m_Movement;
    public float speed = 1;
    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.A)) {
            this.transform.position += Vector3.left * speed;
        }
        if (Input.GetKey(KeyCode.D)) {
            this.transform.position += Vector3.right * speed;
        }
        if (Input.GetKey(KeyCode.W))
        {
            this.transform.position += Vector3.forward * speed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            this.transform.position += Vector3.back * speed;
        }
    }
}
