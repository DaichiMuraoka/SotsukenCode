using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputCaller : MonoBehaviour
{
    protected string text = "";

    public void Input(string _text)
    {
        text = _text;
        OnInputClosed();
    }

    protected virtual void OnInputClosed() { }
}
