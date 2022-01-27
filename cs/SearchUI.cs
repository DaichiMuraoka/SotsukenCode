using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SearchUI : MonoBehaviour
{
    private Filter.Criteria criteria = null;

    public Filter.Criteria Criteria { get { return criteria; } }

    [SerializeField] private TMP_Dropdown dropdown = null;

    [SerializeField] private RangeUI rangeUI = null;

    private List<Device.Data> datas = new List<Device.Data>();

    public void SetParam(Device.Param _param)
    {
        criteria = new Filter.Criteria() { param = _param };
        if (_param.param == Device.PARAM.AUTHOR || _param.param == Device.PARAM.CLASS) SetDropdown();
        else SetRangeUI();
    }

    private void SetDropdown()
    {
        dropdown.gameObject.SetActive(true);
        rangeUI.gameObject.SetActive(false);
        datas = new List<Device.Data>();

        dropdown.options.Add(new TMP_Dropdown.OptionData { text = "設定なし" });

        foreach (var device in Network.Cache.devices)
        {
            var d1 = device.datas.Find(d => (d.param.param == criteria.param.param) && (d.param.tag == criteria.param.tag));
            bool addList = true;
            foreach(var d2 in datas)
            {
                if (d2.valueString == d1.valueString) addList = false;
            }
            if (addList) datas.Add(d1);
        }
        foreach (var data in datas)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData { text = data.valueString });
        }
        var fcri = Filter.criterias.Find(c => (c.param.param == criteria.param.param) && (c.param.tag == criteria.param.tag));
        if (fcri != null)
        {
            criteria.value = fcri.value;
            dropdown.value = datas.FindIndex(d => d.valueString == fcri.value) + 1;
        }
    }

    public void OnDropdownValueChanged()
    {
        if(dropdown.value == 0)
        {
            criteria.value = "";
        }
        else
        {
            criteria.value = datas[dropdown.value - 1].valueString;
        }
    }

    private void SetRangeUI()
    {
        dropdown.gameObject.SetActive(false);
        rangeUI.gameObject.SetActive(true);

        var fcri = Filter.criterias.Find(c => (c.param.param == criteria.param.param) && (c.param.tag == criteria.param.tag));
        if (fcri != null)
        {
            rangeUI.Max = fcri.max;
            rangeUI.Min = fcri.min;
        }
    }

    public void OnRangeUIValueChanged()
    {
        criteria.max = rangeUI.Max;
        criteria.min = rangeUI.Min;
        Debug.Log(criteria.param.param.ToString() + " : max=" + rangeUI.Max + " min=" + rangeUI.Min);
    }
}
