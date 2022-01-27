using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class InputManager : MonoBehaviour
{
    public static string Text { get; private set; } = "";

    private static string message = "";

    private static InputCaller inputCaller = null;

    private static MODE mode = MODE.DEFAULT;

    public static void Open(InputCaller _inputCaller, MODE _mode, string _message, string text = "")
    {
        mode = _mode;
        Text = text;
        message = _message;
        inputCaller = _inputCaller;
        SceneManager.LoadScene("Input", LoadSceneMode.Additive);
    }

    [SerializeField] private TMP_InputField inputField = null;

    [SerializeField] private TextMeshProUGUI messageText = null;

    [SerializeField] private TMP_Dropdown dropdown = null;

    private void Start()
    {
        messageText.text = message;
        InitializeDropdown();
    }

    private void InitializeDropdown()
    {
        dropdown.gameObject.SetActive(mode != MODE.DEFAULT);
        if(mode == MODE.EXTRA)
        {
            List<string> tagList = new List<string>();
            foreach (var device in Network.Cache.devices)
            {
                foreach(var d in device.datas)
                {
                    if (d.param.param == Device.PARAM.EXTRA)
                    {
                        var tag = tagList.Find(t => t == d.param.tag);
                        if (tag == null) tagList.Add(d.param.tag);
                    }
                }
            }
            dropdown.AddOptions(tagList);
        }
        dropdown.value = dropdown.options.FindIndex(o => o.text == Text);
    }

    public void OnDropdownValueChanged()
    {
        inputField.text = dropdown.options[dropdown.value].text;
    }

    public void OnOKButtonClicked()
    {
        if (inputField.text == "") return;
        inputCaller.Input(inputField.text);
        SceneManager.UnloadSceneAsync("Input");
    }

    public void OnCancelButtonClicked()
    {
        SceneManager.UnloadSceneAsync("Input");
    }

    public enum MODE
    {
        DEFAULT,
        EXTRA
    }
}
