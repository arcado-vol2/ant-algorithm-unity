using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VertexControll : MonoBehaviour
{
    public int index;
    [SerializeField]
    private TextMeshProUGUI label;
    void Start()
    {
        label.text = index.ToString();
    }
}
