using System;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject controls;
    [SerializeField] private GameObject settings;
    [SerializeField] private GameObject compendium;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject background;
    [SerializeField] GameObject fullMenu,lobbyScreen;

    bool open = false;
    private void OnEnable()
    {
        InputManager.Pause += PauseGame;
    }
    private void OnDisable()
    {
        InputManager.Pause -= PauseGame;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void quitGame() 
    {
        Application.Quit();
    }
    public void Back() 
    {
        if(controls)
        controls.SetActive(false);
        settings.SetActive(false);
        compendium.SetActive(false);
        pauseMenu.SetActive(true);
        if (lobbyScreen)
            lobbyScreen.SetActive(false);
        if (fullMenu)
            fullMenu.SetActive(true);
    }
    public void BackToGame() 
    {
        Back();
        pauseMenu.SetActive(false);
        background.SetActive(false);
    }
    public void Controls() 
    {
        controls.SetActive(true);
        settings.SetActive(false);
        compendium.SetActive(false);
        pauseMenu.SetActive(false);
    }
    public void Compendium()
    {
        if (controls)
        controls.SetActive(false);
        settings.SetActive(false);
        compendium.SetActive(true);
        pauseMenu.SetActive(false);
    }
    public void Settings()
    {
        if(controls)
        controls.SetActive(false);
        settings.SetActive(true);
        compendium.SetActive(false);
        pauseMenu.SetActive(false);
    }
    public void PauseGame() 
    {
        if (!open)
        {
            controls.SetActive(false);
            settings.SetActive(false);
            compendium.SetActive(false);
            pauseMenu.SetActive(true);
            background.SetActive(true);
            open=true;
        }
        else
        {
            open = false;
            BackToGame();
        }
    }
    public void ShowLobby()
    {
        fullMenu.SetActive(false);
        lobbyScreen.SetActive(true);
    }
}
