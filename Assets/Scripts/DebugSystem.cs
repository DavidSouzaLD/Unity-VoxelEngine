using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugSystem : MonoBehaviour
{
    [System.Serializable]
    public class Info
    {
        public string name;
        public string value;

        public Info(string _name, string _value)
        {
            name = _name;
            value = _value;
        }
    }

    public TextMeshProUGUI textMesh;
    private static List<Info> infos = new List<Info>();

    private void LateUpdate()
    {
        string fullInfo = "";

        for (int i = 0; i < infos.Count; i++)
        {
            fullInfo += infos[i].value + "\n";
        }

        textMesh.text = fullInfo;
    }

    public static void AddInfo(string _name, string _value)
    {
        for (int i = 0; i < infos.Count; i++)
        {
            if (infos[i].name == _name)
            {
                infos[i].value = _value;
                return;
            }
        }

        infos.Add(new Info(_name, _value));
    }

    public static void RemoveInfo(string _name)
    {
        for (int i = 0; i < infos.Count; i++)
        {
            if (infos[i].name == _name)
            {
                infos.Remove(infos[i]);
                return;
            }
        }
    }
}