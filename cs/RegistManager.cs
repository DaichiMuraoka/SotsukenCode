using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RegistManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField stuID = null;
    [SerializeField] private TMP_InputField password = null;
    [SerializeField] private Button registBtn = null;

    public void OnRegistButtonClicked()
    {
        registBtn.interactable = false;
        StartCoroutine(Network.User.Regist(password.text, stuID.text));
    }


    public void OnInputFieldChanged()
    {
        registBtn.interactable = true;
    }
}
