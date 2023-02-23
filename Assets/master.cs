using System.Threading.Tasks;
using System;
using OpenAI;
using OpenAI.Models;
using TMPro;
using UnityEngine;

public class master : MonoBehaviour
{
    private string prompt;
    private string prevPrompt;

    private string conversation;
    public TextMeshProUGUI convoTextBox;
    public string startPromptInterviewer;
    public string applicantType;
    public string[] questions;
    private int questionCount;
    private int startDelay;
    // Start is called before the first frame update
    void Start()
    {
        prevPrompt = "";
        conversation = "Interviewer : " + startPromptInterviewer + "\n";
        questionCount = 0;
        convoTextBox.text = "Conversation \n " + conversation;
        if (startDelay < 2) startDelay = 2;
        Invoke("startConvo", startDelay);
        

    }
    private void Update()
    {
        prompt = FrostweepGames.Plugins.GoogleCloud.SpeechRecognition.Examples.GCSR_Example.getResponse();
        if (prompt != null && prompt != prevPrompt)
        {
            prevPrompt = prompt;
            Debug.Log("New Prompt = " + prompt);
            StartAsync();
        }
    }
    void startConvo()
    {
        gameObject.GetComponent<FrostweepGames.Plugins.GoogleCloud.TextToSpeech.GC_TextToSpeech_TutorialExample>().speak(startPromptInterviewer);
    }

    async Task StartAsync()
    {
        Debug.Log("Running");
        try
        {
            string p = "";
            if (questionCount < questions.Length) p = " ( " + questions[questionCount] + " ) ";
            conversation += "Applicant (" + applicantType + ") : " + prompt + "\n Interviewer " + p +" : ";
            questionCount++;
            var api = new OpenAIClient(OpenAIAuthentication.LoadFromEnv());
            var result = await api.CompletionsEndpoint.CreateCompletionAsync(conversation, temperature: 0.3, model: Model.Davinci, maxTokens: 1024);
            conversation += result.ToString().Trim() + "\n";
            Debug.Log(conversation.ToString());
            convoTextBox.text = "Conversation \n " + conversation;
            gameObject.GetComponent<FrostweepGames.Plugins.GoogleCloud.TextToSpeech.GC_TextToSpeech_TutorialExample>().speak(result.ToString());


        } catch (System.Exception)
        {
            throw;
        }


    }

}
