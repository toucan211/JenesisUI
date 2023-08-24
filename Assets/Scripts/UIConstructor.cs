using Newtonsoft.Json;
using System;
using System.IO;
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

    void Start()
    {
        //string json = File.ReadAllText(jsonPath);
        //ConstructUI(json);
    }

    public void ConstructUI(string json)
    {
        try
        {
            UIElement rootElement = JsonConvert.DeserializeObject<UIElement>(json);
            ConstructUI(rootElement, CanvasObject.transform);
        }
        catch (System.Exception)
        {
            CorruptedJSONDetected();
        }
    }

    private void CorruptedJSONDetected()
    {

    }

    public void ConstructUI(UIElement element, Transform parentTransform)
    {
        GameObject newElement = new GameObject(element.name);
        newElement.transform.SetParent(parentTransform);
        newElement.transform.localPosition = element.position;
        RectTransform rectTransform = newElement.AddComponent<RectTransform>();
        rectTransform.sizeDelta = element.size;

        Image image = newElement.AddComponent<Image>();
        image.color = element.color.ToColor();

        if (element.type == "Button")
        {
            Button button = newElement.AddComponent<Button>();
            Text buttonText = CreateText(newElement.transform, element.text);
            button.targetGraphic = buttonText;
        }
        else if (element.type == "Text")
        {
            CreateText(newElement.transform, element.text);
        }

        // Create children recursively
        foreach (UIElement child in element.children)
        {
            ConstructUI(child, newElement.transform);
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
