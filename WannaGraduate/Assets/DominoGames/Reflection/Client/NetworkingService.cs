using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;
using Defective.JSON;
using UnityEngine.Events;
using System.Security.Cryptography;
using System.Text;
using System;

public class NetworkingService : MonoBehaviour
{
    public static string gameId = "com.DominoGames.ProjectF";
    public static string channelId = "";
    public static string uid = "";

    static SHA256 sha256 = null;
    public static string GetHashedUID()
    {
        sha256 ??= new SHA256Managed();

        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(uid));
        return BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
    }

    [Button]
    public static void LoadRankingOne(string boardId, UnityAction<JSONObject> responseCallback)
    {
        JSONObject data = new JSONObject();
        data.AddField("gameId", gameId);
        data.AddField("channelId", channelId);
        data.AddField("uid", uid);

        data.AddField("boardId", boardId);
        data.AddField("withscore", false);
        RequestBuilderStarter.New("lond_ranking_one").SetData(data).BlockingType(RequestBlocking.NONE).ResponseCallback(responseCallback).Send();
    }

    [Button]
    public static void LoadRankings(string boardId, int startRank, int endRank, UnityAction<JSONObject> responseCallback)
    {
        JSONObject data = new JSONObject();
        data.AddField("gameId", gameId);
        data.AddField("channelId", channelId);
        data.AddField("uid", uid);

        data.AddField("boardId", boardId);
        data.AddField("startRank", startRank);
        data.AddField("endRank", endRank);
        RequestBuilderStarter.New("load_rankings").SetData(data).BlockingType(RequestBlocking.NONE).ResponseCallback(responseCallback).Send();
    }
}