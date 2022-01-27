using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DataText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tmp = null;

    protected Device.Data data = null;

    public Device.Data GetData()
    {
        if (data == null) return null;
        return new Device.Data() { param = data.param, valueFloat = data.valueFloat, valueString = data.valueString };
    }

    public virtual void SetData(Device.Data value, bool space = false)
    {
        data = value;
        if (value == null)
        {
            Debug.LogError("data is null.");
            return;
        }
        if (space)
        {
            tmp.text = "";
        }
        else
        {
            tmp.text = value.param.param == Device.PARAM.AUTHOR || value.param.param == Device.PARAM.CLASS || value.param.param == Device.PARAM.CLASS_DETAIL
            || value.param.param == Device.PARAM.SULFURIZATION || value.param.param == Device.PARAM.ALD
            ? value.valueString : value.valueFloat.ToString();
        }
    }
}
