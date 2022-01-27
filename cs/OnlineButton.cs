using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnlineButton : MonoBehaviour
{
    private Button button = null;
    private void Start()
    {
        button = GetComponent<Button>();
    }

    private void FixedUpdate()
    {
        button.interactable = Network.online;
    }
}
