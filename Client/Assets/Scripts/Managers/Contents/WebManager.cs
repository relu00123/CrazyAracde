using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class WebManager
{
    public string BaseUrl { get; set; } = "http://192.168.219.109:5000/api"; //"https://localhost:5001/api";

	 

    public void SendPostRequest<T>(string url, object obj, Action<T> res)
	{
		Managers.Instance.StartCoroutine(CoSendWebRequest(url, UnityWebRequest.kHttpVerbPOST, obj, res));
	}


	// 추가된 코드 
    public bool DisableCertificateValidation(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        return true;
    }

    class FreeCertificate : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true; // Always accept
        }
    }



    IEnumerator CoSendWebRequest<T>(string url, string method, object obj, Action<T> res)
	{
		string sendUrl = $"{BaseUrl}/{url}";
		Debug.Log($"Send URL : {sendUrl}");

		byte[] jsonBytes = null;
		if (obj != null)
		{
			string jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
			jsonBytes = Encoding.UTF8.GetBytes(jsonStr);
		}

			// 추가된 코드 
        ServicePointManager.ServerCertificateValidationCallback = DisableCertificateValidation;


        using (var uwr = new UnityWebRequest(sendUrl, method))
		{
			uwr.uploadHandler = new UploadHandlerRaw(jsonBytes);
			uwr.downloadHandler = new DownloadHandlerBuffer();
            uwr.SetRequestHeader("Content-Type", "application/json");
            uwr.certificateHandler = new FreeCertificate();
            yield return uwr.SendWebRequest();

			if (uwr.isNetworkError || uwr.isHttpError)
			{
				Debug.Log(uwr.error);
                T resObj = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(uwr.downloadHandler.text);
                res.Invoke(resObj);
            }
			else
			{
				T resObj = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(uwr.downloadHandler.text);
				res.Invoke(resObj);
			}
		}
	}
}
