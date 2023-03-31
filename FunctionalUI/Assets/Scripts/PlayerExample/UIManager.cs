using System.Collections;
using PlayerExample;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable InconsistentNaming

public class UIManager : MonoBehaviour
{
    [SerializeField] private PlayerManager playerManager;
    private bool _canHeal = true;
    private Button _currentSideScreenButton;

    //Sidebar (inventory etc.)
    public static string SidebarTitle = "Inventory";
    private int _coinCount;
    
    private VisualElement _currentSideScreenContent;

    private VisualElement _documentContainer;
    private Button _healButton;

    private VisualElement _hitScreen;

    //Declaring UI elements
    private ProgressBar _hpBar;
    private Label _hpBarLabel;
    private Button _menuButton;
    private Label _messageLabel;
    private Label _coinAmountLabel;
    private ProgressBar _staminaBar;
    private VisualElement _sidebarTitle;

    // Start is called before he first frame update
    private void Awake()
    {
        //This is a container with all the UI documents, like the Inventory part, the menu part and the HUD 
        _documentContainer = GetRootDocument();

        //Get the root element of the UIDocument
        var hudDocumentRoot = _documentContainer.Q("PlayerHUDDocument");

        //Get the HPBar inside the root element
        _hpBar = hudDocumentRoot.Q<ProgressBar>("hp-bar");

        //Get the label inside the HPBar
        _hpBarLabel = _hpBar.Q<Label>();

        //Get the label to display messages on screen
        _messageLabel = hudDocumentRoot.Q<Label>("message-label");
        _coinAmountLabel = hudDocumentRoot.Q<Label>("coin-amount-label");
        _hitScreen = hudDocumentRoot.Q<VisualElement>("hit-screen");

        //Heal button
        _healButton = hudDocumentRoot.Q<Button>("heal-button");
        _healButton.clicked += () => StartCoroutine(HealPlayer());

        //Menu button
        _menuButton = hudDocumentRoot.Q<Button>("menu-button");
        _menuButton.clicked += () =>
        {
            hudDocumentRoot.style.display = DisplayStyle.None;
            InitializePauseScreen();
        };
        
        _sidebarTitle = _documentContainer.Q("side-menu-title");
        InitiateSidebarUI();
        
        //Stamina-bar
        _staminaBar = hudDocumentRoot.Q<ProgressBar>("stamina-bar");

        
        //Add this method as a listener to the UnityEvent when the HP changes
        playerManager.GetComponent<HealthSystem>().onHpChangedEvent.AddListener(ChangeHpProgressBar);
        playerManager.GetComponent<PlayerMovement>().onStaminaChangedEvent.AddListener(ChangeStaminaProgressBar);
        }

    private void InitiateSidebarUI()
    {
        var root = _documentContainer.Q("SidemenuContainer");
        
        var invButton = root.Q<Button>("inventory-button");
        var inventoryContent = root.Q("SlotContainer");
        var upgradeButton = root.Q<Button>("upgrade-button");
        var upgradeContent = root.Q("upgrades-content");
        var statsButton = root.Q<Button>("stats-button");
        var statsContent = root.Q("stats-content");
        
        invButton.clicked += () => { ChangeCurrentSidebarContent(invButton, inventoryContent, "Inventory"); };

        upgradeButton.clicked += () => { ChangeCurrentSidebarContent(upgradeButton, upgradeContent, "Upgrades"); };

        statsButton.clicked += () => { ChangeCurrentSidebarContent(statsButton, statsContent, "Stats"); };
    }
    

    private void ChangeCurrentSidebarContent(Button button, VisualElement content, string title)
    {
        _currentSideScreenButton?.RemoveFromClassList("current");
        if (_currentSideScreenContent != null)
        {
            _currentSideScreenContent.style.display = DisplayStyle.None;
            if (_currentSideScreenContent == content)
            {
                _sidebarTitle.style.display = DisplayStyle.None;
                _currentSideScreenContent = null;
                return;
            }
        }
        
        //Set the new content to be the current content and enable Display
        _currentSideScreenContent = content;
        _currentSideScreenContent.style.display = DisplayStyle.Flex;
        _currentSideScreenButton = button;
        SidebarTitle = title;
        _sidebarTitle.style.display = DisplayStyle.Flex;
        _sidebarTitle.Q<Label>().text = SidebarTitle;
        button.AddToClassList("current");
    }

    private void InitializePauseScreen()
    {
        //Settings timescale to 0 to pause the game
        Time.timeScale = 0f;
        
        //Get the pause-menu UXML document
        var root = _documentContainer.Q("PauseMenuDocument");
        root.style.display = DisplayStyle.Flex;

        //Resume button on the pause screen
        var resumeButton = root.Q<Button>("resume-button");
        
        resumeButton.clicked += () =>
        {
            Time.timeScale = 1f;
            
            //Hiding the pause menu
            root.style.display = DisplayStyle.None;
            
            //Displaying the HUD again
            _documentContainer.Q("PlayerHUDDocument").style.display = DisplayStyle.Flex;
        };

        //Exit button on the pause screen
        var exitButton = root.Q<Button>("exit-button");
        
        exitButton.clicked += AppHelper.Quit;
    }

    private IEnumerator HealPlayer()
    {
        if (!_canHeal || !PlayerManager.Heal(10)) yield break;
        _canHeal = false;
        _healButton.text = "3";
        yield return new WaitForSeconds(1f);
        _healButton.text = "2";
        yield return new WaitForSeconds(1f);
        _healButton.text = "1";
        yield return new WaitForSeconds(1f);
        _healButton.text = "Heal";
        _canHeal = true;
    }

    private void ChangeHpProgressBar(float hp)
    {
        _hpBar.value = hp;
        _hpBarLabel.text = "HP: " + _hpBar.value;

        
        //Changing the HP-bar color based on value
        // Calculate the red, green values based on the input value, so the bar will be colored according the value (red is low HP, green is high HP)
        var red = Mathf.Clamp(2.0f * (1 - hp / 100), 0, 1f);
        var green = Mathf.Clamp(2.0f * hp / 100, 0, 1f);
        _hpBar[0][0][0].style.backgroundColor = new Color(red, green, 0);

        //Change the HP-bar Label text color to be readable on all colors in the transition
        var rgb = 1 - hp / 100;
        _hpBarLabel.style.color = new Color(rgb, rgb, rgb, 1f);
    }

    private void ChangeStaminaProgressBar(float stamina)
    {
        _staminaBar.value = stamina;
    }

    private IEnumerator ShowHitScreen(float changedHp, float time)
    {
        if (changedHp < 0) //If player loses HP
            _hitScreen.style.backgroundColor = new Color(1, 0, 0, 0.2f);
        else // If player gets HP
            _hitScreen.style.backgroundColor = new Color(0, 1, 0, 0.2f);
        yield return new WaitForSeconds(time);
        _hitScreen.style.backgroundColor = new Color(0, 0, 0, 0);
    }


    private IEnumerator DisplayMessage(string message, float seconds)
    {
        _messageLabel.text = message;
        _messageLabel.style.display = DisplayStyle.Flex;
        yield return new WaitForSeconds(seconds);
        _messageLabel.style.display = DisplayStyle.None;
    }

    //Public methods for GameManager
    public void DisplayMessageOnScreen(string message, float seconds)
    {
        StartCoroutine(DisplayMessage(message, seconds));
    }

    public void DisplayHitScreen(float changedHp, float time)
    {
        StartCoroutine(ShowHitScreen(changedHp, time));
    }
    
    private VisualElement GetRootDocument()
    {
        return GetComponent<UIDocument>().rootVisualElement;
    }

    public void AddCoin()
    {
        _coinCount += 1;
        
        //This is how you can update text in a label
        _coinAmountLabel.text = _coinCount + "";
    }
}