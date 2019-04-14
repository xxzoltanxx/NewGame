using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.Xml;
using System.IO;

public class ParametersDDOL : MonoBehaviour
{
    public void Awake()
    {
        Load("Parameters");
    }
    public void Load(string path)
    {
        TextAsset _xml = Resources.Load<TextAsset>("Parameters");

        XmlSerializer serializer = new XmlSerializer(typeof(Parameters));

        StringReader reader = new StringReader(_xml.text);

        parameters = serializer.Deserialize(reader) as Parameters;

        reader.Close();
    }
    public Parameters parameters;
}

[XmlRoot("PARAMETERS")]
public class Parameters
{
    [XmlArray("VILLAGES")]
    [XmlArrayItem("VILLAGE")]
    public List<XmlVillage> villages = new List<XmlVillage>();
}
public class XmlVillage
{
    [XmlElement("NAME")]
    public string name;
}
