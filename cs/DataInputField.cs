using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DataInputField : DataText
{
    [SerializeField] private TMP_InputField tmpIF = null;

    private bool isString = false;

    public override void SetData(Device.Data value, bool space = false)
    {
        data = value;
        if (value == null)
        {
            Debug.LogError("data is null.");
            return;
        }
        isString = value.param.param == Device.PARAM.AUTHOR || value.param.param == Device.PARAM.CLASS || value.param.param == Device.PARAM.CLASS_DETAIL
            || value.param.param == Device.PARAM.SULFURIZATION || value.param.param == Device.PARAM.ALD;
        tmpIF.text = isString ? value.valueString : value.valueFloat.ToString();
        if (!isString && tmpIF.text == "") tmpIF.text = "0";
        OnEndEdit();
    }

    public void OnEndEdit()
    {
        if (!isString)
        {
            bool changeable = float.TryParse(tmpIF.text, out float res);
            if (changeable) { data.valueFloat = res; }
            else { tmpIF.text = data.valueFloat.ToString(); }
        }
        else data.valueString = tmpIF.text;
    }
}
