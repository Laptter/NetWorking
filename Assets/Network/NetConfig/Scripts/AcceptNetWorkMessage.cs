using System;
using UnityEngine;
using System.Collections;
using System.Collections.Specialized;
using System.Net;
using System.Net.Sockets;

public class AcceptNetWorkMessage : MonoBehaviour {

    public XMLConfig Config;

    public NetProtocol NetType = NetProtocol.UDP;

    public string LocalIpConfigName;
    public string LocalPortConfigName;

    private string LocalIp;
    private int LocalPort;
    private bool AutoLocalIp = false;
    private NetworkFunction NetFunction;

    public NetAcceptEvent NetEvent ;

    // Use this for initialization
    void Start()
    {
        AutoLocalIp = Config.GetBool("AutoLocalIp");
        if (AutoLocalIp)
        {
            LocalIp = GetLocalIpv4()[0];
        }
        else
        {
            LocalIp = Config.GetString(LocalIpConfigName);
        }
        
        LocalPort = Config.GetInt(LocalPortConfigName);

        NetFunction = new NetworkFunction(LocalIp, LocalPort, "");

        NetEvent += AcceptMessage;
        switch (NetType)
        {
            case NetProtocol.TCP:
                NetFunction.StartTCPAccept(NetEvent);
                break;
            case NetProtocol.UDP:
                NetFunction.StartUDPAccept(NetEvent);
                break;
        }
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

    // Update is called once per frame
    void LateUpdate ()
	{

	}

    void OnApplicationQuit()
    {
        NetFunction.Close();
    }

    private void AcceptMessage(string message)
    {
        Debug.Log(message);
    }
}
