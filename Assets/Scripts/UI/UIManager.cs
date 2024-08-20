using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager: MonoBehaviour
{
    public static UIManager Instance;

    public void OnEnable()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public enum UIState
    {
        MainMenu = 0,
        Game = 1,
        Pause = 2,
        HowToPlay = 3,
        StageSelect = 4,
        ConfirmQuit = 5,
        StageComplete = 6,
    }
    
    public List<UIState> StateStack = new List<UIState>() {UIState.MainMenu};
    
    public GameObject MainMenu;
    public GameObject Reticle;
    public GameObject HowToPlay;
    public GameObject StageSelect;
    public GameObject Pause;
    public GameObject ConfirmQuit;
    
    public GameObject StageComplete;
    public GameObject RetryButton;
    public GameObject NextStageButton;
    public TextMeshProUGUI StageCompleteText;
    
    public void Start()
    {
        MainMenu.SetActive(false);
        Reticle.SetActive(false);
        HowToPlay.SetActive(false);
        StageSelect.SetActive(false);
        ConfirmQuit.SetActive(false);
        Pause.SetActive(false);
        StageComplete.SetActive(false);
        
        PushState(UIState.MainMenu);
    }

    public void PushState(UIState state)
    {
        if (StateStack.Count > 0)
        {
            if (StateStack[0] == state)
            {
                return;
            }
            
            OnExitState(StateStack[0]);
        }
        
        PopState();
        StateStack.Add(state);
        OnEnterState(state);
    }
    
    public void PushMainMenu()
    {
        PushState(UIState.MainMenu);
    }
    
    public void PushHowToPlay()
    {
        PushState(UIState.HowToPlay);
    }
    
    public void PushStageSelect()
    {
        PushState(UIState.StageSelect);
    }
    
    public void PushConfirmQuit()
    {
        PushState(UIState.ConfirmQuit);
    }
    
    public void Resume()
    {
        TryTogglePause();
    }

    public void TryTogglePause()
    {
        switch (StateStack[0])
        {
            case UIState.Game:
                PushState(UIState.Pause);
                break;
            case UIState.Pause:
                PushState(UIState.Game);
                break;
            case UIState.MainMenu:
                PushState(UIState.ConfirmQuit);
                break;
            case UIState.ConfirmQuit:
                PushState(UIState.MainMenu);
                break;
            case UIState.StageSelect:
                PushState(UIState.MainMenu);
                break;
            case UIState.HowToPlay:
                TryBackFromHelp();
                break;
        }
    }

    private bool gameRunning;
    
    public void TryToggleHelp()
    {
        if (StateStack[0] == UIState.Game)
        {
            PushState(UIState.HowToPlay);
        }
        else if (StateStack[0] == UIState.HowToPlay)
        {
            if (!gameRunning)
            {
                return;
            }
            
            PushState(UIState.Game);
        }
    }

    public void TryBackFromHelp()
    {
        if (gameRunning)
        {
            PushState(UIState.Game);
        }
        else
        {
            PushState(UIState.MainMenu);
        }
    }
    
    public void PopState()
    {
        bool backToMain = false;
        
        if (StateStack.Count > 0)
        { 
            OnExitState(StateStack[0]);
            StateStack.RemoveAt(0);
        }
    }

    private void OnEnterState(UIState state)
    {
        switch (state)
        {
            case UIState.MainMenu:
                MainMenu.SetActive(true);
                break;
            case UIState.Game:
                AudioListener.pause = false;
                Time.timeScale = 1f;
                Cursor.lockState = CursorLockMode.Locked;
                Reticle.SetActive(true);
                break;
            case UIState.ConfirmQuit:
                ConfirmQuit.SetActive(true);
                break;
            case UIState.StageSelect:
                StageSelect.SetActive(true);
                break;
            case UIState.HowToPlay:
                Cursor.lockState = CursorLockMode.None;
                HowToPlay.SetActive(true);
                break;            
            case UIState.Pause:
                Cursor.lockState = CursorLockMode.None;
                Pause.SetActive(true);
                break;
            case UIState.StageComplete:
                Cursor.lockState = CursorLockMode.None;
                StageComplete.SetActive(true);
                break;
        }
    }
    
    private void OnExitState(UIState state)
    {
        switch (state)
        {
            case UIState.MainMenu:
                MainMenu.SetActive(false);
                break;
            case UIState.Game:
                Reticle.SetActive(false);
                AudioListener.pause = true;
                Time.timeScale = 0f;
                break;            
            case UIState.ConfirmQuit:
                ConfirmQuit.SetActive(false);
                break;
            case UIState.StageSelect:
                StageSelect.SetActive(false);
                break;
            case UIState.HowToPlay:
                HowToPlay.SetActive(false);
                break;            
            case UIState.Pause:
                Pause.SetActive(false);
                break;
            case UIState.StageComplete:
                StageComplete.SetActive(false);
                break;
        }
    }

    public void StartStage(int stage)
    {
        gameRunning = true;
        
        Time.timeScale = 1f;
        Scoreboard.Instance.StartStage((StagingManager.StageEnum)stage);
        PushState(UIState.Game);
    }
    
    public void RestartStage()
    {
        Time.timeScale = 1f;
        Scoreboard.RestartCurrentStage();
        PushState(UIState.Game);
    }

    public void Quit()
    {
        Application.Quit();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TryTogglePause();
            return;
        }
        
        if (Input.GetKeyDown(KeyCode.F1))
        {
            TryToggleHelp();
            return;
        }
    }

    public void PushStageComplete(bool success, string message)
    {
        if (Scoreboard.Instance.CurrentStage != StagingManager.StageEnum.Level3)
        {
            NextStageButton.SetActive(false);
            RetryButton.SetActive(false);
        }
        if (success)
        {
            NextStageButton.SetActive(true);
            RetryButton.SetActive(false);
        }
        else
        {
            NextStageButton.SetActive(false);
            RetryButton.SetActive(true);
        }
        
        StageCompleteText.text = message;
        PushState(UIState.StageComplete);
    }
    
    public void NextStage()
    {
        switch (Scoreboard.Instance.CurrentStage)
        {
            case StagingManager.StageEnum.Level1:
                StartStage(1);
                break;
            case StagingManager.StageEnum.Level2:
                StartStage(2);
                break;
            case StagingManager.StageEnum.Level3:
                break;
        }
    }
}