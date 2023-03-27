using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MenuController : MonoBehaviour
{
    private UIDocument _doc;
    private Button _playButton;
    private Button _settingsButton;
    private Button _exitButton;

    //[SerializeField]
    //private VisualTreeAsset _settingsButtonsTemplate;
    //private VisualElement _settingsButtons;
    //private VisualElement _buttonContainer;

    //private Button _backButton;
    //private Button _settingAButton;
    //private Button _settingBButton;
    //private Button _settingCButton;

    private void Awake()
    {
        _doc = GetComponent<UIDocument>();
        _playButton = _doc.rootVisualElement.Q<Button>("PlayButton");
        _settingsButton = _doc.rootVisualElement.Q<Button>("SettingsButton");
        _exitButton = _doc.rootVisualElement.Q<Button>("ExitButton");

        _playButton.clicked += playButtonClicked;
        _settingsButton.clicked += settingsButtonClicked;
        _exitButton.clicked += exitButtonClicked;

        //_settingsButtons = _settingsButtonsTemplate.CloneTree();
        //_backButton = _settingsButtons.Q<Button>("BackButton");
        //_settingAButton = _settingsButtons.Q<Button>("SettingAButton");
        //_settingBButton = _settingsButtons.Q<Button>("SettingBButton");
        //_settingCButton = _settingsButtons.Q<Button>("SettingCButton");
        //_buttonContainer = _doc.rootVisualElement.Q<VisualElement>("Buttons");

        //_backButton.clicked += backButtonClicked;
        //_settingAButton.clicked += changeSettingClicked;
        //_settingBButton.clicked += changeSettingClicked;
        //_settingCButton.clicked += changeSettingClicked;
    }

    private void playButtonClicked()
    {
        Debug.Log("Play Game!");
    }

    private void settingsButtonClicked()
    {
        Debug.Log("Settings screen!");

        //_buttonContainer.Clear();
        //_buttonContainer.Add(_settingsButtons);
    }

    private void exitButtonClicked()
    {
        Debug.Log("Quit Game!");
    }

    //private void backButtonClicked()
    //{
    //    Debug.Log("Back To Main Menu!");

    //    _buttonContainer.Clear();
    //    _buttonContainer.Add(_playButton);
    //    _buttonContainer.Add(_settingsButton);
    //    _buttonContainer.Add(_exitButton);
    //}

    //private void changeSettingClicked()
    //{
    //    Debug.Log("Setting adjusted!");
    //}
}
