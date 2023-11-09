using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using MM;
using MM.Msg;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;
using ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling;

public class NetCenter : MM.Singleton<NetCenter>
{
    private Dictionary<string, Sprite> iconCache = new Dictionary<string, Sprite>();

    //cache gif： Avoid repeated download file
    private Dictionary<string, byte[]> gifCache = new Dictionary<string, byte[]>();

    public CompleteTokenSource Send<T>(IMessage msg, Action<IMessage> success_callback, Action<string> faill_callback)
        where T : IMessage, new()
    {
        CompleteTokenSource completeToken = new CompleteTokenSource();

        StartCoroutine(AsyncSend<T>(msg, success_callback, faill_callback, completeToken));

        return completeToken;
    }

    public IEnumerator AsyncSend<T>(IMessage msg, Action<IMessage> success_callback, Action<string> faill_callback,
        CompleteTokenSource onCompleteCTS = null)
        where T : IMessage, new()
    {
        string url;
        UnityWebRequest request;
        var ack = new T();
        var headers = new Dictionary<string, string>
        {
            {"Content-Type", "application/json"}
        };
        headers.Add("Authorization", "Bearer " + "47e93ea948f856482ed2d1e1e45836187df0c48387b567b3b9973963dcbd1c91");
        
        // if (IgroveAppdata.HasJWTToken)
        // {
        //     headers.Add("Authorization", "Bearer " + IgroveAppdata.JWTToken);
        // }
        // else if (IgroveAppdata.TemporaryJWT != String.Empty)
        // {
        //     headers.Add("Authorization", "Bearer " + IgroveAppdata.TemporaryJWT);
        // }

        if (!string.IsNullOrEmpty(msg.PostUrl()))
        {
            url = msg.PostUrl();
            request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
            request.certificateHandler = new AcceptAllCertificateForTest();

            var data = msg.encrypted()
                ? Encoding.UTF8.GetBytes("{\"request\": \"" + NetHelper.Encrypt(msg.ToJson()) + "\"}")
                : Encoding.UTF8.GetBytes(msg.ToJson());

            request.uploadHandler = new UploadHandlerRaw(data);
            request.downloadHandler = new DownloadHandlerBuffer();
            foreach (var item in headers)
            {
                request.SetRequestHeader(item.Key, item.Value);
            }
        }
        else if (!string.IsNullOrEmpty(msg.PutUrl()))
        {
            url = msg.PutUrl();
            request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPUT);
            request.certificateHandler = new AcceptAllCertificateForTest();
            var data = msg.encrypted()
                ? Encoding.UTF8.GetBytes("{\"request\": \"" + NetHelper.Encrypt(msg.ToJson()) + "\"}")
                : Encoding.UTF8.GetBytes(msg.ToJson());
            request.uploadHandler = new UploadHandlerRaw(data);
            request.downloadHandler = new DownloadHandlerBuffer();
            foreach (var item in headers)
            {
                request.SetRequestHeader(item.Key, item.Value);
            }
        }
        else
        {
            url = msg.GetUrl();

            Dictionary<string, string> values =
                JsonConvert.DeserializeObject<Dictionary<string, string>>(msg.ToJson());

            if (values != null)
            {
                var queryParams = string.Join("&", values.Select(pair => $"{pair.Key}={pair.Value}"));
                var urlQuery = string.IsNullOrEmpty(queryParams) ? "" : "?" + queryParams;

                url += urlQuery;
            }

            request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET);
            request.certificateHandler = new AcceptAllCertificateForTest();
            request.downloadHandler = new DownloadHandlerBuffer();
            foreach (var item in headers)
            {
                request.SetRequestHeader(item.Key, item.Value);
            }
        }

        // Debug.Log(request.GetRequestHeader("Authorization"));
        var op = request.SendWebRequest();

        yield return new WaitUntil(() => op.isDone);

        onCompleteCTS?.Cancel();

        request.uploadHandler?.Dispose();
        if (!string.IsNullOrEmpty(request.error))
        {
            faill_callback?.Invoke(request.error);
            yield break;
        }

        string results = new T().encrypted()
            ? NetHelper.Decrypt(request.downloadHandler.text)
            : request.downloadHandler.text;
        try
        {
            //Workround: replace space with underscore
            if (results.StartsWith("{"))
            {
                JObject parsedObj = JsonConvert.DeserializeObject<JObject>(results);
                var newObj = new JObject();
                foreach (var item in parsedObj)
                {
                    newObj.Add(item.Key.Replace(" ", "_"), item.Value);
                }

                results = JsonConvert.SerializeObject(newObj);
            }

            Printer.Print($"Url:{url}, Result: {results}");
            var settings = new Newtonsoft.Json.JsonSerializerSettings();
            // This tells your serializer that multiple references are okay.
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            ack = JsonConvert.DeserializeObject<T>(results, settings);
            Printer.Print($"ack {ack.ToJson()}");
        }
        catch (Exception e)
        {
            Printer.PrintWarning($"Error {e.ToString()}");
            faill_callback?.Invoke(e.Data != null ? e.Data.ToString() : "null");
            yield break;
        }

        success_callback?.Invoke(ack);
    }
    

}

public class AcceptAllCertificateForTest : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        return true;
    }
}