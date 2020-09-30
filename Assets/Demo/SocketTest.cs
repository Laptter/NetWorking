using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocketTest : MonoBehaviour
{
    SocketManager socketManager;
    private void OnEnable()
    {
        socketManager = GetComponent<SocketManager>();
    }


    private void OnGUI()
    {
        if (GUILayout.Button("UdpSend"))
        {
            socketManager.SendToDefault("hello world");
        }

        if (GUILayout.Button("TcpSend"))
        {
            socketManager.SendToDefault("hello world", System.Net.Sockets.ProtocolType.Tcp);
        }
    }
}
