using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CharacterObject : MonoBehaviour
{
    [SerializeField] string Nombre;
    [SerializeField] Image Image;

    private void Start()
    {
        AudioInputSystem.Instance.DiccionarioTextoToPersonaje(Nombre.ToLower()).Trigger += PlayerDisable;
        AudioInputSystem.Instance.DiccionarioTextoToPersonaje(Nombre.ToLower()).Winner += Win;
        Debug.Log(AudioInputSystem.Instance.DiccionarioTextoToPersonaje(Nombre.ToLower()).name);
    }

    private void PlayerDisable()
    {
        gameObject.SetActive(false);
    }

    private void Win()
    {
        Image.color = Color.green;
        Canvas.ForceUpdateCanvases();
    }
}
