using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Looker : MonoBehaviour
{
    public GameObject guard;

    private float reset = 5;
    private bool movingDown;
    // Start is called before the first frame update
    //void Start()
    //{

    //}

    // Update is called once per frame
    void Update()
    {
        if (!movingDown)
            transform.position -= new Vector3(0, 0, 0.1f);
        else
            transform.position += new Vector3(0, 0, 0.1f);

        if (transform.position.z >= 10)
            movingDown = false;
        else if (transform.position.z <= -10)
            movingDown = true;

        reset -= Time.deltaTime;
        if (reset < 0) {
            guard.GetComponent<Guard>().enabled = false;
            GetComponent<SphereCollider>().enabled = true;
        }
    }

    // 碰撞到 Looker 才通知 Guard
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")   // tag 可以对 GameObject 进行分类
        {
            guard.GetComponent<Guard>().enabled = true;
            reset = 5;
            GetComponent<SphereCollider>().enabled = false; // 取消碰撞组件后会有“穿模”现象
        }
    }
}
