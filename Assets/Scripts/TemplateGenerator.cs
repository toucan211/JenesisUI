using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TemplateGenerator : MonoBehaviour
{
    public UIElement Container;
    public bool add;
    public UIElement IncomingElement;
    public UIConstructor UIConstructor;

    public void InsertUIElement(UIElement incomingElement)
    {
        List<UIElement> targetChildrenList = Container.children;

        List<int> address = incomingElement.address;
        if (address.Count == 0)
            address.Add(0);

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

    private void OnValidate()
    {
        if (add)
        {
            var settings = new Newtonsoft.Json.JsonSerializerSettings();
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

            InsertUIElement(IncomingElement);
            UIConstructor.ConstructUI(JsonConvert.SerializeObject(Container, settings));
            add = false;
        }
    }
}
