using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Action RestartAction;

    [SerializeField] private TMP_Text _ScoreText;

    private bool _StopGame;

    [SerializeField] private GameObject PauseCanvas;

    private int Intentos = 0;

    // Sera para vida y el texto
    void Awake()
    {
        PauseCanvas.SetActive(true);
        _StopGame = true;
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    #region Gameloop

    public void RestartGame()
    {
        StartCoroutine(RestartRutine());
    }

    public void startgame()
    {
        Intentos = 5;
        UpdateTextIntentos();
        AudioInputSystem.Instance.ChangeWinner();
        PauseCanvas.SetActive(false);
        _StopGame = false;
    }

    IEnumerator RestartRutine()
    {
        yield return new WaitForSeconds(4f);
        startgame();
        RestartAction.Invoke();
    }

    public void SetStopGame(bool value)
    {
        _StopGame = value;
        return;
    }

    public bool GetStopGame()
    {
        return _StopGame;
    }

    #endregion

    #region Intentos Things

    public void AddIntentos(int value)
    {
        Intentos += value;
        UpdateTextIntentos();

        Debug.Log("Te quedan: " + Intentos + " Intentos");

        if(Intentos <= 0)
        {
            Intentos = 0;
            Debug.Log("Perdiste");
        }
    }

    public void RemoveIntentos(int value)
    {
        Intentos -= value;
        UpdateTextIntentos();

        Debug.Log("Te quedan: " + Intentos + " Intentos");

        if (Intentos <= 0)
        {
            Debug.Log("Perdiste");
        }
    }

    public int GetIntentos()
    {
        return Intentos;
    }

    private void UpdateTextIntentos()
    {
        _ScoreText.text ="Intentos " + Intentos;
    } 

    #endregion
}
