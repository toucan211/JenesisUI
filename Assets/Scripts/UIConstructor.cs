using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIConstructor : MonoBehaviour
{
    public string jsonPath;
    private GameObject canvasObject;

    public GameObject CanvasObject
    {
        get
        {
            if (canvasObject == null)
            {
                canvasObject = new GameObject("Canvas");
                Canvas canvas = CanvasObject.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObject.AddComponent<CanvasScaler>();
                canvasObject.AddComponent<GraphicRaycaster>();
            }
            return canvasObject;
        }
        set => canvasObject = value;
    }

    public bool ConstructUI(string json)
    {
        try
        {
            UIElement rootElement = JsonConvert.DeserializeObject<UIElement>(json);
            ConstructUI(rootElement, CanvasObject.transform,new List<int>());
            return true;
        }
        catch (System.Exception e)
        {
            CorruptedJSONDetected(e);
            return false;
        }
    }

    private void CorruptedJSONDetected(System.Exception e)
    {
        print($"Invalid JSON: Message:{e.Message}\n StackTrace:{e.StackTrace}");
    }

    public void ConstructUI(UIElement element, Transform parentTransform, List<int> address)
    {
        GameObject newElement = new GameObject($"{element.name}_{string.Join("-", address)}");
        newElement.transform.SetParent(parentTransform);
        newElement.transform.localPosition = element.position;
        RectTransform rectTransform = newElement.AddComponent<RectTransform>();
        rectTransform.sizeDelta = element.size;

        Image image = newElement.AddComponent<Image>();
        image.color = element.color.ToColor();
        if (!string.IsNullOrEmpty(element.imagePath))
            image.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(element.imagePath);

        if (element.UIType == UIType.Button)
        {
            Button button = newElement.AddComponent<Button>();
            Text buttonText = CreateText(newElement.transform, element.text);
            button.targetGraphic = buttonText;
        }
        else if (element.UIType == UIType.Text)
        {
            CreateText(newElement.transform, element.text);
        }


        for (int i = 0; i < element.children.Count; i++)
        {
            UIElement child = element.children[i];
            address.Add(i);
            ConstructUI(child, newElement.transform, address);
            address.RemoveAt(address.Count - 1);
        }
    }

    Text CreateText(Transform parent, string textContent)
    {
        GameObject textObject = new GameObject("Text");
        textObject.transform.SetParent(parent);
        RectTransform textRect = textObject.AddComponent<RectTransform>();
        textRect.anchoredPosition = Vector2.zero;
        textRect.sizeDelta = parent.GetComponent<RectTransform>().sizeDelta;

        Text text = textObject.AddComponent<Text>();
        text.text = textContent;
        text.color = Color.black;
        text.alignment = TextAnchor.MiddleCenter;
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");

        return text;
    }
}
