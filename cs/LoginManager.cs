using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    private void Start()
    {
        if (SaveDataManager.IsSaveDataExist)
        {
            StartCoroutine(Network.User.Login());
        }
        else
        {
            SceneManager.LoadScene("Regist");
        }
    }
}
