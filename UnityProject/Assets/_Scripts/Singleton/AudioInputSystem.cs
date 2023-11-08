
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class SubjectColor
{
    public string Subject { get; set; }
    public Colors.ColorType WhatColor { get; set; }
}

public class AudioInputSystem : MonoBehaviour
{
    private DictationRecognizer m_DictationRecognizer;

    private Dictionary<string, List<SubjectColor>> NounIdentify;

    private Dictionary<string, Colors.ColorType> ColorIdentify;

    private Dictionary<string, PersonajeEvent> Personaje;
    public List<string> ListaPersonajes;
    public PersonajeEvent[] Personajes;


    public List<string> NegativeDefinition;

    private void SetListaNombre()
    {
        foreach (PersonajeEvent Personaje in Personajes)
        {
            ListaPersonajes.Add(Personaje.Key.ToLower());
        }
    }

    private void SetNounsDictionary()
    {
        {
            NounIdentify = new Dictionary<string, List<SubjectColor>>();
            foreach (PersonajeEvent PersonajeTemp in Personajes)
            {
                if (!NounIdentify.ContainsKey(PersonajeTemp.ScriptableObjectPersonaje.Apellido.ToLower()))
                {
                    List<SubjectColor> TemporalList = new List<SubjectColor>();
                    NounIdentify.Add(PersonajeTemp.ScriptableObjectPersonaje.Apellido.ToLower(), TemporalList);
                }

                SubjectColor newtempColor = new SubjectColor();
                newtempColor.Subject = PersonajeTemp.Key.ToLower();
                newtempColor.WhatColor = Colors.ColorType.None;

                NounIdentify[PersonajeTemp.ScriptableObjectPersonaje.Apellido.ToLower()].Add(newtempColor);

                foreach (Noun TemporalNoun in PersonajeTemp.ScriptableObjectPersonaje.PalabrasEtiquetas)
                {
                    if (!NounIdentify.ContainsKey(TemporalNoun.Palabra))
                    {
                        List<SubjectColor> TemporalList = new List<SubjectColor>();
                        NounIdentify.Add(TemporalNoun.Palabra.ToLower(), TemporalList);
                    }
                    SubjectColor newtempColor2 = new SubjectColor();
                    newtempColor2.Subject = PersonajeTemp.Key.ToLower();
                    newtempColor2.WhatColor = TemporalNoun.Color.adjective;

                    NounIdentify[TemporalNoun.Palabra.ToLower()].Add(newtempColor2);
                }
            }
        }
    }

    private void SetEventsDictionary()
    {
        Personaje = new Dictionary<string, PersonajeEvent>();

        foreach (PersonajeEvent PersonajeTemp in Personajes)
        {
            Personaje.Add(PersonajeTemp.Key.ToLower(), PersonajeTemp);
        }
    }

    private void SetColorDictinary()
    {
        ColorIdentify = new Dictionary<string, Colors.ColorType>();

        ColorIdentify.Add("amarillo", Colors.ColorType.Amarillo);
        ColorIdentify.Add("amarillos", Colors.ColorType.Amarillo);
        ColorIdentify.Add("amarilla", Colors.ColorType.Amarillo);
        ColorIdentify.Add("amarillas", Colors.ColorType.Amarillo);
    }

    private void SetNegative()
    {
        NegativeDefinition.Add("no");
    }

    private void Awake()
    {

        SetListaNombre();
        SetNegative();

        SetEventsDictionary();
        SetNounsDictionary();
        SetColorDictinary();

        Debug.Log(PhraseRecognitionSystem.isSupported);
    }



    private void OnEnable()
    {

    }

    void BartFunction()
    {
        Debug.Log("Bart");
    }

    // Start is called before the first frame update
    void Start()
    {

        Debug.Log(NounIdentify["simpson"]);
        Dictation();
    }

    void Dictation()
    {
        m_DictationRecognizer = new DictationRecognizer();

        Debug.Log(m_DictationRecognizer.Status);

        m_DictationRecognizer.DictationResult += (text, confidence) =>
        {
            Debug.LogFormat("Dictation result: {0}", text);
            TextAnalyzer(text);
            //m_Recognitions.text += text + "\n";
        };

        m_DictationRecognizer.DictationHypothesis += (text) =>
        {
            Debug.LogFormat("Dictation hypothesis: {0}", text);
            //m_Hypotheses.text += text;
        };

        m_DictationRecognizer.DictationComplete += (completionCause) =>
        {
            if (completionCause != DictationCompletionCause.Complete)
            {
                m_DictationRecognizer.Start();
                Debug.LogErrorFormat("Dictation completed unsuccessfully: {0}.", completionCause);
            }
        };

        m_DictationRecognizer.DictationError += (error, hresult) =>
        {
            Debug.LogErrorFormat("Dictation error: {0}; HResult = {1}.", error, hresult);
        };

        m_DictationRecognizer.Start();
    }

    #region Analiza el texto

    void TextAnalyzer(string text)
    {
        string[] words = text.Split(" ");
        bool negative = false;
        string temporalnoun = null;
        string temporalColor = null;

        foreach (string word in words)
        {
            //Mira si dice no tambien mantiene la logica de no sobre no es si
            if (NegativeDefinition.Contains(word.ToLower()))
            {
                negative = !negative;
                continue;
            }

            //Mira en la lista de nons a ver si esta
            if (NounIdentify.ContainsKey(word.ToLower()))
            {
                temporalnoun = word.ToLower();
            }

            if (ColorIdentify.ContainsKey(word.ToLower()))
            {
                temporalColor = word.ToLower();
            }

            //Deja de buscar en la frase
            if (temporalnoun != null && temporalColor != null)
                break;

        }

        if (temporalnoun == null)
            return;

        //Si no es no entonces X
        if (!negative)
        {
            foreach (SubjectColor name in NounIdentify[temporalnoun])
            {
                if(temporalColor == null || ColorIdentify[temporalColor] == name.WhatColor)
                    Debug.Log(name);
            }
        }
        //Si es no entonces x
        else
        {
            List<string> CharactersOptions = new List<string>(ListaPersonajes);
            foreach (SubjectColor name in NounIdentify[temporalnoun])
            {
                CharactersOptions.Remove(name.Subject);
            }
            foreach (string name in CharactersOptions)
            {
                if (temporalColor == null || ColorIdentify[temporalColor] == name.WhatColor)
                    Debug.Log(name);
                //Personaje[name].RaiseEvent();
            }

        }
    }

    #endregion

}
