
using UnityEngine;

public class OpenAIKey : MonoBehaviour
{
    [Header("Open AI Authentication Key")]
    [SerializeField]  private string key;
    [SerializeField]  private string orgID;
    public string getKey()
    {
        return key;
    }
    public string getId()
    {
        return orgID;
    }
}
