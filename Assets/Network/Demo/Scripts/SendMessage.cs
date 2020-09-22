using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SendMessage : MonoBehaviour
{
    public SendNetWorkMessage SendMessade;
    public SendNetWorkMessage Send2;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void F_ButtonClick_SendMessage(InputField inputMessage)
    {
        SendMessade.SocketSendMessage(inputMessage.text);
        Send2.SocketSendXMLMessage("Open");
    }
}
