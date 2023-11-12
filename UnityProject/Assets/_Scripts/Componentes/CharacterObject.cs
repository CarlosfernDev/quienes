using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CharacterObject : MonoBehaviour
{
    [SerializeField] string Nombre;
    [SerializeField] Image Image;
    Transform[] childs;

    private void Start()
    {
        childs = gameObject.GetComponentsInChildren<Transform>();
        AudioInputSystem.Instance.DiccionarioTextoToPersonaje(Nombre.ToLower()).Trigger += PlayerDisable;
        AudioInputSystem.Instance.DiccionarioTextoToPersonaje(Nombre.ToLower()).Winner += Win;
        Debug.Log(AudioInputSystem.Instance.DiccionarioTextoToPersonaje(Nombre.ToLower()).name);
        GameManager.Instance.RestartAction += Enable;
    }

    private void PlayerDisable()
    {
        StartCoroutine(ChangeColor());
    }

    private void Win()
    {
        Image.color = Color.green;
        GameManager.Instance.RestartGame();
    }

    IEnumerator ChangeColor()
    {
        Image.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        Image.color = Color.white;
        yield return new WaitForSeconds(0.25f);
        Image.color = Color.red;
        yield return new WaitForSeconds(0.25f);
        Image.color = Color.white;
        yield return new WaitForSeconds(0.3f);
        Image.color = Color.red;
        yield return new WaitForSeconds(0.4f);
        Disable();
        Image.color = Color.white;
    }

    private void Disable()
    {
        foreach(Transform child in childs)
        {
            child.gameObject.SetActive(false);
        }
    }

    private void Enable()
    {
        Debug.Log(Nombre);
        Image.color = Color.white;
        foreach (Transform child in childs)
        {
            child.gameObject.SetActive(true);
        }
    }
}
