using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;   // C#原生模块
using UnityEngine.UI;
using System.Threading;
using System;

/* 该程序中 —— Connect / Send / Receive 都是阻塞方法
 */
public class EchoTest : MonoBehaviour
{
    // Socket definition
    Socket socket;

    public InputField inputField;
    public Text text;

    // 接收缓冲区
    byte[] readBuff = new byte[1024];
    string recvStr = "";

    // ConnectButton on click
    public void Connecttion() {
        // Socket
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        // Connect
        //socket.Connect("127.0.0.1", 8888);  // synchronous connection
        socket.BeginConnect("127.0.0.1", 8888, ConnectionCallback, socket);

    }
    // Connect 回调
    // 这里的返回值、参数值都是由委托类 AsyncCallback 决定的
    public void ConnectionCallback(System.IAsyncResult ar) {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            socket.EndConnect(ar);                  //异步挂起连接请求
            Debug.Log("Socket Connect Succeeded.");

            socket.BeginReceive(readBuff, 0, 1024, 0, ReceiveCallback, socket);
        }
        catch (SocketException ex) {
            Debug.Log("Socket Connect fail: " + ex.ToString());
        }
    }
    // Receive 回调
    public void ReceiveCallback(IAsyncResult ar) {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            int count = socket.EndReceive(ar);
            recvStr = System.Text.Encoding.Default.GetString(readBuff, 0, count);

            socket.BeginReceive(readBuff, 0, 1024, 0, ReceiveCallback, socket);  // 不算递归，这是一个委托
        }
        catch (SocketException ex) {
            Debug.Log("Socket Receive fail" + ex.ToString());
        }
    }

    // SendButton on click
    // 很奇怪 ： 连接建立了以后，为什么会只能发送一次呢？
    public void Send() {
        // Send
        string sendStr = inputField.text;

        byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendStr);  // 以一定的编码方式将字符转为字节流
        socket.Send(sendBytes);

        // Receive
        //byte[] readBuff = new byte[1024];   // 1KB
        //int count = socket.Receive(readBuff); 
        //string recvStr = System.Text.Encoding.Default.GetString(readBuff, 0, count);    //从字节流中转码为字符

        //text.text = recvStr;

        //// Close
        //socket.Close();
    }

    public void Update()
    {
        text.text = recvStr;
    }
}
