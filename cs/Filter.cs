using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Filter
{
    public static bool onlyEffMax = true;

    public static bool onlyLab = false;

    public static List<Criteria> criterias = new List<Criteria>();

    public static void AddCriteria(Criteria _criteria)
    {
        var rem = criterias.Find(c => (c.param.param == _criteria.param.param) && (c.param.tag == _criteria.param.tag));
        if (rem != null) criterias.Remove(rem);
        if ((_criteria.param.param == Device.PARAM.AUTHOR || _criteria.param.param == Device.PARAM.CLASS) && _criteria.value == "") return;
        criterias.Add(_criteria);
    }

    public class Criteria
    {
        public Device.Param param = new Device.Param();
        public string value = "";
        public float? max = null;
        public float? min = null;
    }

    public static List<Device> GetDevice()
    {
        var allDevices = onlyLab ? Network.Cache.devices.FindAll(dv => dv.lab) : Network.Cache.devices;
        var devices = new List<Device>();

        foreach (var d1 in allDevices)
        {
            if (!onlyEffMax)
            {
                devices = allDevices;
                break;
            }

            var c1 = d1.datas.Find(d => d.param.param == Device.PARAM.CLASS);
            var cd1 = d1.datas.Find(d => d.param.param == Device.PARAM.CLASS_DETAIL);
            var a1 = d1.datas.Find(d => d.param.param == Device.PARAM.AUTHOR);
            var e1 = d1.datas.Find(d => d.param.param == Device.PARAM.EFFICIENCY);

            bool skip = false;

            foreach (var d3 in devices)
            {
                var c3 = d3.datas.Find(d => d.param.param == Device.PARAM.CLASS);
                var cd3 = d3.datas.Find(d => d.param.param == Device.PARAM.CLASS_DETAIL);
                var a3 = d3.datas.Find(d => d.param.param == Device.PARAM.AUTHOR);
                if ((c1.valueString == c3.valueString) && (cd1.valueString == cd3.valueString) && (a1.valueString == a3.valueString))
                {
                    skip = true;
                }
            }

            if (skip) { continue; }

            var cache = d1;

            var list = new List<Device>(allDevices);

            foreach (var d2 in list)
            {
                var c2 = d2.datas.Find(d => d.param.param == Device.PARAM.CLASS);
                var cd2 = d2.datas.Find(d => d.param.param == Device.PARAM.CLASS_DETAIL);
                var a2 = d2.datas.Find(d => d.param.param == Device.PARAM.AUTHOR);
                var e2 = d2.datas.Find(d => d.param.param == Device.PARAM.EFFICIENCY);

                if ((c1.valueString == c2.valueString) && (cd1.valueString == cd2.valueString) && (a1.valueString == a2.valueString))
                {
                    var e3 = cache.datas.Find(d => d.param.param == Device.PARAM.EFFICIENCY);
                    if (e2.valueFloat > e3.valueFloat)
                    {
                        cache = d2;
                    }
                }
            }
            devices.Add(cache);
        }

        foreach (var criteria in criterias)
        {
            devices = devices.FindAll(dv => dv.IsMatchCriteria(criteria));
        }
        return devices;
    }

    public static void Clear()
    {
        onlyEffMax = true;
        criterias = new List<Criteria>();
    }
}
