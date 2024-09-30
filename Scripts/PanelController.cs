using System.Collections;
using TMPro;
using UnityEngine;

public class PanelController : MonoBehaviour
{
    [SerializeField] 
    private Vector2 startPosition;
    [SerializeField]
    private Vector2 endPosition;
    [SerializeField]
    private float moveDuration = 1.0f;
    private bool open = false;
    [SerializeField]
    private RectTransform selfRect;
    public void OnOpenButtonClick()
    {
        if (open)
        {
            StartCoroutine(MovePanelTo(endPosition));
        }
        else
        {
            StartCoroutine(MovePanelTo(startPosition));
        }
        open = !open;
    }

    private IEnumerator MovePanelTo(Vector2 target)
    {
        Vector2 startPos = selfRect.anchoredPosition;
        float elapsedTime = 0.0f;

        while (elapsedTime < moveDuration)
        {
           
            selfRect.anchoredPosition = Vector2.Lerp(startPos, target, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        selfRect.anchoredPosition = target; 
    }
    public void UpdateLabel(TextMeshProUGUI label, int value, string startText)
    {
        label.text = startText + value.ToString();
    }

}
