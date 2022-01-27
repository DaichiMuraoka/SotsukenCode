using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataButton : DataText
{
    public void OnClick()
    {
        Filter.Criteria criteria = new Filter.Criteria();
        criteria.param = data.param;
        criteria.value = GetData().valueString;
        Filter.AddCriteria(criteria);
        SceneManager.LoadScene("Extra");
    }
}
