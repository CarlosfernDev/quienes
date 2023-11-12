using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TablaPersonaje", menuName = "ScriptableObjects/PersonajeEvent", order = 1)]
public class PersonajeEvent : ScriptableObject
{
    public string Key;
    public Action Trigger;
    public Action Winner;
    public Personaje ScriptableObjectPersonaje;

    public void RaiseEvent()
    {
        Trigger?.Invoke();
    }
}
