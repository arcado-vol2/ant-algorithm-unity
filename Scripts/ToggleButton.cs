using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToggleButton : MonoBehaviour
{
    private bool isPressed = false;
    private Image selfImage;
    private Color idleColor; 
    private Color pressedColor;
    [SerializeField]
    private string idleText;
    [SerializeField]
    private string pressedText;
    [SerializeField]
    private TextMeshProUGUI label;

    private void Start()
    {
        Button selfButtonComponent = GetComponent<Button>();
        idleColor = selfButtonComponent.colors.normalColor;
        pressedColor = selfButtonComponent.colors.pressedColor;
        selfImage = GetComponent<Image>();
        idleText = label.text;
    }

    public void OnPressed()
    {
        selfImage.color = isPressed ? idleColor : pressedColor;
        label.text = isPressed ? idleText : pressedText;
        isPressed = !isPressed;
    }
}
