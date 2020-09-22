using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using UnityEngine;
public class SocketManager : MonoBehaviour
{
    public bool bInitTcpServer;
    [SerializeField]
    public string tcpServerIp;
    [SerializeField]
    public int tcpServerPort;

    public bool bInitUdpServer;
    [SerializeField]
    public string udpServerIp;
    [SerializeField]
    public int udpServerPort;

    public bool bInitTcpClient;
    [SerializeField]
    public string tcpClientIp;
    [SerializeField]
    public int tcpClientPort;

    public bool bInitUdpClient;
    [SerializeField]
    public string udpClientIp;
    [SerializeField]
    public int udpClientPort;


    private List<Server> servers = new List<Server>();

    private void OnEnable()
    {
        if (bInitTcpServer)
        {
            InitTcpServer();
        }

        if (bInitUdpServer)
        {
            InitUdpServer();
        }

        if (bInitTcpClient)
        {
            InitTcpClient();
        }

        if (bInitTcpServer)
        {
            InitUdpClient();
        }
        
    }


    public void InitTcpServer()
    {
        Server tcpServer = new Server(tcpServerIp, tcpServerPort, ProtocolType.Tcp);
        tcpServer.Initialize(ReciveBytes);
        servers.Add(tcpServer);
    }


    private void ReciveBytes(byte[] bytes,int length)
    {
        
    }

    public void InitUdpServer()
    { 

    }

    public void InitUdpClient()
    { 

    }

    public void InitTcpClient()
    {
        Client tcpClient = new Client(tcpClientIp,tcpClientPort, ProtocolType.Tcp);
        tcpClient.SendToTarget("dd", tcpClientIp, tcpClientPort, Encoding.UTF8);
    }

    private void OnDisable()
    {
        servers.ForEach(s => s.OnDisable());
    }
}
