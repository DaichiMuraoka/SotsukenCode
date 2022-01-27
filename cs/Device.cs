using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Device
{
    public int id = 0;
    public List<Data> datas = new List<Data>();
    public List<string> jsonDatas = new List<string>();
    public bool lab = false;
    public string url = "";

    public bool IsMatchCriteria(Filter.Criteria criteria)
    {
        var data = datas.Find(d => (d.param.param == criteria.param.param)&&(d.param.tag == criteria.param.tag));
        if(criteria.value == "")
        {
            //数値(範囲)
            return ((criteria.min == null) || (data.valueFloat >= criteria.min)) && ((criteria.max == null) || (data.valueFloat <= criteria.max));
        }
        else
        {
            //文字
            return data.valueString == criteria.value;
        }
    }

    public Data GetData(Param param)
    {
        var data = datas.Find(d => (d.param.param == param.param) && (d.param.tag == param.tag));
        //if (data == null) Debug.LogError("null");
        return data;
    }

    [System.Serializable]
    public class Data
    {
        public Param param = new Param();
        public string valueString = "";
        public float valueFloat = 0f;
        public string paramJson = "";
    }

    [System.Serializable]
    public class Param
    {
        public PARAM param = PARAM.AUTHOR;
        public string tag = "";

        public string GetTitle()
        {
            if (param == PARAM.AUTHOR) return "作成者";
            if (param == PARAM.CLASS) return "分類";
            if (param == PARAM.CLASS_DETAIL) return "形状";
            if (param == PARAM.ALD) return "原子層堆積法";
            if (param == PARAM.SULFURIZATION) return "硫化処理法";
            if (param == PARAM.EFFICIENCY) return "Eff(%)";
            if (param == PARAM.FF) return "FF(%)";
            if (param == PARAM.V_OC) return "Voc(V)";
            if (param == PARAM.J_SC) return "Jsc(mA/cm²)";
            if (param == PARAM.EXTRA) return tag;
            return "?";
        }
    }

    public enum PARAM
    {
        AUTHOR,
        CLASS,
        CLASS_DETAIL,
        ALD,
        SULFURIZATION,
        EFFICIENCY,
        FF,
        V_OC,
        J_SC,
        EXTRA
    }

    

    public void AddData(Param _param, string _valueString = "", float _valueFloat = 0f)
    {
        Data data = new Data() { param = _param, valueString = _valueString, valueFloat = _valueFloat };
        datas.Add(data);
    }

    public string Serialize()
    {
        jsonDatas = new List<string>();
        foreach(var d in datas)
        {
            d.paramJson = JsonUtility.ToJson(d.param);
            jsonDatas.Add(JsonUtility.ToJson(d));
        }
        return JsonUtility.ToJson(this);
    }

    public void Deserialize()
    {
        datas = new List<Data>();
        foreach (var jd in jsonDatas)
        {
            Data d = JsonUtility.FromJson<Data>(jd);
            d.param = JsonUtility.FromJson<Param>(d.paramJson);
            datas.Add(d);
        }
    }
}
