using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShowReceiveMessage : MonoBehaviour
{
    public AcceptNetWorkMessage AcceptNet;

    private string ReceiveMessage = "";

	// Use this for initialization
	void Start ()
	{
        AcceptNet.NetEvent += ShowMessage;
    }
	
	// Update is called once per frame
	void Update () {
	    if (ReceiveMessage != "")
	    {
            this.GetComponent<Text>().text = "Receive  Message：" + ReceiveMessage;
	        ReceiveMessage = "";
	    }
	}

    private void ShowMessage(string message)
    {
        //this.GetComponent<Text>().text = "Receive  Message：" + message;
        ReceiveMessage = message;
    }
}
