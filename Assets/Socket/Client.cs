using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Client 
{
    
    Socket sendScoket;
    public IPAddress targetIp;
    public int targetPort;

    public Client(string targetIp,int targetPort, SocketType socketType, ProtocolType protocolType)
    {
        this.targetIp = IPAddress.Parse(targetIp);
        this.targetPort = targetPort;
        sendScoket = new Socket(AddressFamily.InterNetwork, socketType, protocolType);
    }

    public void SendToDefault(string message)
    {
        EndPoint sendPoint = new IPEndPoint(targetIp, targetPort);   //目标节点
        if(!sendScoket.Connected)
            sendScoket.Connect(sendPoint);
        byte[] data = Encoding.ASCII.GetBytes(message);
        sendScoket.SendTo(data, sendPoint);
        
    }

    public void SendToDefault(byte[] bytes)
    {
        EndPoint sendPoint = new IPEndPoint(targetIp, targetPort);   //目标节点
        if (!sendScoket.Connected)
            sendScoket.Connect(sendPoint);
        sendScoket.SendTo(bytes, sendPoint);
        
    }


    public void SendToTarget(string message, string ip, int port)
    {
        
        EndPoint sendPoint = new IPEndPoint(IPAddress.Parse(ip), port);   //目标节点
        if (!sendScoket.Connected)
            sendScoket.Connect(sendPoint);
        byte[] data = Encoding.ASCII.GetBytes(message);
        sendScoket.SendTo(data, sendPoint);
        
    }

 

    public void SendToTarget(byte[] bytes, string ip, int port)
    {
        EndPoint sendPoint = new IPEndPoint(IPAddress.Parse(ip), port);   //目标节点
        if (!sendScoket.Connected)
            sendScoket.Connect(sendPoint);
        sendScoket.SendTo(bytes, sendPoint);
       
    }

    public void OnDisable()
    {
        if (sendScoket != null && sendScoket.Connected)
        {
            sendScoket.Shutdown(SocketShutdown.Both);
            
            sendScoket.Close();
        }
    }
}
