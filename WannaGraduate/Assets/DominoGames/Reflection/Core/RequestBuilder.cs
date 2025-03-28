using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Defective.JSON;
using UnityEngine.Events;

public enum RequestBlocking
{
    ALL,  // 내가 보낸 request의 response 가 도착할 때까지, 모든 request 를 block함
    SELF, // response 가 도착할 때까지, 내 eventName 의 request 를 block
    NONE // 아무런 blocking 을 걸지 않음.
}

public enum RequestType
{
    GET,
    POST
}

// human fail 방지용 클래스입니다.
// 패킷을 만들때는 무조건 New 로 시작해야합니다.
public class RequestBuilderStarter{
    public static RequestBuilder builder = new RequestBuilder();

    public static RequestBuilder New(){
        builder.New();
        return builder;
    }

    public static RequestBuilder New(string eventName){
        builder.New().SetEventName(eventName);
        return builder;
    }
}

public class RequestBuilder : MonoBehaviour
{
    private PacketData packet;

    public RequestBuilder New(){
        packet = new PacketData();
        return this;
    }

    public RequestBuilder SetEventName(string eventName){
        packet.eventName = eventName;
        return this;
    }

    public RequestBuilder SetData(JSONObject data){
        packet.data = data;
        return this;
    }

    public RequestBuilder ResponseCallback(UnityAction<JSONObject> callback){
        packet.responseCallback = callback;
        return this;
    }

    public RequestBuilder RequestType(RequestType requestType)
    {
        packet.requestType = requestType;
        return this;
    }

    public RequestBuilder BlockingType(RequestBlocking type){
        packet.requestBlocking = type;
        return this;
    }

    public RequestBuilder OnRequestBlocked(UnityAction callback){
        packet.onRequestBlocked = callback;
        return this;
    }

    public RequestBuilder OnError(UnityAction callback){
        packet.onError = callback;
        return this;
    }

    public void Send(){
        NetworkingManager.instance.SendRequest(packet);
    }
}

// 패킷 데이터 클래스입니다.
public class PacketData
{
    public string eventName = "";
    public JSONObject data = null; // packet data
    public UnityAction<JSONObject> responseCallback = null; // response 콜백
    public RequestBlocking requestBlocking = RequestBlocking.SELF; // requestBlocking Status
    public RequestType requestType = RequestType.POST;
    public UnityAction onRequestBlocked = null; // requestBlocking에 의해 block 되면 실행되는 콜백
    public UnityAction onError = null; // 에러 발생시 실행되는 콜백
}

public class RequestBlockingManager
{
    // 이전 리퀘스트 상태에 따라 특정 혹은 전체 Web Request 를 허용 / 지연 / 차단을 관리하는 매니저입니다.
    private bool blockingRequestAll = false;
    private List<string> blockedEventNameList = new List<string>();

    public void ClearBlockingRequest(){
        blockingRequestAll = false;
        blockedEventNameList.Clear();
    }

    public bool IsRequestBlocked(string eventName){
        // 모든 request 가 막혀 있는 상태 (blockingRequestAll = true) 이거나,
        // 혹은 selfBlockingRequest에 의해 내 EventName Request가 막혀 있는 상태이면 true 반환
        return blockingRequestAll || blockedEventNameList.Contains(eventName);
    }

    public void ReleaseBlocking(string eventName){
        blockedEventNameList.Remove(eventName);
    }

    public void ReleaseBlocking(PacketData packet){
        switch(packet.requestBlocking){
            case RequestBlocking.ALL:
                blockingRequestAll = false;
                break;

            case RequestBlocking.SELF:
                ReleaseBlocking(packet.eventName);
                break;
        }
    }

    public void AddBlocking(string eventName){
        // eventName 은 List 안에서 유일함.
        if(blockedEventNameList.IndexOf(eventName) == -1){
            blockedEventNameList.Add(eventName);
        }
    }

    public void AddBlocking(PacketData packet){
        switch(packet.requestBlocking){
            case RequestBlocking.ALL:
                blockingRequestAll = true;
                break;

            case RequestBlocking.SELF:
                blockedEventNameList.Add(packet.eventName);
                break;
        }
    }
}