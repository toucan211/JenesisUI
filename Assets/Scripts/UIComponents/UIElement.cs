using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UIElement
{
    public string name;
    public string type;
    public Vector2 position;
    public Vector2 size;
    public SerializableColor color;
    public string text;
    public List<UIElement> children;
    public List<int> address;

    public UIElement()
    {

    }
}