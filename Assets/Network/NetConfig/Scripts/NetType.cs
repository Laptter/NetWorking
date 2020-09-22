using System;
using UnityEngine;
using System.Collections;
using System.Net.Sockets;

//网络协议
public enum NetProtocol
{
	UDP,
    TCP
}

[Serializable]
public delegate void NetAcceptEvent(string message);
