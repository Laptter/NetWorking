using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml.Linq;
using UnityEngine;
[DisallowMultipleComponent]
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

    public Queue<string> TcpMessageQueue => tcpMessageQueue;
    private Queue<string> tcpMessageQueue = new Queue<string>();
    public Queue<string> UdpMessageQueue => udpMessageQueue;
    private Queue<string> udpMessageQueue = new Queue<string>();


    Server tcpServer;
    Server udpServer;
    Client tcpClient;
    Client udpClient;

    private void Awake()
    {
        InitilizeConfig();
    }

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


    private void InitilizeConfig()
    {

        var fullPath = System.IO.Path.Combine(Application.streamingAssetsPath, "Config.xml");
        XElement root = XElement.Load(fullPath);

        bool autoLocalIp = bool.Parse(root.Element("autoLocalIp").Value);

        if (autoLocalIp)
            tcpServerIp = GetLocalIPAddress();
        else
            tcpServerIp = root.Element("tcpServerIp").Value;
        tcpServerPort = int.Parse(root.Element("tcpServerPort").Value);



        if (autoLocalIp)
            udpServerIp = GetLocalIPAddress();
        else
            udpServerIp = root.Element("udpServerIp").Value;
        udpServerPort = int.Parse(root.Element("udpServerPort").Value);

        tcpClientIp = root.Element("tcpClientIp").Value;
        tcpClientPort = int.Parse(root.Element("tcpClientPort").Value);

        udpClientIp = root.Element("udpClientIp").Value;
        udpClientPort = int.Parse(root.Element("udpClientPort").Value);
    }


    public void InitTcpServer()
    {
        tcpServer = new Server(tcpServerIp, tcpServerPort, SocketType.Stream, ProtocolType.Tcp);
        tcpServer.Initialize(ReciveTcpBytes);
    }


    private void ReciveTcpBytes(byte[] bytes, int length)
    {
        if (length > 0)
        {
            Array.Resize(ref bytes, length);
            var message = System.Text.Encoding.ASCII.GetString(bytes).TrimEnd('\0');
            tcpMessageQueue.Enqueue(message);
            Debug.Log("tcp recive--"+message);
        }
    }

    private void ReciveUdpBytes(byte[] bytes, int length)
    {
        if (length > 0)
        {
            Array.Resize(ref bytes, length);
            var message = System.Text.Encoding.ASCII.GetString(bytes).TrimEnd('\0');
            udpMessageQueue.Enqueue(message);
            Debug.Log("udp recive--" + message);
        }
    }


    private void InitUdpServer()
    {
        udpServer = new Server(udpServerIp, udpServerPort, SocketType.Dgram, ProtocolType.Udp);
        udpServer.Initialize(ReciveUdpBytes);
    }


    public void SendToDefault(string message, ProtocolType type = ProtocolType.Udp)
    {
        switch (type)
        {
            case ProtocolType.Udp:
                udpClient.SendToDefault(message);
                break;
            case ProtocolType.Tcp:
                tcpClient.SendToDefault(message);
                break;
        }
    }
    public void SendToDefault(byte[] bytes, ProtocolType type = ProtocolType.Udp)
    {
        switch (type)
        {
            case ProtocolType.Udp:
                udpClient.SendToDefault(bytes);
                break;
            case ProtocolType.Tcp:
                tcpClient.SendToDefault(bytes);
                break;
        }
    }
    public void SendToTarget(string message, string ip, int port, ProtocolType type = ProtocolType.Udp)
    {
        switch (type)
        {
            case ProtocolType.Udp:
                udpClient.SendToTarget(message, ip, port);
                break;
            case ProtocolType.Tcp:
                tcpClient.SendToTarget(message, ip, port);
                break;
        }
    }

 
    private void SendToTarget(byte[] bytes, string ip, int port, ProtocolType type = ProtocolType.Udp)
    {
        switch (type)
        {
            case ProtocolType.Udp:
                udpClient.SendToTarget(bytes, ip, port);
                break;
            case ProtocolType.Tcp:
                tcpClient.SendToTarget(bytes, ip, port);
                break;
        }
    }



    private void InitUdpClient()
    {
        udpClient = new Client(udpClientIp, udpClientPort, SocketType.Dgram, ProtocolType.Udp);
    }

    private void InitTcpClient()
    {
        tcpClient = new Client(tcpClientIp, tcpClientPort, SocketType.Stream, ProtocolType.Tcp);
    }

    private void OnDisable()
    {
        tcpServer?.OnDisable();
        udpServer?.OnDisable();
        tcpClient?.OnDisable();
        udpClient?.OnDisable();
    }

    public static string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new Exception("No network adapters with an IPv4 address in the system!");
    }


    public void  SaveToFile()
    {

        var fullPath = System.IO.Path.Combine(Application.streamingAssetsPath, "Config.xml");
        XElement root = XElement.Load(fullPath);       

        root.Element("tcpServerIp").Value = tcpServerIp;
        root.Element("tcpServerPort").Value = tcpServerPort.ToString();


        root.Element("udpServerIp").Value = udpServerIp;
        root.Element("udpServerPort").Value = udpServerPort.ToString();

        root.Element("tcpClientIp").Value = tcpClientIp;
        root.Element("tcpClientPort").Value = tcpClientPort.ToString();

        root.Element("udpClientIp").Value = udpClientIp;
        root.Element("udpClientPort").Value = udpClientPort.ToString();

        root.Save(fullPath);

        
    }
}
