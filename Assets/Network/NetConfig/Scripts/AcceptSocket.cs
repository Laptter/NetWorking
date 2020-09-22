using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Text.RegularExpressions;

public class AcceptSocket : MonoBehaviour
{
    public static AcceptSocket Instance;

    public XMLConfig Config;
    public string Command;

    private static UdpClient reveive;
    private string IP;
    private bool acceptto = false;

    // Use this for initialization
    void Start()
    {
        Instance = this;

        bool autoip = Config.GetBool("AutoIp");

        if(autoip)
        {
            IP = GetLocalIpv4()[0];
        }
        else
        {
            IP = Config.GetString("AcceptIp");
        }
        int port = Config.GetInt("AcceptPort");

        Debug.Log("Accept: IP=" + IP + ";Port=" + port);

        IPAddress localIP = IPAddress.Parse(IP);
        IPEndPoint localIPEndPoint = new IPEndPoint(localIP, port);
        reveive = new UdpClient(localIPEndPoint);

        //启动接受线程
        Thread threadReceive = new Thread(ReceiveMessages);
        threadReceive.IsBackground = true;
        threadReceive.Start();

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

    void OnApplicationQuit()
    {
        reveive.Close();
    }

    // Update is called once per frame
    void Update()
    {
      
    }

    private void ReceiveMessages()
    {
        IPEndPoint remoteIPEndPoint = new IPEndPoint(IPAddress.Any, 0);
        while (true)
        {
            try
            {
                byte[] receiveBytes = reveive.Receive(ref remoteIPEndPoint);
                string message = System.Text.Encoding.ASCII.GetString(receiveBytes, 0, receiveBytes.Length);
                string[] date = message.Split('$');

                Command = date[0];

            }
            catch
            {
                Debug.Log("message=NULL");
                break;
            }
        }
    }

}
