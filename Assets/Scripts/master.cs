using System.Threading.Tasks;
using System;
using OpenAI;
using OpenAI.Models;
using TMPro;
using UnityEngine;

public class master : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string startPromptInterviewer;
    [SerializeField] private string applicantType;
    [SerializeField] private string[] questions;
    [SerializeField] private bool toggleCamera;
    [Header("References")]
    [SerializeField] private GameObject camScreen;
    [SerializeField] private GameObject virtualScreen;
    [SerializeField] private GameObject openAi;
    [SerializeField] private TextMeshProUGUI convoTextBox;
    [SerializeField] private TextMeshProUGUI chatContent;
    [SerializeField] private GameObject chatWrapper;
    [SerializeField] private TMP_InputField inputChat;
    private string prompt;
    private string prevPrompt;
    private string conversation;
    private string chatStr;
    private int questionCount;
    private int startDelay;
    static WebCamTexture backCam;
    
    
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
        if (toggleCamera)
        {
            virtualScreen.SetActive(false);
            camScreen.SetActive(true);
        }
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
        if (toggleCamera)
        {
            virtualScreen.SetActive(true);
            camScreen.SetActive(false);
        }
    }

    async Task StartAsync()
    {
        Debug.Log("Running");
        try
        {
            if (toggleCamera)
            {
                virtualScreen.SetActive(true);
                camScreen.SetActive(false);
            }
            string p = "";
            if (questionCount < questions.Length) p = " ( " + questions[questionCount] + " ) ";
            conversation += "Applicant (" + applicantType + ") : " + prompt + "\n Job Coach (only replies to applicants and the waits for a response) " + p +" : ";
            questionCount++;
            //var api = new OpenAIClient(OpenAIAuthentication.LoadFromEnv());
            
            var api = new OpenAIClient(new OpenAIAuthentication(openAi.GetComponent<OpenAIKey>().getKey(), openAi.GetComponent<OpenAIKey>().getId()));
            //var api = new OpenAIClient(openAi.GetComponent<OpenAIKey>().getKey());
            //var api = new OpenAIClient("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");
            var output = await api.CompletionsEndpoint.CreateCompletionAsync(conversation, temperature: 0.3, model: Model.Davinci, maxTokens: 1024);
            Debug.Log(output);
            string result = output.ToString();
            var indexCheck = result.IndexOf("Applicant");
            if (indexCheck != -1) result = result.Substring(0, indexCheck);
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
            chatStr += "\nYou : " + prompt + "\n";
            chatContent.text = chatStr;
            StartAsync();
        }
        

    }

    public void unmuted()
    {
        if (toggleCamera)
        {
            virtualScreen.SetActive(false);
            camScreen.SetActive(true);
        }
    }

}
