
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using SpeechLib;

public class SubjectColor
{
    public string Subject { get; set; }
    public Colors.ColorType WhatColor { get; set; }
}

public class AudioInputSystem : MonoBehaviour
{
    public static AudioInputSystem Instance { get; private set; }

    private DictationRecognizer m_DictationRecognizer;

    private SpVoice voice = new SpVoice();

    private Dictionary<string, List<SubjectColor>> NounIdentify;

    private Dictionary<string, Colors.ColorType> ColorIdentify;

    public Dictionary<string, PersonajeEvent> Personaje;
    private Dictionary<string, List<PersonajeEvent>> Apellidos;
    public List<string> ListaPersonajes;
    public PersonajeEvent[] Personajes;

    private int WinnerID;

    public List<string> NegativeDefinition;
    public List<string> ApellidoSinonimoDefinition;
    public List<string> Resolver;

    #region Diccionarios

    private void SetNounsDictionary()
    {
        {
            NounIdentify = new Dictionary<string, List<SubjectColor>>();
            foreach (PersonajeEvent PersonajeTemp in Personajes)
            {
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

    private void SetApellidos()
    {
        Apellidos = new Dictionary<string, List<PersonajeEvent>>();

        foreach (PersonajeEvent PersonajeTemp in Personajes)
        {
            if (!Apellidos.ContainsKey(PersonajeTemp.ScriptableObjectPersonaje.Apellido.ToLower()))
            {
                List<PersonajeEvent> TemporalList = new List<PersonajeEvent>();
                Apellidos.Add(PersonajeTemp.ScriptableObjectPersonaje.Apellido.ToLower(), TemporalList);
            }
            Apellidos[PersonajeTemp.ScriptableObjectPersonaje.Apellido.ToLower()].Add(PersonajeTemp);
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

    #endregion

    #region Ensenyarle palabras

    private void SetListaNombre()
    {
        foreach (PersonajeEvent Personaje in Personajes)
        {
            ListaPersonajes.Add(Personaje.Key.ToLower());
        }
    }

    private void SetColorDictinary()
    {
        ColorIdentify = new Dictionary<string, Colors.ColorType>();

        ColorIdentify.Add("amarillo", Colors.ColorType.Amarillo);
        ColorIdentify.Add("amarillos", Colors.ColorType.Amarillo);
        ColorIdentify.Add("amarilla", Colors.ColorType.Amarillo);
        ColorIdentify.Add("amarillas", Colors.ColorType.Amarillo);

        ColorIdentify.Add("azul", Colors.ColorType.Azul);
        ColorIdentify.Add("azulado", Colors.ColorType.Azul);
    }

    private void SetNegative()
    {
        NegativeDefinition.Add("no");
    }

    private void SetApellidoSinonimo()
    {
        ApellidoSinonimoDefinition.Add("apellido");
        ApellidoSinonimoDefinition.Add("apellida");
    }

    private void SetSolutionKeyWords()
    {
        Resolver.Add("es");
    }
    #endregion

    #region Unity Functions

    private void Awake()
    {
        InstanceSingleton();

        SetListaNombre();
        SetApellidos();
        SetNegative();
        SetSolutionKeyWords();

        SetEventsDictionary();
        SetNounsDictionary();
        SetColorDictinary();
        SetApellidoSinonimo();

        Debug.Log(PhraseRecognitionSystem.isSupported);
    }



    private void OnEnable()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        WinnerID = UnityEngine.Random.Range(0, Personajes.Length);
        Debug.Log(Apellidos["simpson"]);
        Dictation();
    }

    #endregion

    #region Input Audio Things
    void Dictation()
    {
        m_DictationRecognizer = new DictationRecognizer();

        Debug.Log(m_DictationRecognizer.Status);

        m_DictationRecognizer.DictationResult += (text, confidence) =>
        {
            Debug.LogFormat("Dictation result: {0}", text);

            if (!SolutionChecker(text))
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
            }

            Debug.LogErrorFormat("Dictation completed unsuccessfully: {0}.", completionCause);
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
        if (GameManager.Instance.GetIntentos() <= 0)
            return;

        string[] words = text.Split(" ");
        bool negative = false;
        bool apellido = false;
        string temporalnoun = null;
        string temporalColor = null;

        foreach (string word in words)
        {
            if (apellido == true)
            {
                if (Apellidos.ContainsKey(word.ToLower()))
                {
                    temporalnoun = word.ToLower();
                    break;
                }
            }

            //Mira si dice no tambien mantiene la logica de no sobre no es si
            if (NegativeDefinition.Contains(word.ToLower()))
            {
                negative = !negative;
                continue;
            }

            if (ApellidoSinonimoDefinition.Contains(word.ToLower()))
            {
                apellido = true;
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

        List<string> NoAfectedCharacters = new List<string>(ListaPersonajes);
        if (apellido)
        {
            foreach (PersonajeEvent name in Apellidos[temporalnoun])
            {
                NoAfectedCharacters.Remove(name.Key.ToLower());
                continue;
            }
        }
        else
        {

            foreach (SubjectColor name in NounIdentify[temporalnoun])
            {
                if (temporalColor == null)
                {
                    NoAfectedCharacters.Remove(name.Subject.ToLower());
                    continue;
                }

                if (name.WhatColor == Colors.ColorType.None || ColorIdentify[temporalColor] == name.WhatColor)
                    NoAfectedCharacters.Remove(name.Subject.ToLower());
            }
        }

        GameManager.Instance.RemoveIntentos(1);
        if (GameManager.Instance.GetIntentos() <= 0)
        {
            voice.Speak("No has acertado, era el " + Personajes[WinnerID].ScriptableObjectPersonaje.Nombre, SpeechVoiceSpeakFlags.SVSFlagsAsync | SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);
        }

        if (NoAfectedCharacters.Contains(Personajes[WinnerID].Key.ToLower()) && !negative || !NoAfectedCharacters.Contains(Personajes[WinnerID].Key.ToLower()) && negative)
        {
            voice.Speak("No", SpeechVoiceSpeakFlags.SVSFlagsAsync | SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);

            Debug.Log("No");
            foreach(SubjectColor name in NounIdentify[temporalnoun])
            {
                Personaje[name.Subject].Trigger.Invoke();
            }
        }
        else
        {
            voice.Speak("Si", SpeechVoiceSpeakFlags.SVSFlagsAsync | SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);
            Debug.Log("Si");
            foreach(string name in NoAfectedCharacters)
            {
                Personaje[name].Trigger.Invoke();
            }
        }
    }

    #endregion

    #region Revisa si es la solucion
    bool SolutionChecker(string text)
    {
        if (GameManager.Instance.GetIntentos() <= 0)
            return false;

        string[] words = text.Split(" ");
        bool TimeToSolve = false;

        foreach (string word in words)
        {
            if (Resolver.Contains(word.ToLower()))
            {
                TimeToSolve = true;
                break;
            }
        }

        if (!TimeToSolve)
            return false;

        //Ahora a hacer la comprobacion
        foreach (string word in words)
        {
            foreach (PersonajeEvent PersonajeTemp in Personajes)
            {
                if (PersonajeTemp.ScriptableObjectPersonaje.Nombre.ToLower() == word.ToLower())
                {
                    GameManager.Instance.RemoveIntentos(1);
                    if (GameManager.Instance.GetIntentos() <= 0)
                    {
                        voice.Speak("No has acertado, era el " + Personajes[WinnerID].name, SpeechVoiceSpeakFlags.SVSFlagsAsync | SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);
                    }

                    if (Personajes[WinnerID].name.ToLower() == word.ToLower())
                    {
                        voice.Speak("Correcto", SpeechVoiceSpeakFlags.SVSFlagsAsync | SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);
                        Debug.Log("Si es " + Personajes[WinnerID].name);
                    }
                    else
                    {
                        voice.Speak("No", SpeechVoiceSpeakFlags.SVSFlagsAsync | SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);
                        Debug.Log("No es " + word.ToLower() + " era " + Personajes[WinnerID].name);
                    }
                    return true;
                }

            }
        }
        return false;
    }
    #endregion

    #endregion

    #region Audio OutputValues

    private void InstanceSingleton()
    {
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

    public PersonajeEvent DiccionarioTextoToPersonaje(string Value)
    {
        return Personaje[Value];
    } 

    #endregion
}
