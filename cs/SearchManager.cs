using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SearchManager : Manager
{
    [SerializeField] protected SearchUI searchUI = null;

    protected List<SearchUI> searchUIs = new List<SearchUI>();

    public bool debug = true;

    private void Start()
    {
        StartCoroutine(Initialize());
    }

    private IEnumerator Initialize()
    {
        if(debug && !Network.Cache.isHavingData) yield return StartCoroutine(Network.Get.AllData());

        paramOrder.Add(new Device.Param() { param = Device.PARAM.CLASS });
        paramOrder.Add(new Device.Param() { param = Device.PARAM.EFFICIENCY });
        paramOrder.Add(new Device.Param() { param = Device.PARAM.FF });
        paramOrder.Add(new Device.Param() { param = Device.PARAM.V_OC });
        paramOrder.Add(new Device.Param() { param = Device.PARAM.J_SC });
        paramOrder.Add(new Device.Param() { param = Device.PARAM.AUTHOR });

        title.Set(paramOrder, this);

        SetList();
    }

    protected override void SetList()
    {
        while (searchUIs.Count < paramOrder.Count)
        {
            SearchUI sui = Instantiate(searchUI, content);
            searchUIs.Add(sui);
        }
        while (searchUIs.Count > paramOrder.Count)
        {
            SearchUI sui = searchUIs[0];
            searchUIs.Remove(sui);
            Destroy(sui.gameObject);
        }

        for (int i = 0; i < paramOrder.Count; i++)
        {
            searchUIs[i].SetParam(paramOrder[i]);
        }
    }

    public void Search()
    {
        foreach(var sui in searchUIs)
        {
            if (sui.Criteria == null) continue;
            Filter.AddCriteria(sui.Criteria);
        }
        SceneManager.LoadScene("Extra");
    }
}
