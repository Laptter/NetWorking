using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
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


    Server tcpServer;
    Server udpServer;
    Client tcpClient;
    Client udpClient;

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

        if (bInitUdpClient)
        {
            InitUdpClient();
        }
        
    }


    public void InitTcpServer()
    {
        tcpServer = new Server(tcpServerIp, tcpServerPort, ProtocolType.Tcp);
        tcpServer.Initialize(ReciveBytes);
    }


    private void ReciveBytes(byte[] bytes,int length)
    {
        if (length > 0)
        {
            Array.Resize(ref bytes, length);
            DataType data = FromByteArray<DataType>(bytes);
        }
    }

    public void InitUdpServer()
    {
        udpServer = new Server(udpServerIp, udpServerPort, ProtocolType.Udp);
        udpServer.Initialize(ReciveBytes);
    }

    public void InitUdpClient()
    {
        udpClient = new Client(udpClientIp, udpClientPort, ProtocolType.Udp);
    }

    public void InitTcpClient()
    {
        tcpClient = new Client(tcpClientIp,tcpClientPort, ProtocolType.Tcp);
    }

    private void OnDisable()
    {
        tcpServer?.OnDisable();
        udpServer?.OnDisable();
        tcpClient?.OnDisable();
        udpClient?.OnDisable();
    }


    public byte[] ToByteArray<T>(T obj)
    {
        if (obj == null)
            return null;
        BinaryFormatter bf = new BinaryFormatter();
        using (MemoryStream ms = new MemoryStream())
        {
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }
    }

    public T FromByteArray<T>(byte[] data)
    {
        if (data == null)
            return default(T);
        BinaryFormatter bf = new BinaryFormatter();
        using (MemoryStream ms = new MemoryStream(data))
        {
            object obj = bf.Deserialize(ms);
            return (T)obj;
        }
    }
}
