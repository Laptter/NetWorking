using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SocketManager))]
public class SocketManagerEditor : Editor
{
    SocketManager socketManager;
    public override void OnInspectorGUI()
    {
        socketManager = target as SocketManager;

        //Tcp Server
        EditorGUI.BeginChangeCheck();
        bool bInitTcpServer = EditorGUILayout.Toggle("TcpServer", socketManager.bInitTcpServer);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(socketManager, "Toggle Tcp Server");
            EditorUtility.SetDirty(socketManager);
            socketManager.bInitTcpServer = bInitTcpServer;
        }

        if (bInitTcpServer)
        {
            DrawIpAndPort(ref socketManager.tcpServerIp,ref socketManager.tcpServerPort);
        }




        //Udp Server
        EditorGUI.BeginChangeCheck();
        bool bInitUdpServer = EditorGUILayout.Toggle("UdpServer", socketManager.bInitUdpServer);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(socketManager, "Toggle Udp Server");
            EditorUtility.SetDirty(socketManager);
            socketManager.bInitUdpServer = bInitUdpServer;
        }

        if (bInitUdpServer)
        {
            DrawIpAndPort(ref socketManager.udpServerIp, ref socketManager.udpServerPort);
        }

        //Tcp Client
        EditorGUI.BeginChangeCheck();
        bool bInitTcpClient = EditorGUILayout.Toggle("TcpClient", socketManager.bInitTcpClient);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(socketManager, "Toggle Udp Server");
            EditorUtility.SetDirty(socketManager);
            socketManager.bInitTcpClient = bInitTcpClient;
        }

        if (bInitTcpClient)
        {
            DrawIpAndPort(ref socketManager.tcpClientIp, ref socketManager.tcpClientPort);
        }

        //Udp Client
        EditorGUI.BeginChangeCheck();
        bool bInitUdpClient = EditorGUILayout.Toggle("UdpClient", socketManager.bInitUdpClient);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(socketManager, "Toggle Udp Server");
            EditorUtility.SetDirty(socketManager);
            socketManager.bInitUdpClient = bInitUdpClient;
        }

        if (bInitUdpClient)
        {
            DrawIpAndPort(ref socketManager.udpClientIp, ref socketManager.udpClientPort);
        }
    }


    private void DrawIpAndPort(ref string originIp,ref int originPort)
    {
        EditorGUI.BeginChangeCheck();
        string ip = EditorGUILayout.TextField("Ip", originIp);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(socketManager, "IP");
            EditorUtility.SetDirty(socketManager);
            originIp = ip;
        }

        EditorGUI.BeginChangeCheck();
        string port = EditorGUILayout.TextField("Port", originPort.ToString());
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(socketManager, "Port");
            EditorUtility.SetDirty(socketManager);
            originPort = int.Parse(port);
        }
    }

}
