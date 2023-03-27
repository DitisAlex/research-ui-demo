using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BarScript : MonoBehaviour
{
    [SerializeField] private UIDocument _UIDocument;
    [SerializeField] private float maxHp;
    [SerializeField] private string labelName;
    [Space][SerializeField] private string cssName;
    [SerializeField] private Color niceMaxHealthColor;

    [SerializeField] private Color nicetextColor;
    private ProgressBar _progressBar;
    private Label _niceLabel;
    private float _currentHp;

    // Start is called before the first frame update
    void Start()
    {
        var root = _UIDocument.rootVisualElement;
        _progressBar = root.Q<ProgressBar>(cssName);
        if (_progressBar == null)
        {
            throw new NotImplementedException("There is no css progress-bar named " + cssName);
        }
        _niceLabel = _progressBar.Q<Label>();
        _currentHp = maxHp;
        SetBackgroundColorBar(niceMaxHealthColor, nicetextColor);
        UpdateHpBarUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            UpdateHp(-10);

        }
        else if (Input.GetKeyDown(KeyCode.H))
        {
            UpdateHp(10);
        }
    }


    void UpdateHp(float hpDiff)
    {
        if (_currentHp + hpDiff < 0 || _currentHp + hpDiff > 100)
        {
            return;
        }
        _currentHp += hpDiff;
        UpdateHpBarUI();
    }

    void UpdateHpBarUI()
    {
        _progressBar.SetValueWithoutNotify(_currentHp);
        _niceLabel.text = labelName + ": " + _currentHp;
    }

    void SetBackgroundColorBar(Color color, Color textColor)
    {
        _progressBar[0][0][0].style.backgroundColor = color;
        _progressBar.style.backgroundColor = color;
        _progressBar.Q<VisualElement>("unity-progress-bar").style.backgroundColor = color;
        _niceLabel.style.color = textColor;
    }


}
