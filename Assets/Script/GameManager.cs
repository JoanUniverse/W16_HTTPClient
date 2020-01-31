using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private const string httpserver = "https://localhost:44353";

    public string GetHttpServer()
    {
        return httpserver;
    }

    private string _token;
    public string Token
    {
        get { return _token; }
        set { _token = value; }
    }

    private string _playerId;
    public string PlayerId
    {
        get { return _playerId; }
        set { _playerId = value; }
    }


}
