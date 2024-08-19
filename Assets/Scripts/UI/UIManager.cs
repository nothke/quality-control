using System.Collections.Generic;
using UnityEngine;

public class UIManager: MonoBehaviour
{
    public enum UIState
    {
        MainMenu = 0,
        Game = 1,
        Pause = 2,
        HowToPlay = 3,
        StageSelect = 4,
        ConfirmQuit = 5,
    }
    
    public List<UIState> StateStack = new List<UIState>() {UIState.MainMenu};
    
    public GameObject MainMenu;
    public GameObject Reticle;
    public GameObject HowToPlay;
    public GameObject StageSelect;
    public GameObject ConfirmQuit;

    public void Start()
    {
        MainMenu.SetActive(false);
        Reticle.SetActive(false);
        HowToPlay.SetActive(false);
        StageSelect.SetActive(false);
        ConfirmQuit.SetActive(false);
        
        PushState(UIState.MainMenu);
    }

    public void PushState(UIState state)
    {
        if (StateStack.Count > 0)
        {
            OnExitState(StateStack[0]);
        }
        
        StateStack.Add(state);
        OnEnterState(state);
    }
    
    public void PopState()
    {
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
                Reticle.SetActive(true);
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
                break;
        }
    }

    public void StartStage(int stage)
    {
        Scoreboard.Instance.StartStage((StagingManager.StageEnum)stage);
        PushState(UIState.Game);
    }

    public void Quit()
    {
        Application.Quit();
    }
}