using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using UnityEngine.UI;

public class SendSocket : MonoBehaviour
{
    public static SendSocket Instance;

    public XMLConfig Config;

    private string IP;
    private IPAddress targetIP;
    private IPEndPoint myServer;
    private UdpClient udpsend;
    private int port;

    void Start()
    {
        Instance = this;

        bool autoip = Config.GetBool("AutoIp");

        if (autoip)
        {
            IP = GetLocalIpv4()[0];
        }
        else
        {
            IP = Config.GetString("SendIp");
        }
        port = Config.GetInt("SendPort");

        Debug.Log("Send: IP=" + IP + ";Port=" + port);

        

    }


    private string[] GetLocalIpv4()
    {
        //事先不知道ip的个数，数组长度未知，因此用StringCollection储存  
        IPAddress[] localIPs;
        localIPs = Dns.GetHostAddresses(Dns.GetHostName());
        StringCollection IpCollection = new StringCollection();
        foreach (IPAddress ip in localIPs)
        {
            //根据AddressFamily判断是否为ipv4,如果是InterNetWorkV6则为ipv6  
            if (ip.AddressFamily == AddressFamily.InterNetwork)
                IpCollection.Add(ip.ToString());
        }
        string[] IpArray = new string[IpCollection.Count];
        IpCollection.CopyTo(IpArray, 0);
        return IpArray;
    }

    /// <summary>
    /// 发送信息
    /// </summary>
    /// <param name="message"></param>
    public void SocketSendMessage(string message)
    {
        //开启socket
        targetIP = IPAddress.Parse(IP);
        myServer = new IPEndPoint(targetIP, port);
        udpsend = new UdpClient();

        message += "$";

        Byte[] myByte = new Byte[64];
        myByte = System.Text.Encoding.ASCII.GetBytes(message.ToCharArray());
        udpsend.Send(myByte, myByte.Length, myServer);
    }
}
