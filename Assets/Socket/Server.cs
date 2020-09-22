using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class Server
{
    private IPAddress ip;
    private int port;
    private Dictionary<string, Thread> ThreadDic;
    private Dictionary<string, Socket> SocketDic;
    private Socket acceptSocket;
    private bool flag = true;
    public Server(string ip,int port, ProtocolType type)
    {
        this.ip = IPAddress.Parse(ip);
        this.port = port;
        acceptSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, type);
        ThreadDic = new Dictionary<string, Thread>();
        SocketDic = new Dictionary<string, Socket>();
    }

    public void Initialize(Action<byte[],int> call)
    {
       
        EndPoint acceptPoint = new IPEndPoint(ip, port);
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
        acceptSocket.Listen(100);

        Thread t = new Thread(() => TCPReceive(acceptSocket, call));
        ThreadDic.Add(acceptSocket.ToString(), t);
        SocketDic.Add(acceptSocket.ToString(), acceptSocket);
        t.IsBackground = true;
        t.Start();
    }


    private void TCPReceive(Socket acceptSocket, Action<byte[],int> call)
    {
        while (flag)
        {
            Socket client = acceptSocket.Accept();
            Debug.Log(client.RemoteEndPoint.ToString());
            Thread t = new Thread(() => ReceiveMessage(client, call));
            ThreadDic.Add(client.RemoteEndPoint.ToString(), t);
            SocketDic.Add(client.RemoteEndPoint.ToString(), client);
            t.IsBackground = true;
            t.Start();
        }
    }

    private void ReceiveMessage(Socket client, Action<byte[],int> call)
    {
        while (flag)
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
            //string message = Encoding.ASCII.GetString(data, 0, length);
            call(data,length);
            //Debug.Log("接收到TCP消息：" + message + " from " + client.RemoteEndPoint.ToString());
        }
    }

    public void OnDisable()
    {
        flag = false;
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
