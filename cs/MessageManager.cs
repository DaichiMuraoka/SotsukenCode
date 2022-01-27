using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MessageManager : MonoBehaviour
{
    private static string message = "";

    private static bool waitButtonClicked = true;

    private static float waitTime = 1f;

    [SerializeField] private GameObject okButton = null;

    [SerializeField] private TextMeshProUGUI messageText = null;

    public static void Open(string _message, bool _waitButtonClicked = true, float _waitTime = 1f)
    {
        message = _message;
        waitButtonClicked = _waitButtonClicked;
        waitTime = _waitTime;
        SceneManager.LoadScene("Message", LoadSceneMode.Additive);
    }

    private void Start()
    {
        okButton.SetActive(waitButtonClicked);
        messageText.text = message;
        if (!waitButtonClicked) StartCoroutine(CloseCountDown());
    }

    private IEnumerator CloseCountDown()
    {
        yield return new WaitForSeconds(waitTime);
        SceneManager.UnloadSceneAsync("Message");
    }

    public void Close()
    {
        SceneManager.UnloadSceneAsync("Message");
    }
}
