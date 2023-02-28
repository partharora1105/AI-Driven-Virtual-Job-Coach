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
    public TMP_InputField inputChat;
    private string conversation;
    public TextMeshProUGUI convoTextBox;
    public TextMeshProUGUI chatContent;
    public GameObject chatWrapper;
    private string chatStr;
    public string startPromptInterviewer;
    public string applicantType;
    public string[] questions;
    private int questionCount;
    private int startDelay;
    static WebCamTexture backCam;
    public GameObject camScreen;
    public GameObject virtualScreen;
    // Start is called before the first frame update
    void Start()
    {
        prevPrompt = "";
        conversation = "Conversation between a job coach and a " + applicantType + " where the job coach checks in with them and gives them strategies to tackle difficulties at job Job Coach : " + startPromptInterviewer + "\n";
        questionCount = 0;
        convoTextBox.text = "Conversation \n " + conversation;
        if (startDelay < 2) startDelay = 2;
        chatWrapper.SetActive(false);
        Invoke("startConvo", startDelay);
        if (backCam == null) backCam = new WebCamTexture();
        camScreen.GetComponent<Renderer>().material.mainTexture = backCam;
        if (!backCam.isPlaying) backCam.Play();
        virtualScreen.SetActive(false);
        camScreen.SetActive(true);
        

    }
    private void Update()
    {
        prompt = FrostweepGames.Plugins.GoogleCloud.SpeechRecognition.Examples.GCSR_Example.getResponse();
        if (prompt != null && prompt != prevPrompt)
        {
            prevPrompt = prompt;
            Debug.Log("New Prompt = " + prompt);
            chatStr += "You : " + prompt + "\n";
            chatContent.text = chatStr;
            StartAsync();
        }
    }
    void startConvo()
    {
        gameObject.GetComponent<FrostweepGames.Plugins.GoogleCloud.TextToSpeech.GC_TextToSpeech_TutorialExample>().speak(startPromptInterviewer);
        chatStr = "Job Coach : " + startPromptInterviewer + "\n";
        chatContent.text = chatStr;
        virtualScreen.SetActive(true);
        camScreen.SetActive(false);
    }

    async Task StartAsync()
    {
        Debug.Log("Running");
        try
        {
            virtualScreen.SetActive(true);
            camScreen.SetActive(false);
            string p = "";
            if (questionCount < questions.Length) p = " ( " + questions[questionCount] + " ) ";
            conversation += "Applicant (" + applicantType + ") : " + prompt + "\n Job Coach (only replies to applicants and the waits for a response) " + p +" : ";
            questionCount++;
            var api = new OpenAIClient(OpenAIAuthentication.LoadFromEnv());
            var result = await api.CompletionsEndpoint.CreateCompletionAsync(conversation, temperature: 0.3, model: Model.Davinci, maxTokens: 1024);
            conversation += result.ToString().Trim() + "\n";
            chatStr = "Job Coach : " + result.ToString().Trim() + "\n";
            chatContent.text = chatStr;
            Debug.Log(conversation.ToString());
            convoTextBox.text = "Conversation \n " + conversation;
            gameObject.GetComponent<FrostweepGames.Plugins.GoogleCloud.TextToSpeech.GC_TextToSpeech_TutorialExample>().speak(result.ToString());


        } catch (System.Exception)
        {
            throw;
        }


    }

    public void toggleChat()
    {
        chatWrapper.SetActive(!chatWrapper.activeSelf);
    }

    public void enterChat()
    {
        Debug.Log(inputChat.text);
        prompt = inputChat.text;
        inputChat.text = "";
        if (prompt != null && prompt != prevPrompt)
        {
            prevPrompt = prompt;
            Debug.Log("New Prompt = " + prompt);
            chatStr += "You : " + prompt + "\n";
            chatContent.text = chatStr;
            StartAsync();
        }
        

    }

    public void unmuted()
    {
        virtualScreen.SetActive(false);
        camScreen.SetActive(true);
    }

}
