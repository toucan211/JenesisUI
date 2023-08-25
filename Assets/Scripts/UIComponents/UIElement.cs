using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum UIType { Button, Text, Panel }

[System.Serializable]
public class UIElement
{
    public string name;
    public UIType UIType;
    public Vector2 position;
    public Vector2 size;
    public SerializableColor color;
    public string text;
    public int fontSize;
    public string imagePath;
    public Image.Type imageType;
    public List<UIElement> children = new List<UIElement>();
    public List<int> address = new List<int>();

    public void InsertUIElement(UIElement incomingElement)
    {
        List<UIElement> targetChildrenList = this.children;

        List<int> address = incomingElement.address;
        if (address.Count == 0) address.Add(0);

        int targetInsertIndex = address.Last();

        for (int i = 0; i < address.Count - 1; i++)
        {
            if (address[i] < targetChildrenList.Count)
                targetChildrenList = targetChildrenList[address[i]].children;
            else
                targetChildrenList = targetChildrenList[0].children;
        }

        if (targetInsertIndex <= targetChildrenList.Count)
        {
            targetChildrenList.Insert(targetInsertIndex, incomingElement);
        }
        else
        {
            Debug.LogWarning("Sibling order is greater than the children count. Appending to the end.");
            targetChildrenList.Add(incomingElement);
        }
    }

    public bool RemoveUIElement(List<int> address)
    {
        List<UIElement> targetChildrenList = this.children;
        // Debug.Log($"childrenCount:{this.children.Count}");

        if (address.Count == 0) return false;

        int targetDeleteIndex = address.Last();

        for (int i = 0; i < address.Count - 1; i++)
        {
            if (address[i] < targetChildrenList.Count)
                targetChildrenList = targetChildrenList[address[i]].children;
            else
            {
                return false;
            }
        }

        if (targetDeleteIndex <= targetChildrenList.Count)
        {
            targetChildrenList.RemoveAt(targetDeleteIndex);
        }
        else
        {
            ShowInvalidAddress();
            return false;
        }


        return true;
        void ShowInvalidAddress()
        {
            Debug.LogWarning("Invalid Address");
        }
    }
}