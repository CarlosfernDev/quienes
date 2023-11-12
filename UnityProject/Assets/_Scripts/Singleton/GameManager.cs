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

    private int Intentos = 0;

    // Sera para vida y el texto
    void Awake()
    {
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

    private void Start()
    {
        RestartGame();
    }

    #region Gameloop

    public void RestartGame()
    {
        StartCoroutine(RestartRutine());
    }

    IEnumerator RestartRutine()
    {
        yield return new WaitForSeconds(4f);
        Intentos = 5;
        UpdateTextIntentos();
        RestartAction.Invoke();
        AudioInputSystem.Instance.ChangeWinner();
        _StopGame = false;
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
