using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AlgoPannelController : PanelController
{
    public TextMeshProUGUI pathSizeLabel;
    public TextMeshProUGUI iterationCountLabel;
    public TextMeshProUGUI antCountLabel;
    public Slider antCountSlider;
    public TextMeshProUGUI eliteAntCountLabel;
    public Slider eliteAntCountSlider;
    public TMP_InputField alphaInput;
    public TMP_InputField betaInput;
    public TMP_InputField qInput;
    public TMP_InputField roInput;
    public TMP_InputField startPheromoneInput;
    public TMP_InputField minPheromoneInput;


    public ToggleButton startAlgoButtonToggle;
    public Button createGraphButton;
    public Button deleteGraphButton;
    public Button startAlgoButton;

    public string startPathSizeText { get; private set; } = string.Empty;
    public string startIterationCountText { get; private set; } = string.Empty;
    public string startAntCountText { get; private set; } = string.Empty;
    public string startEliteAntCountText { get; private set; } = string.Empty;

    void Start()
    {
        startPathSizeText = pathSizeLabel.text;
        startIterationCountText = iterationCountLabel.text;
        startAntCountText = antCountLabel.text;
        startEliteAntCountText = eliteAntCountLabel.text;
        UpdateLabel(pathSizeLabel, 0, startPathSizeText);
        UpdateLabel(iterationCountLabel, 0, startIterationCountText);
        OnAntSizeSliderValueChanged();
        OnEliteAntSliderValueChanged();

    }
    public void OnAntSizeSliderValueChanged()
    {
        UpdateLabel(antCountLabel, (int)antCountSlider.value, startAntCountText);
        eliteAntCountSlider.maxValue = antCountSlider.value;
    }

    public void OnEliteAntSliderValueChanged()
    {
        UpdateLabel(eliteAntCountLabel, (int)eliteAntCountSlider.value, startEliteAntCountText);
    }

    public void OnCreateGraphButtonPressed()
    {
        startAlgoButton.interactable = !startAlgoButton.interactable;
    }
}
