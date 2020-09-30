using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;


public struct UdpState
{
    public UdpClient u;
    public IPEndPoint e;
}
public class Server
{
    private IPAddress ip;
    private int port;
    private Dictionary<string, Thread> ThreadDic;
    private Dictionary<string, Socket> SocketDic;
    private Socket acceptSocket;
    private bool flag = true;

    public Server(string ip,int port,SocketType socketType, ProtocolType protocolType)
    {
        this.ip = IPAddress.Parse(ip);
        this.port = port;
        acceptSocket = new Socket(AddressFamily.InterNetwork, socketType, protocolType);
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
        

        Thread thread = null;

        if (acceptSocket.ProtocolType == ProtocolType.Tcp)
        {
            acceptSocket.Listen(10);
            thread = new Thread(() => TCPReceive(call));
        }
        else
        {
            thread = new Thread(() => UdpRecive(call));
        }

        ThreadDic.Add(acceptSocket.ToString(), thread);
        SocketDic.Add(acceptSocket.ToString(), acceptSocket);
        thread.IsBackground = true;
        thread.Start();
    }


    private void TCPReceive(Action<byte[],int> call)
    {
        while (flag)
        {
            Socket client = acceptSocket.Accept();
            Thread t = new Thread(() => ReceiveMessage(client, call));
            ThreadDic.Add(client.RemoteEndPoint.ToString(), t);
            SocketDic.Add(client.RemoteEndPoint.ToString(), client);
            t.IsBackground = true;
            t.Start();
        }
    }


    private void UdpRecive(Action<byte[], int> call)
    {
        while (flag)
        {
            EndPoint remoteEndpoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] data = new byte[2048];
            int length = -1;
            try
            {
                length = acceptSocket.ReceiveFrom(data,ref remoteEndpoint);
            }
            catch (SocketException se)
            {
                ThreadDic.Remove(acceptSocket.ToString());
                acceptSocket.Close();
                SocketDic.Remove(acceptSocket.ToString());
                Debug.LogError(se);
                break;
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
            call(data,length);
        }
    }


    private void ReceiveMessage(Socket client, Action<byte[],int> call)
    {
        while (flag)
        {
            byte[] data = new byte[2048];
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
           
            if (length == 0)
            {
                ReaseResources(client);
            }
        }
    }

    /// <summary>
    /// 按照客户端端口号释放线程占用的资源
    /// </summary>
    /// <param name="client"></param>
    private void ReaseResources(Socket client)
    {
        if (client != null && client.Connected)
        {
            var key = client.RemoteEndPoint.ToString();
            SocketDic.Remove(key);
            client.Shutdown(SocketShutdown.Both);
            client.Close();
            Debug.Log("1");
            Thread t = ThreadDic[key];
            ThreadDic.Remove(key);
            t.Abort();
        }
    }

    public void OnDisable()
    {
        flag = false;
        foreach (var socket in SocketDic)
        {
            if (socket.Value != null && socket.Value.Connected)
            {
                socket.Value.Shutdown(SocketShutdown.Both);
                socket.Value.Close();
            }
        }



        foreach (var thread in ThreadDic)
        {
            if (thread.Value != null)
            {
                thread.Value.Abort();
            }
        }

        SocketDic.Clear();
        ThreadDic.Clear();
    }
}
