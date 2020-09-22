using System;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class NetworkFunction
{
    private IPAddress Ip;
    private int Port;
    private string SendMessage;
    private string AcceptMessage;
    private bool IsHex;

    private Dictionary<string,Thread> ThreadDic;
    private Dictionary<string, Socket> SocketDic;

    public NetworkFunction(string ip, int port, string message,bool isHex = false)
    {
        Ip = IPAddress.Parse(ip);
        Port = port;
        SendMessage = message;
        IsHex = isHex;
        ThreadDic = new Dictionary<string, Thread>();
        SocketDic = new Dictionary<string, Socket>();
    }


    public void UDPSend()
    {
        Socket sendScoket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        EndPoint sendPoint = new IPEndPoint(Ip, Port);   //目标节点
        if (IsHex)
        {
            byte[] data = StringToByteArray.strsToHexByte(SendMessage);
            sendScoket.SendTo(data, sendPoint);   //UDP使用SendTo()方法发送数据
            sendScoket.Close();
            Debug.Log("发送16进制UDP数据：<color=green>" + SendMessage + "</color> to " + sendPoint + "\n");
        }
        else
        {
            byte[] data = Encoding.ASCII.GetBytes(SendMessage);
            sendScoket.SendTo(data, sendPoint);   //UDP使用SendTo()方法发送数据
            sendScoket.Close();
            Debug.Log("发送UDP数据：<color=green>" + SendMessage + "</color> to " + sendPoint + "\n");
        }
    }

    public void TCPSend()
    {
        Socket sendScoket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        EndPoint sendPoint = new IPEndPoint(Ip, Port);   //目标节点
        sendScoket.Connect(sendPoint);
        if (IsHex)
        {
            byte[] data = StringToByteArray.strsToHexByte(SendMessage);
            sendScoket.Send(data);   //TCP使用Send()方法发送数据
            sendScoket.Close();
            Debug.Log("发送16进制TCP数据：<color=green>" + SendMessage + "</color> to " + sendPoint + "\n");
        }
        else
        {
            byte[] data = Encoding.ASCII.GetBytes(SendMessage);
            sendScoket.Send(data);   //TCP使用Send()方法发送数据
            sendScoket.Close();
            Debug.Log("发送TCP数据：<color=green>" + SendMessage + "</color> to " + sendPoint + "\n");
        }
    }

    public void StartTCPAccept(NetAcceptEvent tcpEvent)
    {
        Socket acceptSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        Debug.Log(Port);
        EndPoint acceptPoint = new IPEndPoint(Ip, Port);
        Debug.Log("开始TCP监听：" + acceptPoint + "\n");
        try
        {
            acceptSocket.Bind(acceptPoint);
        }
        catch (SocketException se)
        {
            acceptSocket.Close();
            Debug.LogError(se);
            throw;
        }

        acceptSocket.Listen(100);    //开始监听并设置最大连接数

        Thread t = new Thread(() => TCPReceive(acceptSocket, tcpEvent));
        ThreadDic.Add(acceptSocket.ToString() , t);
        SocketDic.Add(acceptSocket.ToString(), acceptSocket);
        t.IsBackground = true;
        t.Start();
    }

    private void TCPReceive(Socket acceptSocket, NetAcceptEvent tcpEvent)
    {
        while (true)
        {
            Socket client = acceptSocket.Accept();
            Debug.Log(client.RemoteEndPoint.ToString());
            Thread t = new Thread(() => ReceiveMessage(client, tcpEvent));
            ThreadDic.Add(client.RemoteEndPoint.ToString(),t);
            SocketDic.Add(client.RemoteEndPoint.ToString(),client);
            t.IsBackground = true;
            t.Start();
        }
    }

    private void ReceiveMessage(Socket client, NetAcceptEvent tcpEvent)
    {
        while (true)
        {
            byte[] data = new byte[1024 * 1024];

            int length = -1;
            try
            {
                length = client.Receive(data);
            }
            catch (SocketException se)
            {
                ThreadDic.Remove(client.RemoteEndPoint.ToString());
                client.Close();
                SocketDic.Remove(client.RemoteEndPoint.ToString());
                Debug.LogError(se);
                break;
            }
            catch (Exception e)
            {
                ThreadDic.Remove(client.RemoteEndPoint.ToString());
                client.Close();
                SocketDic.Remove(client.RemoteEndPoint.ToString());
                Debug.LogError(e);
                break;
            }

            string message = Encoding.ASCII.GetString(data, 0, length);
           
            tcpEvent(message);
            Debug.Log("接收到TCP消息：" + message + " from " + client.RemoteEndPoint.ToString());
        }
    }

    public void StartUDPAccept(NetAcceptEvent udpEvent)
    {
        Socket acceptScoket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        EndPoint acceptPoint = new IPEndPoint(Ip, Port);
        Debug.Log("开始UDP监听：" + acceptPoint + "\n");
        try
        {
            acceptScoket.Bind(acceptPoint);
        }
        catch (SocketException se)
        {
            acceptScoket.Close();
            Debug.LogError(se);
            throw;
        }

        Thread t = new Thread(() => UDPReceive(acceptScoket, udpEvent));
        ThreadDic.Add(acceptScoket.ToString(),t);
        SocketDic.Add(acceptScoket.ToString(),acceptScoket);
        t.IsBackground = true;
        t.Start();
    }

    private void UDPReceive(Socket scoket, NetAcceptEvent udpEvent)
    {
        while (true)
        {
            EndPoint remoteEndpoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] data = new byte[1024 * 1024];
            int length = -1;
            try
            {
                length = scoket.ReceiveFrom(data, ref remoteEndpoint);
            }
            catch (SocketException se)
            {
                ThreadDic.Remove(scoket.ToString());
                scoket.Close();
                SocketDic.Remove(scoket.ToString());
                Debug.LogError(se);
                break;
            }

            string message = Encoding.ASCII.GetString(data, 0, length);
            udpEvent(message);
            Debug.Log("接收到UDP消息：" + message + " from " + remoteEndpoint);
        }
    }

    public void Close()
    {
        foreach (var socket in SocketDic)
        {
            socket.Value.Close();
        }

        foreach (var thread in ThreadDic)
        {
            thread.Value.Abort();
        }
    }
}
