using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class XMLConfig : MonoBehaviour
{
    public string ConfigFileName = "Config.xml"; // In StreamingAssets folder.
    public KeyCode ReloadKey = KeyCode.F5;

    private Dictionary<string, string> configDict;

    public bool GetBool(string key)
    {
        return GetBool(key, false);
    }

    public bool GetBool(string key, bool defaultVal)
    {
        bool val = defaultVal;
        if (configDict.ContainsKey(key))
        {
            try
            {
                bool.TryParse(configDict[key], out val);
            }
            catch (System.Exception)
            {
                Debug.LogError("Try to parse bool failed " + configDict[key]);
            }
        }

        return val;
    }

    public void SetBool(string key, bool val)
    {
        SetVal(key, val.ToString());
    }

    public int GetInt(string key)
    {
        return GetInt(key, 0);
    }

    public int GetInt(string key, int defaultVal)
    {
        int val = defaultVal;
        if (configDict.ContainsKey(key))
        {
            try
            {
                int.TryParse(configDict[key], out val);
            }
            catch (System.Exception )
            {
                Debug.LogError("Try to parse int failed " + configDict[key]);
            }
        }

        return val;
    }

    public void SetInt(string key, int val)
    {
        SetVal(key, val.ToString());
    }

    public float GetFloat(string key)
    {
        return GetFloat(key, 0);
    }

    public float GetFloat(string key, float defaultVal)
    {
        float val = defaultVal;
        if (configDict.ContainsKey(key))
        {
            try
            {
                float.TryParse(configDict[key], out val);
            }
            catch (System.Exception)
            {
                Debug.LogError("Try to parse float failed " + configDict[key]);
            }
        }

        return val;
    }

    public void SetFloat(string key, float val)
    {
        SetVal(key, val.ToString());
    }

    public string GetString(string key)
    {
        return GetString(key, "");
    }

    public string GetString(string key, string defaultVal)
    {
        string val = defaultVal;
        if (configDict.ContainsKey(key))
        {
            val = configDict[key];
        }

        return val;
    }

    public void SetString(string key, string val)
    {
        SetVal(key, val.ToString());
    }

    void Awake()
    {
        configDict = new Dictionary<string, string>();

        ParseConfig();
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(ReloadKey))
        {
            ParseConfig();
        }
    }

    void ParseConfig()
    {
        configDict.Clear();
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, ConfigFileName);
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(path);

        XmlNode root = xmlDoc.DocumentElement;

        XmlNodeList nodeList = root.ChildNodes;
        for (int i = 0; i < nodeList.Count; i++)
        {
            XmlNode curNode = nodeList[i];
            string key = curNode.Name;
            string val = curNode.InnerText;

            if (!configDict.ContainsKey(key))
            {
                configDict.Add(key, val);
            }
            else
            {
                Debug.LogError("Duplicated key " + key + " in config file " + path);
            }
        }
    }

    void SetVal(string key, string val)
    {
        if (configDict.ContainsKey(key))
        {
            configDict[key] = val;
        }
        else
        {
            configDict.Add(key, val);
        }

        UpdateXml(key, val);
    }

    void UpdateXml(string key, string val)
    {
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, ConfigFileName);
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(path);

        XmlNode root = xmlDoc.DocumentElement;

        XmlNodeList nodeList = root.ChildNodes;

        bool found = false;
        for (int i = 0; i < nodeList.Count; i++)
        {
            XmlNode curNode = nodeList[i];
            if (curNode.Name == key)
            {
                curNode.InnerText = val;
                found = true;
                break;
            }
        }

        if (!found)
        {
            XmlNode node = xmlDoc.CreateNode(XmlNodeType.Element, key, null);
            node.InnerText = val;
            root.AppendChild(node);
        }

        xmlDoc.Save(path);
        Debug.Log(xmlDoc.InnerXml);
    }
}