
using UnityEngine;

public class OpenAIKey : MonoBehaviour
{
    [Header("Open AI Authentication Key")]
    [SerializeField]  private string key;
    public string getKey()
    {
        return key;
    }
}
