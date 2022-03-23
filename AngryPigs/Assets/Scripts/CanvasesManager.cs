using System;
using UnityEngine;

public class CanvasesManager : MonoBehaviour
{
    public static CanvasesManager Current;
    [SerializeField] private GameObject inGameUi;
    [SerializeField] private GameObject looseCanvas;
    [SerializeField] private GameObject startCanvas;

    public void InitializeGame()
    {
        GraphController.Current.GenerateNewObjects();
        AgentManager.Current.CreateAgents();
        BombManager.Current.ClearBombs();
    }
    public void OpenLooseMenu()
    {
        looseCanvas.SetActive(true);
        CloseInGameUi();
    }

    public void CloseLooseMenu()
    {
        looseCanvas.SetActive(false);
        InitializeGame();
        OpenInGameUi();
    }

    public void CloseStartMenu()
    {
        startCanvas.SetActive(false);
        InitializeGame();
        OpenInGameUi();
    }

    public void CloseInGameUi()
    {
        inGameUi.SetActive(false);
    }
    private void OpenInGameUi()
    {
        inGameUi.SetActive(true);
    }
    private void Awake()
    {
        Current = this;
    }
}