using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using TMPro;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance { get; private set; }
    private string token;
    private string baseUrl = "https://bluepath-dxadgghmenf8g6dp.northeurope-01.azurewebsites.net/api/";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetToken(string newToken)
    {
        token = newToken;
    }

    public void MakeRequest<T>(string endpoint, RequestType type, object data, Action<T> callback)
    {
        StartCoroutine(RequestCoroutine(endpoint, type, data, callback));
    }

    private IEnumerator RequestCoroutine<T>(string endpoint, RequestType type, object data, Action<T> callback)
    {
        var getRequest = CreateRequest(endpoint, type, data);
        if (endpoint != "auth/login" && endpoint != "auth/register")
        {
            AttachHeader(getRequest, "Authorization", token);
        }

        yield return getRequest.SendWebRequest();

        if (getRequest.result == UnityWebRequest.Result.Success)
        {
            string rawJson = getRequest.downloadHandler.text;

            try
            {
                ;

                if (rawJson.StartsWith("["))
                {
                    string wrappedJson = "{\"items\":" + rawJson + "}";
                    RouteStepListWrapper<T> wrapper = JsonUtility.FromJson<RouteStepListWrapper<T>>(wrappedJson);
                    if (wrapper != null && wrapper.items != null)
                    {
                        foreach (var item in wrapper.items)
                        {
                            callback?.Invoke(item);
                        }
                    }
                }
                else if (rawJson.StartsWith("{"))
                {
                    T deserializedObject = JsonUtility.FromJson<T>(rawJson);
                    callback?.Invoke(deserializedObject);
                }
                else
                {
                    Debug.LogError("Raw JSON is not in an expected format (neither array nor object)");
                    callback?.Invoke(default);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error deserializing response from {endpoint}: {ex.Message}");
                Debug.LogError($"Raw Response: {getRequest.downloadHandler.text}");
                callback?.Invoke(default);
            }


            if (typeof(T) == typeof(PostResult))
            {
                if (typeof(T) == typeof(PostResult) && (endpoint == "auth/login"))
                {
                    var result = JsonUtility.FromJson<PostResult>(getRequest.downloadHandler.text);
                    if (!string.IsNullOrEmpty(result.accessToken))
                    {
                        SetToken(result.accessToken);
                    }
                }

            }
        }
        else
        {
            Debug.LogError($"Request to {endpoint} failed. Error: {getRequest.error}, Status Code: {getRequest.responseCode}");
            Debug.LogError($"Raw Response: {getRequest.downloadHandler.text}");
            callback?.Invoke(default);
        }

    }
    [Serializable]
    private class RouteStepListWrapper<T>
    {
        public List<T> items;
    }

    private UnityWebRequest CreateRequest(string path, RequestType type = RequestType.GET, object data = null)
    {
        var url = baseUrl + path;
        var request = new UnityWebRequest(url, type.ToString());

        if (data != null)
        {
            var bodyRaw = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        }

        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        return request;
    }

    private void AttachHeader(UnityWebRequest request, string key, string value)
    {
        request.SetRequestHeader(key, $"Bearer {value}");
    }

    /*


    */
    public void SaveSpawnedNodes(
        Dictionary<string, GameObject> spawnedNodes,
        SceneGraphManager graph,
        float idleWindow = 1f,
        float hardTimeout = 2f)
    {
        //-------------------  A)  TAKE A SNAPSHOT  --------------------
        List<SpawnedNodeSnapshot> snapshot = new List<SpawnedNodeSnapshot>();

        foreach (var kvp in spawnedNodes)
        {
            string nodeId = kvp.Key;
            GameObject go = kvp.Value;
            if (go == null) continue;              // should exist right now

            RectTransform rt = go.GetComponent<RectTransform>();
            if (rt == null) continue;

            SceneNode sn = graph.sceneNodes[nodeId];

            snapshot.Add(new SpawnedNodeSnapshot
            {
                id = nodeId,
                title = sn.title,
                description = sn.description,
                x = rt.anchoredPosition.x,
                y = rt.anchoredPosition.y
            });
        }

        //-------------------  B)  CONTINUE AS BEFORE  -----------------
        StartCoroutine(CoWaitAndPost(snapshot, idleWindow, hardTimeout));
    }

    private IEnumerator CoWaitAndPost(List<SpawnedNodeSnapshot> snapshot,
                                      float idleWindow,
                                      float hardTimeout)
    {
        // --- 1) fetch existing titles exactly as before -------------
        HashSet<string> existingTitles = new HashSet<string>();
        float lastDataTime = Time.realtimeSinceStartup;
        bool gotAny = false;

        void CollectExisting(RouteStep step)
        {
            if (step == null) return;
            gotAny = true;
            existingTitles.Add(step.title);
            lastDataTime = Time.realtimeSinceStartup;
        }

        MakeRequest<RouteStep>("routes/A", RequestType.GET, null, CollectExisting);

        // --- 2) wait for idle gap or timeout ------------------------
        float start = Time.realtimeSinceStartup;
        while (true)
        {
            float now = Time.realtimeSinceStartup;
            float idle = now - lastDataTime;

            bool idleEnough = gotAny && idle >= idleWindow;
            bool timedOut = (now - start) >= hardTimeout;

            if (idleEnough || timedOut) break;
            yield return null;
        }

        // --- 3) post only nodes whose title isn't in existingTitles --
        foreach (var snap in snapshot)
        {
            if (existingTitles.Contains(snap.title))
                continue;

            RouteData payload = new RouteData
            {
                id = Guid.NewGuid(),
                routeType = "A",
                stepOrder = 0,
                title = snap.title,
                description = snap.description,
                iconName = "Icon",
                x = snap.x,
                y = snap.y
            };

            MakeRequest<PostResult>("routes", RequestType.POST, payload,
                res =>
                {
                    if (res != null)
                        Debug.Log($"[SaveSpawnedNodes] \"{snap.title}\" saved.");
                    else
                        Debug.LogError($"[SaveSpawnedNodes] Failed to save \"{snap.title}\".");
                });
        }
    }
}


    [Serializable]
public struct SpawnedNodeSnapshot
{
    public string id;
    public string title;
    public string description;
    public float x;
    public float y;
}

public enum RequestType
{
    GET,
    POST,
    PUT,
    DELETE
}

public class PostResult
{
    public string accessToken;
}