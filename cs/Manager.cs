using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
    [SerializeField] protected DataUI dataUI = null;

    protected List<DataUI> dataUIs = new List<DataUI>();

    [SerializeField] protected Title title = null;

    [SerializeField] protected Transform content = null;

    public List<Device.Param> paramOrder = new List<Device.Param>();

    protected List<Device> devices = new List<Device>();

    protected virtual void SetList() { }

    protected bool renamed = false;

    public void SetParam(Device.Param param, MENU menu)
    {
        Debug.Log(param.param.ToString() + " " + param.tag.ToString());

        if (menu == MENU.ASC) SortDeviceASC(param);
        if (menu == MENU.DESC) SortDeviceDESC(param);
        if (menu == MENU.FIRST_POS) SetParamStartPos(param);

        SetList();
    }

    public void AddParam(Device.Param param)
    {
        paramOrder.Add(param);
        StartCoroutine(DisplayReset());
    }

    public void RemoveParam(Device.Param param)
    {
        if (param.param != Device.PARAM.EXTRA) return;
        var rem = paramOrder.Find(p => p.param == param.param && p.tag == param.tag);
        paramOrder.Remove(rem);
        StartCoroutine(DisplayReset());
    }

    public void RenameParam(Device.Param param, string newTag)
    {
        if (param.param != Device.PARAM.EXTRA) return;
        var ren = paramOrder.Find(p => p.param == param.param && p.tag == param.tag);
        
        foreach (var dui in dataUIs)
        {
            Debug.Log(dui.Device.datas.Count);
            var data = dui.Device.datas.Find(d => d.param.param == param.param && d.param.tag == param.tag);
            data.param.tag = newTag;
        }
        ren.tag = newTag;
        renamed = true;
        StartCoroutine(DisplayReset());
    }

    private IEnumerator DisplayReset()
    {
        title.Set(paramOrder, this);
        yield return null;
        SetList();
    }

    private void SortDeviceASC(Device.Param param)
    {
        if(param.param == Device.PARAM.AUTHOR || param.param == Device.PARAM.CLASS || param.param == Device.PARAM.CLASS_DETAIL
            || param.param == Device.PARAM.ALD || param.param == Device.PARAM.SULFURIZATION)
        {
            devices.Sort((a, b) =>
            {
                var a_data = a.GetData(param);
                var b_data = b.GetData(param);
                return a_data.valueString.CompareTo(b_data.valueString);
            });
            return;
        }
        devices.Sort((a, b) =>
        {
            var a_data = a.GetData(param);
            var b_data = b.GetData(param);
            if ((a_data == null) && (b_data == null)) return 0;
            else if (a_data == null) return 1;
            else if (b_data == null) return -1;
            else
            {
                float value = (float)(a_data.valueFloat - b_data.valueFloat);
                if (value == 0) return 0;
                else return value < 0 ? 1 : -1;
            }
        });
    }

    private void SortDeviceDESC(Device.Param param)
    {
        if (param.param == Device.PARAM.AUTHOR || param.param == Device.PARAM.CLASS || param.param == Device.PARAM.CLASS_DETAIL
            || param.param == Device.PARAM.ALD || param.param == Device.PARAM.SULFURIZATION)
        {
            devices.Sort((a, b) =>
            {
                var a_data = a.GetData(param);
                var b_data = b.GetData(param);
                return -a_data.valueString.CompareTo(b_data.valueString);
            });
            return;
        }
        devices.Sort((a, b) =>
        {
            var a_data = a.GetData(param);
            var b_data = b.GetData(param);
            if ((a_data == null) && (b_data == null)) return 0;
            else if (a_data == null) return 1;
            else if (b_data == null) return -1;
            else
            {
                float value = (float)(a_data.valueFloat - b_data.valueFloat);
                if (value == 0) return 0;
                else return value < 0 ? -1 : 1;
            }
        });
    }

    private void SetParamStartPos(Device.Param _param)
    {
        var param = paramOrder.Find(p => (p.param == _param.param) && (p.tag == _param.tag));
        paramOrder.Remove(param);
        paramOrder.Insert(0, param);

        title.Set(paramOrder, this);
    }

    public void OnClickSearchButton()
    {
        SceneManager.LoadScene("Search");
    }

    public void ClearFilter()
    {
        SceneManager.LoadScene("Device");
    }

    public void RemoveDataUI(DataUI dataUI)
    {
        dataUIs.Remove(dataUI);
        Destroy(dataUI.gameObject);
        SetList();
    }

    public enum MENU
    {
        ASC,
        DESC,
        FIRST_POS
    }
}
