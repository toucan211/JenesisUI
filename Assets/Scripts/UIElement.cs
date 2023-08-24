using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UIElement
{
    public string name;
    public Vector2 position;
    public Vector2 size;
    public SerializableColor color;
    public List<UIElement> children = new List<UIElement>();
}
