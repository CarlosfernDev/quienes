
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class AudioInputSystem : MonoBehaviour
{
    private DictationRecognizer m_DictationRecognizer;

    private Dictionary<string, string[]> Identificador;
    private Dictionary<string, System.Action> Personaje;

    private List<string> NegativeDefinition;

    private string[] NombrePersonajes = new string[] {"Homer", "Bart", "Marge", "Lisa", "Maggie", "Abraham", 
        "Apu", "Skinner", "Wiggum", "Krusty", "Milhouse", "Edna", "Fat", "Quimby", "Lenny", "Barney",
        "Carl", "Moe", "Otto", "Kent", "Lovejoy", "Comic", "Willies"};

     public System.Action Homer ,Bart , Marge,
     Lisa, Maggie, Abraham, Apu, Skinner,
     Wiggum, Krusty, Milhouse, Edna, Fat, 
     Quimby, Lenny, Barney, Carl, Moe,
     Otto, Kent, Lovejoy, Comic, Willies;

    private void SetNounsDictionary()
    {
        {
            Identificador = new Dictionary<string, string[]>();
            Identificador.Add("rojo", new string[] { "Bart", "Homer"});
        }
    }

    private void SetEventsDictionary()
    {
        Personaje = new Dictionary<string, System.Action>();
        Personaje.Add("Homer", Homer);
        Personaje.Add("Bart", Bart);
        Personaje.Add("Marge", Marge);
        Personaje.Add("Lisa", Lisa);
        Personaje.Add("Maggie", Maggie);
        Personaje.Add("Abraham", Abraham);
        Personaje.Add("Apu", Apu);
        Personaje.Add("Skinner", Skinner);
        Personaje.Add("Wiggum", Wiggum);
        Personaje.Add("Krusty", Milhouse);
        Personaje.Add("Edna", Edna);
        Personaje.Add("Fat", Fat);
        Personaje.Add("Quimby", Quimby);
        Personaje.Add("Lenny", Lenny);
        Personaje.Add("Barney", Barney);
        Personaje.Add("Carl", Carl);
        Personaje.Add("Moe", Moe);
        Personaje.Add("Otto", Otto);
        Personaje.Add("Kent", Kent);
        Personaje.Add("Lovejoy", Lovejoy);
        Personaje.Add("Comic", Comic);
        Personaje.Add("Willies", Willies);
    }

    private void SetNegative()
    {
        NegativeDefinition.Add("no");
    }

    private void Awake() {
        Bart += BartFunction;
        Homer += BartFunction;
        Lisa += BartFunction;

        SetNounsDictionary();
        SetEventsDictionary();
        Debug.Log(PhraseRecognitionSystem.isSupported);
        Personaje["Bart"]();
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

    void TextAnalyzer(string text)
    {
        string[] words = text.Split(" ");
        bool negative = false;
        foreach (string word in words)
        {
            if (NegativeDefinition.Contains(word.ToLower()))
            {
                negative = !negative;
                continue;
            }

            string[] value;
            if (Identificador.TryGetValue(word.ToLower(), out value))
            {
                if (!negative)
                {
                    foreach (string name in value)
                    {
                        Personaje[name]();
                    }
                }
                else
                {
                    foreach (string NombreOriginal in NombrePersonajes)
                    {
                        bool Icall = false;
                        foreach (string name in value)
                        {
                            if (name == NombreOriginal)
                            {
                                Icall = true;
                                break;
                            }
                        }
                        if (Icall)
                            Personaje[NombreOriginal]();
                    }
                }
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
