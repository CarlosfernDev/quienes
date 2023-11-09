using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterObject : MonoBehaviour
{
    [SerializeField] string Nombre;

    private void Start()
    {
        AudioInputSystem.Instance.DiccionarioTextoToPersonaje(Nombre.ToLower()).Trigger += PlayerDisable;
        Debug.Log(AudioInputSystem.Instance.DiccionarioTextoToPersonaje(Nombre.ToLower()).name);
    }

    private void PlayerDisable()
    {
        gameObject.SetActive(false);
    }
}
