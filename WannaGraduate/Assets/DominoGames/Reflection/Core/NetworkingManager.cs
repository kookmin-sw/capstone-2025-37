using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Defective.JSON;

public class NetworkingManager : MonoBehaviour
{
    public RequestBlockingManager requestBlockingManager = new RequestBlockingManager();
    public bool isDebug;
    public string serverUrl = "http://158.247.203.121:35585/";

    private static int packetId = 0;

    public static NetworkingManager instance;
    private void Awake() {
        if(instance == null){
            instance = this;

            if (isDebug && Application.platform == RuntimePlatform.WindowsEditor)
            {
                serverUrl = "http://127.0.0.1:8982/";
            }

            DontDestroyOnLoad(gameObject);
        }
        else{
            DestroyImmediate(gameObject);
        }
    }

    public void SendRequest(PacketData packet){
        if(requestBlockingManager.IsRequestBlocked(packet.eventName)){
            packet.onRequestBlocked?.Invoke();
            return;
        }

        requestBlockingManager.AddBlocking(packet);// 패킷의 requestBlockingType 에 의한 Blocking 처리

        StartCoroutine(RequestCoroutine(packet));
    }

    UnityWebRequestAsyncOperation operation;
    private UnityWebRequest Send(PacketData packet)
    {
        UnityWebRequest request = null;
        if (packet.requestType == RequestType.GET)
        {
            string queries = "";

            if (packet.data.keys.Count > 0)
            {
                for (int i = 0; i < packet.data.keys.Count; i++)
                {
                    string dataKey = packet.data.keys[i];
                    queries += (i == 0 ? "?" : "&") + dataKey + "=" + packet.data[dataKey].ToString();
                }
            }
            request = UnityWebRequest.Get(serverUrl + packet.eventName + queries);
        }
        else
        {
            WWWForm postBody = new WWWForm();

            packet.data.AddField("packetId", packetId);

            postBody.AddField("eventName", packet.eventName);
            postBody.AddField("data", packet.data.ToString());

            request = UnityWebRequest.Post(serverUrl + packet.eventName, postBody);
        }

        return request;
    }
    IEnumerator RequestCoroutine(PacketData packet){
        UnityWebRequest request = Send(packet);

        request.timeout = 5;

        Debug.Log("[ReqPacket] " + packet.data.ToString());

        bool isError = true;
        for(int i=0;i<10;i++){
            try{
                operation = request.SendWebRequest();
            }
            catch(System.Exception e){
                packet.onError?.Invoke();
                requestBlockingManager.ReleaseBlocking(packet);
                request.Dispose();
            }

            yield return operation;

            if(request.result == UnityWebRequest.Result.Success){
                isError = false;
                break;
            }
            else{
                request.Dispose();
                request = Send(packet);
            }
            yield return new WaitForSecondsRealtime(0.5f);
        }

        if(isError){
            packet.onError?.Invoke();
            request.Dispose();
            yield break;
        }

        requestBlockingManager.ReleaseBlocking(packet);

        JSONObject result = ParsePacketString(request.downloadHandler.text);
        CommitDataProcessor.ApplyData(result);
        packet.responseCallback?.Invoke(result);

        request.Dispose();
    }

    public JSONObject ParsePacketString(string result, bool debug = true){
        result = System.Text.RegularExpressions.Regex.Replace(result, @"\\u(?<Value>[a-zA-Z0-9]{4})", m => {
            return ((char)int.Parse(m.Groups["Value"].Value, System.Globalization.NumberStyles.HexNumber)).ToString();
        });

        if (debug && (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor))
        {
            Debug.Log("[RecvPacket] " + result);
        }

        return new JSONObject(result);
    }
}
