
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using SpeechLib;
using System.Xml.Linq;

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
                Debug.Log("Gerando Lista de " + PersonajeTemp.ScriptableObjectPersonaje.Nombre);
                foreach (Noun TemporalNoun in PersonajeTemp.ScriptableObjectPersonaje.PalabrasEtiquetas)
                {
                    Debug.Log("Intento anadir " + TemporalNoun.Palabra);
                    if (!NounIdentify.ContainsKey(TemporalNoun.Palabra.ToLower()))
                    {
                        List<SubjectColor> TemporalList = new List<SubjectColor>();
                        NounIdentify.Add(TemporalNoun.Palabra.ToLower(), TemporalList);
                    }
                    SubjectColor newtempColor2 = new SubjectColor();
                    newtempColor2.Subject = PersonajeTemp.Key.ToLower();
                    newtempColor2.WhatColor = TemporalNoun.Color.adjective;

                    NounIdentify[TemporalNoun.Palabra.ToLower()].Add(newtempColor2);

                    foreach (string Sinonimo in TemporalNoun.Sinonimos)
                    {
                        Debug.Log("Intento anadir " + TemporalNoun.Palabra);

                        if (!NounIdentify.ContainsKey(Sinonimo.ToLower()))
                        {
                            List<SubjectColor> TemporalList = new List<SubjectColor>();
                            NounIdentify.Add(Sinonimo.ToLower(), TemporalList);
                        }

                        NounIdentify[Sinonimo.ToLower()].Add(newtempColor2);
                    }
                }
                Debug.Log("Personaje completado");
            }
        }
        Debug.Log("Listas generadas de palabras");
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
        ColorIdentify.Add("amarillenta", Colors.ColorType.Amarillo);

        ColorIdentify.Add("azul", Colors.ColorType.Azul);
        ColorIdentify.Add("azulado", Colors.ColorType.Azul);
        ColorIdentify.Add("azules", Colors.ColorType.Azul);

        ColorIdentify.Add("rojo", Colors.ColorType.Rojo);
        ColorIdentify.Add("rojos", Colors.ColorType.Rojo);
        ColorIdentify.Add("roja", Colors.ColorType.Rojo);
        ColorIdentify.Add("rojiza", Colors.ColorType.Rojo);

        ColorIdentify.Add("naranja", Colors.ColorType.Naranja);
        ColorIdentify.Add("anaranjado", Colors.ColorType.Naranja);
        ColorIdentify.Add("naranjas", Colors.ColorType.Naranja);

        //Verde
        ColorIdentify.Add("verde", Colors.ColorType.Verde);
        ColorIdentify.Add("verdoso", Colors.ColorType.Verde);
        ColorIdentify.Add("verdes", Colors.ColorType.Verde);
        ColorIdentify.Add("verdosa", Colors.ColorType.Verde);
        ColorIdentify.Add("verdosos", Colors.ColorType.Verde);
        ColorIdentify.Add("verdosas", Colors.ColorType.Verde);
        //Morado
        ColorIdentify.Add("morado", Colors.ColorType.Morado);
        ColorIdentify.Add("morada", Colors.ColorType.Morado);
        ColorIdentify.Add("morados", Colors.ColorType.Morado);
        ColorIdentify.Add("moradas", Colors.ColorType.Morado);

        //Blanco
        ColorIdentify.Add("blanco", Colors.ColorType.Blanco);
        ColorIdentify.Add("blancos", Colors.ColorType.Blanco);
        ColorIdentify.Add("blancas", Colors.ColorType.Blanco);
        ColorIdentify.Add("blanca", Colors.ColorType.Blanco);
        //Negro
        ColorIdentify.Add("negro", Colors.ColorType.Negro);
        ColorIdentify.Add("*****", Colors.ColorType.Negro);
        ColorIdentify.Add("negros", Colors.ColorType.Negro);
        ColorIdentify.Add("******", Colors.ColorType.Negro);
        ColorIdentify.Add("negra", Colors.ColorType.Negro);
        ColorIdentify.Add("negras", Colors.ColorType.Negro);
        ColorIdentify.Add("oscuros", Colors.ColorType.Negro);
        ColorIdentify.Add("oscuro", Colors.ColorType.Negro);
        ColorIdentify.Add("moreno", Colors.ColorType.Negro);
        ColorIdentify.Add("morenos", Colors.ColorType.Negro);
        //Marron
        ColorIdentify.Add("marron", Colors.ColorType.Marron);
        ColorIdentify.Add("marrón", Colors.ColorType.Marron);
        ColorIdentify.Add("castaño", Colors.ColorType.Marron);
        ColorIdentify.Add("marrones", Colors.ColorType.Marron);

        //Gris
        ColorIdentify.Add("gris", Colors.ColorType.Gris);
        ColorIdentify.Add("grises", Colors.ColorType.Gris);
        ColorIdentify.Add("grisaceo", Colors.ColorType.Gris);
        ColorIdentify.Add("grisaceas", Colors.ColorType.Gris);



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


    private void OnDisable()
    {
        m_DictationRecognizer.Stop();
    }

    // Start is called before the first frame update
    void Start()
    {
        ChangeWinner();
        Debug.Log(Apellidos["simpson"]);
        GameManager.Instance.startgame();
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
            m_DictationRecognizer.Start();
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

                Debug.Log(ColorIdentify[temporalColor] + " " + name.WhatColor);

                if (ColorIdentify[temporalColor] == name.WhatColor)
                    NoAfectedCharacters.Remove(name.Subject.ToLower());
            }
        }

        GameManager.Instance.RemoveIntentos(1);

        if (NoAfectedCharacters.Contains(Personajes[WinnerID].Key.ToLower()) && !negative || !NoAfectedCharacters.Contains(Personajes[WinnerID].Key.ToLower()) && negative)
        {
            if (GameManager.Instance.GetIntentos() <= 0)
            {
                voice.Speak("No has acertado, era el " + Personajes[WinnerID].ScriptableObjectPersonaje.Nombre, SpeechVoiceSpeakFlags.SVSFlagsAsync | SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);
                GameManager.Instance.SetStopGame(true);
                StartCoroutine(TriggerWinner());
            }
            else
                voice.Speak("No", SpeechVoiceSpeakFlags.SVSFlagsAsync | SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);

            Debug.Log("No");
            if (apellido)
            {
                if (negative)
                {
                    Personaje[name].Trigger.Invoke();
                }
                else
                {
                    foreach (PersonajeEvent character in Apellidos[temporalnoun])
                    {
                        character.Trigger.Invoke();
                    }
                }
            }
            else
            {
                foreach (SubjectColor name in NounIdentify[temporalnoun])
                {
                    if (temporalColor == null)
                    {
                        if (negative)
                        {
                            foreach (string name2 in NoAfectedCharacters)
                            {
                                Personaje[name2].Trigger.Invoke();
                            }
                        }
                        else
                        {
                            Personaje[name.Subject].Trigger.Invoke();
                        }
                    }
                    else
                    {
                        if (ColorIdentify[temporalColor] == name.WhatColor && !negative || ColorIdentify[temporalColor] != name.WhatColor && negative)
                            Personaje[name.Subject].Trigger.Invoke();

                        if (negative)
                        {
                            foreach (string name2 in NoAfectedCharacters)
                            {
                                Personaje[name2].Trigger.Invoke();
                            }
                        }

                    }
                }
            }
        }
        else
        {
            if (GameManager.Instance.GetIntentos() <= 0)
            {
                voice.Speak("No has acertado, era el " + Personajes[WinnerID].ScriptableObjectPersonaje.Nombre, SpeechVoiceSpeakFlags.SVSFlagsAsync | SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);
                GameManager.Instance.SetStopGame(true);
                StartCoroutine(TriggerWinner());
            }
            else
                voice.Speak("Si", SpeechVoiceSpeakFlags.SVSFlagsAsync | SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);
            Debug.Log("Si");
            if (apellido)
            {
                foreach (string name in NoAfectedCharacters)
                {
                    if (!negative)
                    {
                        Personaje[name].Trigger.Invoke();
                    }
                    else
                    {
                        foreach (PersonajeEvent character in Apellidos[temporalnoun])
                        {
                            character.Trigger.Invoke();
                        }
                    }

                }
            }
            {
                if (temporalColor == null)
                {
                    if (!negative)
                    {
                        foreach (string name in NoAfectedCharacters)
                        {
                            Personaje[name].Trigger.Invoke();
                        }
                    }
                    else
                    {
                        foreach (SubjectColor name in NounIdentify[temporalnoun])
                        {
                            Personaje[name.Subject].Trigger.Invoke();
                        }
                    }
                }
                else
                {
                    foreach (SubjectColor name in NounIdentify[temporalnoun])
                    {
                        if (ColorIdentify[temporalColor] != name.WhatColor && !negative || ColorIdentify[temporalColor] == name.WhatColor && negative)
                            Personaje[name.Subject].Trigger.Invoke();
                    }
                }
            }
        }
    }

    #endregion

    #region Revisa si es la solucion
    bool SolutionChecker(string text)
    {
        if (GameManager.Instance.GetIntentos() <= 0 || GameManager.Instance.GetStopGame())
            return false;

        string[] words = text.Split(" ");
        bool TimeToSolve = false;

        /*foreach (string word in words)
        {
            if (Resolver.Contains(word.ToLower()))
            {
                TimeToSolve = true;
                break;
            }
        }

        if (!TimeToSolve)
            return false;*/

        //Ahora a hacer la comprobacion
        foreach (string word in words)
        {
            foreach (PersonajeEvent PersonajeTemp in Personajes)
            {
                if (PersonajeTemp.ScriptableObjectPersonaje.Nombre.ToLower() == word.ToLower() || PersonajeTemp.ScriptableObjectPersonaje.Pronunciacion.Contains(word.ToLower()) || (PersonajeTemp.ScriptableObjectPersonaje.Apellido.ToLower() == word.ToLower() && word.ToLower() != "simpson"))
                {
                    GameManager.Instance.RemoveIntentos(1);

                    if (Personajes[WinnerID].ScriptableObjectPersonaje.Nombre.ToLower() == PersonajeTemp.ScriptableObjectPersonaje.Nombre.ToLower())
                    {
                        voice.Speak("Correcto", SpeechVoiceSpeakFlags.SVSFlagsAsync | SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);
                        GameManager.Instance.SetStopGame(true);
                        Personajes[WinnerID].Winner.Invoke();
                        Debug.Log("Si es " + Personajes[WinnerID].name);
                    }
                    else
                    {
                        if (GameManager.Instance.GetIntentos() <= 0)
                        {
                            voice.Speak("No has acertado, era " + Personajes[WinnerID].ScriptableObjectPersonaje.Nombre, SpeechVoiceSpeakFlags.SVSFlagsAsync | SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);
                            GameManager.Instance.SetStopGame(true);
                            StartCoroutine(TriggerWinner());
                        }
                        else
                            voice.Speak("No", SpeechVoiceSpeakFlags.SVSFlagsAsync | SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);

                        PersonajeTemp.Trigger.Invoke();
                        Debug.Log("No es " + word.ToLower() + " era " + Personajes[WinnerID].ScriptableObjectPersonaje.Nombre);
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

    private IEnumerator TriggerWinner()
    {
        yield return new WaitForSeconds(1.5f);
        Personajes[WinnerID].Winner.Invoke();
    }

    public void ChangeWinner()
    {
        WinnerID = UnityEngine.Random.Range(0, Personajes.Length);
    }

    #endregion
}
