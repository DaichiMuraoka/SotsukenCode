using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using System.Security.Cryptography;

public class Network
{
    private const string ACCESS_URL = "https://nianagoblog.com/sotsuken/server_access.php";

    public static bool online = false;

    //POSTリクエスト
    private static UnityWebRequest PostRequest(string reqest, string data = "")
    {
        WWWForm form = new WWWForm();   //Post用フォームの作成
        //Post情報追加
        form.AddField("req", reqest);   //リクエスト
        form.AddField("data", data);   //データ
        //POST
        UnityWebRequest uwr = UnityWebRequest.Post(ACCESS_URL, form);
        return uwr;
    }

    //データ取得
    public class Get
    {
        //device、extra取得
        public static IEnumerator AllData()
        {
            yield return Devices();
            if (online)
            {
                yield return Extra();
                if (online) SaveDataManager.Save();
            }
            Cache.isHavingData = true;
        }

        //device取得
        public static IEnumerator Devices()
        {
            UnityWebRequest uwr = PostRequest("get_device");
            yield return uwr.SendWebRequest();    //応答待機
            if (uwr.isHttpError || uwr.isNetworkError)
            {
                online = false;
                Debug.Log(uwr.isHttpError ? "httpError:" : "networkError:" + uwr.error);        //エラー
            }
            else
            {
                online = true;
                string s = uwr.downloadHandler.text;    //id-aut-cla-ald-sul-eff-ff-voc-jsc:...
                Debug.Log(s);
                //リスト初期化
                Cache.devices = new List<Device>();
                //変換
                string[] arr = s.Split('|');
                foreach (string row in arr)
                {
                    string[] arr3 = row.Split('^');
                    string[] arr2 = arr3[0].Split('=');
                    Device device = new Device();
                    device.id = int.Parse(arr2[0]);
                    device.AddData(new Device.Param() { param = Device.PARAM.AUTHOR }, _valueString: arr2[1]);
                    device.AddData(new Device.Param() { param = Device.PARAM.CLASS }, _valueString: arr2[2]);
                    device.AddData(new Device.Param() { param = Device.PARAM.CLASS_DETAIL }, _valueString: arr2[3]);
                    device.AddData(new Device.Param() { param = Device.PARAM.ALD }, _valueString: arr2[4]);
                    device.AddData(new Device.Param() { param = Device.PARAM.SULFURIZATION }, _valueString: arr2[5]);
                    device.AddData(new Device.Param() { param = Device.PARAM.EFFICIENCY }, _valueFloat: float.Parse(arr2[6]));
                    device.AddData(new Device.Param() { param = Device.PARAM.FF }, _valueFloat: float.Parse(arr2[7]));
                    device.AddData(new Device.Param() { param = Device.PARAM.V_OC }, _valueFloat: float.Parse(arr2[8]));
                    device.AddData(new Device.Param() { param = Device.PARAM.J_SC }, _valueFloat: float.Parse(arr2[9]));
                    device.lab = float.Parse(arr2[10]) != 0;
                    device.url = arr3[1];
                    Cache.devices.Add(device);
                }
                Debug.Log("getting device is complete.");
            }
        }
        //extra取得
        public static IEnumerator Extra()
        {
            UnityWebRequest uwr = PostRequest("get_extra");
            yield return uwr.SendWebRequest();    //応答待機
            if (uwr.isHttpError || uwr.isNetworkError)
            {
                online = false;
                Debug.Log(uwr.isHttpError ? "httpError:" : "networkError:" + uwr.error);        //エラー
            }
            else
            {
                online = true;
                string s = uwr.downloadHandler.text;    //device_id-tag_value:...
                Debug.Log(s);
                //変換
                string[] arr = s.Split('|');
                foreach (string row in arr)
                {
                    string[] arr2 = row.Split('=');
                    int device_id = int.Parse(arr2[0]);
                    //deviceに追加
                    var device = Cache.devices.Find(d => d.id == device_id);
                    if (device == null)
                    {
                        Debug.LogError("device id:" + device_id.ToString() + " is null.");
                        continue;
                    }
                    device.AddData(new Device.Param() { param = global::Device.PARAM.EXTRA, tag = arr2[1] }, _valueFloat: float.Parse(arr2[2]));
                }
            }
            Cache.devices.Sort((a, b) => a.id - b.id);
            Debug.Log("getting extra is complete.");
        }
    }

    //データ編集
    public class Edit
    {
        //更新
        public static IEnumerator Update(Device device)
        {
            string data = device.id.ToString() + "|"
                            + device.GetData(new Device.Param() { param = Device.PARAM.AUTHOR }).valueString + "="
                            + device.GetData(new Device.Param() { param = Device.PARAM.CLASS }).valueString + "="
                            + device.GetData(new Device.Param() { param = Device.PARAM.CLASS_DETAIL }).valueString + "="
                            + device.GetData(new Device.Param() { param = Device.PARAM.ALD }).valueString + "="
                            + device.GetData(new Device.Param() { param = Device.PARAM.SULFURIZATION }).valueString + "="
                            + device.GetData(new Device.Param() { param = Device.PARAM.EFFICIENCY }).valueFloat + "="
                            + device.GetData(new Device.Param() { param = Device.PARAM.FF }).valueFloat + "="
                            + device.GetData(new Device.Param() { param = Device.PARAM.V_OC }).valueFloat + "="
                            + device.GetData(new Device.Param() { param = Device.PARAM.J_SC }).valueFloat + "="
                            + (device.lab ? "1" : "0")
                            +  "|" + device.url;
            var extras = device.datas.FindAll(d => d.param.param == Device.PARAM.EXTRA);
            foreach(var ex in extras)
            {
                data += "|" + ex.param.tag + "=" + ex.valueFloat;
            }
            Debug.Log(data);
            UnityWebRequest uwr = PostRequest("update", data);
            yield return uwr.SendWebRequest();    //応答待機
            if (uwr.isHttpError || uwr.isNetworkError)
            {
                online = false;
                Debug.Log(uwr.isHttpError ? "httpError:" : "networkError:" + uwr.error);        //エラー
            }
            else
            {
                online = true;
                string s = uwr.downloadHandler.text;    //device_id-tag_value:...
                Debug.Log(s);
                yield return Get.AllData();
                SceneManager.LoadScene("Device");
            }
        }

        //追加
        public static IEnumerator Insert(List<Device> devices)
        {
            string data = "";
            foreach(var device in devices)
            {
                data += device.GetData(new Device.Param() { param = Device.PARAM.AUTHOR }).valueString + "="
                        + device.GetData(new Device.Param() { param = Device.PARAM.CLASS }).valueString + "="
                        + device.GetData(new Device.Param() { param = Device.PARAM.CLASS_DETAIL }).valueString + "="
                        + device.GetData(new Device.Param() { param = Device.PARAM.ALD }).valueString + "="
                        + device.GetData(new Device.Param() { param = Device.PARAM.SULFURIZATION }).valueString + "="
                        + device.GetData(new Device.Param() { param = Device.PARAM.EFFICIENCY }).valueFloat + "="
                        + device.GetData(new Device.Param() { param = Device.PARAM.FF }).valueFloat + "="
                        + device.GetData(new Device.Param() { param = Device.PARAM.V_OC }).valueFloat + "="
                        + device.GetData(new Device.Param() { param = Device.PARAM.J_SC }).valueFloat + "="
                        + (device.lab ? "1" : "0") + "|" + device.url;
                var extras = device.datas.FindAll(d => d.param.param == Device.PARAM.EXTRA);
                foreach (var ex in extras)
                {
                    data += "|" + ex.param.tag + "=" + ex.valueFloat;
                }
                data += "^";
            }
            Debug.Log(data);
            UnityWebRequest uwr = PostRequest("insert", data);
            yield return uwr.SendWebRequest();    //応答待機
            if (uwr.isHttpError || uwr.isNetworkError)
            {
                online = false;
                Debug.Log(uwr.isHttpError ? "httpError:" : "networkError:" + uwr.error);        //エラー
            }
            else
            {
                online = true;
                string s = uwr.downloadHandler.text;    //device_id-tag_value:...
                Debug.Log(s);
                yield return Get.AllData();
                SceneManager.LoadScene("Device");
            }
        }

        //削除
        public static IEnumerator Remove(Device device)
        {
            string data = device.id.ToString();
            UnityWebRequest uwr = PostRequest("remove", data);
            yield return uwr.SendWebRequest();    //応答待機
            if (uwr.isHttpError || uwr.isNetworkError)
            {
                online = false;
                Debug.Log(uwr.isHttpError ? "httpError:" : "networkError:" + uwr.error);        //エラー
            }
            else
            {
                online = true;
                string s = uwr.downloadHandler.text;    //device_id-tag_value:...
                Debug.Log(s);
                yield return Get.AllData();
                SceneManager.LoadScene("Device");
            }
        }
    }

    //データ保存
    public class Cache
    {
        public static bool isHavingData = false;
        public static List<Device> devices = new List<Device>();
    }

    public class User
    {
        //ログイン
        public static IEnumerator Login()
        {
            SaveDataManager.Load();    //キャッシュをロード
            string data = SystemInfo.deviceUniqueIdentifier;    //端末ID
            UnityWebRequest uwr = PostRequest("login", data);
            yield return uwr.SendWebRequest();    //応答待機
            if (uwr.isHttpError || uwr.isNetworkError)
            {
                online = false;
                Debug.Log(uwr.isHttpError ? "httpError:" : "networkError:" + uwr.error);        //エラー
                MessageManager.Open("ネットワークエラーです。\n（学内ネットワークでは接続できません...TT）\n閲覧モードで起動します。", false, 2.5f);
                yield return new WaitForSeconds(2.5f);
                SceneManager.LoadScene("Device");
            }
            else
            {
                online = true;
                string s = uwr.downloadHandler.text;
                Debug.Log(s);
                if (s == "can use.")
                {
                    yield return Get.AllData();
                    SceneManager.LoadScene("Device");
                }
                else { MessageManager.Open("アクセスが拒否されています。"); }
            }
        }

        //新規登録
        public static IEnumerator Regist(string password, string stu_id)
        {
            string data = AES.EncryptRJ128(password);   //暗号化
            UnityWebRequest uwr = PostRequest("arrow_register", data);
            yield return uwr.SendWebRequest();    //応答待機
            if (uwr.isHttpError || uwr.isNetworkError)
            {
                online = false;
                Debug.Log(uwr.isHttpError ? "httpError:" : "networkError:" + uwr.error);        //エラー
                MessageManager.Open("ネットワークエラーです。\n（学内ネットワークでは接続できません...TT）\n");
            }
            else
            {
                online = true;
                string s = uwr.downloadHandler.text;
                Debug.Log(s);
                if (s == "register is arrowed.")
                {
                    string data2 = SystemInfo.deviceUniqueIdentifier + "|" + stu_id;
                    UnityWebRequest uwr2 = PostRequest("regist", data2);
                    yield return uwr2.SendWebRequest();    //応答待機
                    if (uwr2.isHttpError || uwr2.isNetworkError)
                    {
                        online = false;
                        Debug.Log(uwr2.isHttpError ? "httpError:" : "networkError:" + uwr2.error);        //エラー
                        MessageManager.Open("ネットワークエラーです。\n（学内ネットワークでは接続できません...TT）\n");
                    }
                    else
                    {
                        online = true;
                        string s2 = uwr2.downloadHandler.text;
                        Debug.Log(s2);
                        if (s2 == "regist complete.")
                        {
                            MessageManager.Open("登録が完了しました。\nしばらくお待ちください。", false, 2.5f);
                            yield return Get.AllData();
                            yield return new WaitForSeconds(2.5f);
                            SceneManager.LoadScene("Device");
                        }
                        else
                        {
                            MessageManager.Open("登録エラー");
                        }
                    }
                }
                else { MessageManager.Open("パスワードが違います"); }
            }
        }
    }

    //暗号化用

    public class AES
    {
        //'Shared 256 bit Key and IV here
        private const string sKy = "FtkX8zyOIXlFaqdJ";
        private const string sIV = "4JQZRZNIng8TCy0O";

        //暗号化
        public static string EncryptRJ128(string prm_text_to_encrypt)
        {
            // 文字列をバイト型配列へ
            byte[] input = System.Text.Encoding.UTF8.GetBytes(prm_text_to_encrypt);

            // AES暗号化
            AesManaged AES = new AesManaged();
            AES.KeySize = 128;   // 鍵の長さ  
            AES.BlockSize = 128; // ブロックサイズ(srcは16byteまで) 
            AES.Mode = CipherMode.CBC; // CBCモード
            AES.IV = System.Text.Encoding.UTF8.GetBytes(sIV);        // 初期化ベクトル(公開)
            AES.Key = System.Text.Encoding.UTF8.GetBytes(sKy);      // 共有キー(非公開)
            AES.Padding = PaddingMode.PKCS7;
            byte[] output = AES.CreateEncryptor().TransformFinalBlock(input, 0, input.Length);

            // BASE64
            return Convert.ToBase64String(output);
        }
    }
}


