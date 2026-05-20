using System;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject controls;
    [SerializeField] private GameObject settings;
    [SerializeField] private GameObject compendium;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject background;


    private void OnEnable()
    {
        InputManager.Pause += pauseGame;
    }
    private void OnDisable()
    {
        InputManager.Pause -= pauseGame;
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
        controls.SetActive(false);
        settings.SetActive(false);
        compendium.SetActive(false);
        pauseMenu.SetActive(true);
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
        controls.SetActive(false);
        settings.SetActive(false);
        compendium.SetActive(true);
        pauseMenu.SetActive(false);
    }
    public void Settings()
    {
        controls.SetActive(false);
        settings.SetActive(true);
        compendium.SetActive(false);
        pauseMenu.SetActive(false);
    }
    public void pauseGame() 
    {
        
        controls.SetActive(false);
        settings.SetActive(false);
        compendium.SetActive(false);
        pauseMenu.SetActive(true);
        background.SetActive(true);
    }
}
