using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class Client 
{
    Socket sendScoket;
    public IPAddress default_ip;
    public int default_port;

    public Client(string defaultIp,int defaultPort, ProtocolType type)
    {
        this.default_ip = IPAddress.Parse(defaultIp);
        this.default_port = defaultPort;
        sendScoket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, type);
    }

    public void SendToAll(string message, Encoding encode)
    {
        EndPoint sendPoint = new IPEndPoint(default_ip, default_port);   //目标节点
        sendScoket.Connect(sendPoint);
        byte[] data = encode.GetBytes(message);
        sendScoket.SendTo(data, sendPoint);
    }
    public void SendToOthers(string message, Encoding encode)
    {
        EndPoint sendPoint = new IPEndPoint(default_ip, default_port);   //目标节点
        sendScoket.Connect(sendPoint);
        byte[] data = encode.GetBytes(message);
        sendScoket.SendTo(data, sendPoint);
    }

    public void SendToTarget(string message, string ip, int port, Encoding encode)
    {
        EndPoint sendPoint = new IPEndPoint(IPAddress.Parse(ip), port);   //目标节点
        sendScoket.Connect(sendPoint);
        byte[] data = encode.GetBytes(message);
        sendScoket.SendTo(data, sendPoint);
    }

    public void SendToAll(byte[] bytes)
    {
        EndPoint sendPoint = new IPEndPoint(default_ip, default_port);   //目标节点
        sendScoket.Connect(sendPoint);
        sendScoket.SendTo(bytes, sendPoint);
    }
    public void SendToOthers(byte[] bytes, string ip, int port)
    {
        EndPoint sendPoint = new IPEndPoint(default_ip, default_port);   //目标节点
        sendScoket.Connect(sendPoint);
        sendScoket.SendTo(bytes, sendPoint);
    }

    public void SendToTarget(byte[] bytes, string ip, int port)
    {
        //如果每次send一次都connetc一次，是不合理的
        EndPoint sendPoint = new IPEndPoint(IPAddress.Parse(ip), port);   //目标节点
        sendScoket.Connect(sendPoint);
        sendScoket.SendTo(bytes, sendPoint);
    }

    public void OnDisable()
    {
        sendScoket.Shutdown(SocketShutdown.Both);
        sendScoket.Close();
    }
}
