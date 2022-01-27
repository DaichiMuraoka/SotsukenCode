using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CheckManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText = null;

    private static string message = "";

    private static IEnumerator enumerator = null;

    public static void Open(IEnumerator _enumerator, string _message)
    {
        enumerator = _enumerator;
        message = _message;
        SceneManager.LoadScene("Check", LoadSceneMode.Additive);
    }

    private void Start()
    {
        messageText.text = message;
    }

    public void OnOKButtonClicked()
    {
        StartCoroutine(enumerator);
        //SceneManager.UnloadSceneAsync("Check");
    }

    public void OnCancelButtonClicked()
    {
        SceneManager.UnloadSceneAsync("Check");
    }
}
