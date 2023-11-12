using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TablaPersonaje", menuName = "ScriptableObjects/Personaje", order = 1)]
public class Personaje : ScriptableObject
{
    enum Color {Amarillo, Rojo, Azul, Naranja, Verde, Morado, Blanco, Negro}

    public string Nombre;

    public string Apellido;

    public Noun[] PalabrasEtiquetas;

    public List<string> Pronunciacion;
}
