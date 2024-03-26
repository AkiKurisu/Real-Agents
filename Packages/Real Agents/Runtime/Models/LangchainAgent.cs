using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
namespace Kurisu.RealAgents
{
    public class LangChainAgent
    {
        public const string DefaultAPI = "http://127.0.0.1:8000";
        private readonly string base_api;
        public LangChainAgent()
        {
            base_api = DefaultAPI;
        }
        public LangChainAgent(string url)
        {
            base_api = url;
        }
        public async Task<string> Query(string inputPrompt, string storeGuid, CancellationToken ct)
        {
            using UnityWebRequest request = new($"{base_api}/query", "POST");
            byte[] data = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(new QueryRequest()
            {
                query = inputPrompt,
                guid = storeGuid
            }));
            request.uploadHandler = new UploadHandlerRaw(data);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SendWebRequest();
            while (!request.isDone)
            {
                ct.ThrowIfCancellationRequested();
                await Task.Yield();
            }
            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseJson = request.downloadHandler.text;
                ResponseMessage responseData = JsonUtility.FromJson<ResponseMessage>(responseJson);
                Debug.Log(responseJson);
                return responseData.result;
            }
            else
            {
                Debug.LogError($"Server ResponseCode: {request.responseCode}\nResponse: {request.downloadHandler.text}");
                return string.Empty;
            }
        }
        public async Task<bool> Persist(string path, string storeGuid, CancellationToken ct)
        {
            using UnityWebRequest request = new($"{base_api}/persist", "POST");
            byte[] data = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(new PersistRequest()
            {
                path = path,
                guid = storeGuid
            }));
            request.uploadHandler = new UploadHandlerRaw(data);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SendWebRequest();
            while (!request.isDone)
            {
                ct.ThrowIfCancellationRequested();
                await Task.Yield();
            }
            if (request.result == UnityWebRequest.Result.Success)
            {
                return true;
            }
            else
            {
                Debug.LogError($"Server ResponseCode: {request.responseCode}\nResponse: {request.downloadHandler.text}");
                return false;
            }
        }
        public async Task<long> Initialize(string apiKey, CancellationToken ct)
        {
            using UnityWebRequest request = new($"{base_api}/initialize", "POST");
            byte[] data = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(new InitializeRequest()
            {
                apiKey = apiKey
            }));
            request.uploadHandler = new UploadHandlerRaw(data);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SendWebRequest();
            while (!request.isDone)
            {
                ct.ThrowIfCancellationRequested();
                await Task.Yield();
            }
            return request.responseCode;
        }
        private class QueryRequest
        {
            public string query;
            public string guid;
        }
        private class InitializeRequest
        {
            public string apiKey;
        }
        private class PersistRequest
        {
            public string path;
            public string guid;
        }
        private class ResponseMessage
        {
            public string result;
        }
    }
}
