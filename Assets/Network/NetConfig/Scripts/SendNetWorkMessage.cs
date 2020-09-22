using UnityEngine;

public class SendNetWorkMessage : MonoBehaviour
{
    public XMLConfig Config;

    public NetProtocol NetType = NetProtocol.UDP;

    public bool ReadXML = true;

    public string RemoteIpConfigName;
    public string RemotePortConfigName;

    private string RemoteIp;
    private int RemotePort;

    // Use this for initialization
    void Start ()
    {
        if (ReadXML)
        {
            RemoteIp = Config.GetString(RemoteIpConfigName);
            RemotePort = Config.GetInt(RemotePortConfigName);

            Debug.Log(RemoteIp);
        }
    }
	
	// Update is called once per frame
	void LateUpdate ()
	{

    }
    /// <summary>
    /// 发送指令
    /// </summary>
    /// <param name="message">需发送的信息</param>
    public void SocketSendMessage(string message ,bool isHex = false)
    {
        NetworkFunction netFunction = new NetworkFunction(RemoteIp, RemotePort, message,isHex);
        switch (NetType)
        {
            case NetProtocol.TCP:
                netFunction.TCPSend();
                break;
            case NetProtocol.UDP:
                netFunction.UDPSend();
                break;
        }
    }

    /// <summary>
    /// 发送指令
    /// </summary>
    /// <param name="ip">目标ip</param>
    /// <param name="port">目标端口</param>
    /// <param name="message">需发送的信息</param>
    public void SocketSendMessage(string ip,int port, string message,bool isHex = false)
    {
        RemoteIp = ip;
        RemotePort = port;
        
        SocketSendMessage(message,isHex);
    }

    public void SocketSendXMLMessage(string key,bool isHex = false)
    {
        string message = Config.GetString(key);
        NetworkFunction netFunction = new NetworkFunction(RemoteIp, RemotePort, message,isHex);
        switch (NetType)
        {
            case NetProtocol.TCP:
                netFunction.TCPSend();
                break;
            case NetProtocol.UDP:
                netFunction.UDPSend();
                break;
        }
    }

}

