using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RangeUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField maxIF = null;
    [SerializeField] private TMP_InputField minIF = null;

    public float? Max
    {
        get
        {
            float f;
            if (float.TryParse(maxIF.text, out f))
            {
                return f;
            }
            else
            {
                return null;
            }
        }
        set
        {
            maxIF.text = value.ToString();
        }
    }

    public float? Min
    {
        get
        {
            float f;
            if (float.TryParse(minIF.text, out f))
            {
                return f;
            }
            else
            {
                return null;
            }
        }
        set
        {
            minIF.text = value.ToString();
        }
    }
}
