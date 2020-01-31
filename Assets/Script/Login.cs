using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Threading.Tasks;

public class Login : MonoBehaviour
{
    // Cached references
    public InputField emailInputField;
    public InputField passwordInputField;
    public Button loginButton;
    public Button logoutButton;
    private string httpServerAddress;
    public GameManager gameManager;
    public Text messageBoardText;

    private void Start()
    {
        httpServerAddress = gameManager.GetHttpServer();
    }

    public void OnLoginButtonClicked()
    {
        TryLogin();
    }

    private void TryLogin()
    {
        if (string.IsNullOrEmpty(gameManager.Token))
        {
            GetToken();
        }

        UnityWebRequest httpClient = new UnityWebRequest(httpServerAddress + "/api/Account/UserId", "GET");

        httpClient.SetRequestHeader("Authorization", "bearer " + gameManager.Token);
        httpClient.SetRequestHeader("Accept", "application/json");

        httpClient.downloadHandler = new DownloadHandlerBuffer();
        httpClient.certificateHandler = new BypassCertificate();
        httpClient.SendWebRequest();

        while (!httpClient.isDone)
        {
            Task.Delay(1);
        }

        if (httpClient.isNetworkError || httpClient.isHttpError)
        {
            Debug.Log(httpClient.error);
        }
        else
        {
            gameManager.PlayerId = httpClient.downloadHandler.text;
            messageBoardText.text += "\nWelcome " + gameManager.PlayerId + ". You are logged in!";
            loginButton.interactable = false;
            logoutButton.interactable = true;
        }

        httpClient.Dispose();
    }

    private void GetToken()
    {
        UnityWebRequest httpClient = new UnityWebRequest(httpServerAddress + "/Token", "POST");

        WWWForm dataToSend = new WWWForm();
        dataToSend.AddField("grant_type", "password");
        dataToSend.AddField("username", emailInputField.text);
        dataToSend.AddField("password", passwordInputField.text);

        httpClient.uploadHandler = new UploadHandlerRaw(dataToSend.data);
        httpClient.downloadHandler = new DownloadHandlerBuffer();

        httpClient.SetRequestHeader("Accept", "application/json");
        httpClient.certificateHandler = new BypassCertificate();
        httpClient.SendWebRequest();

        while (!httpClient.isDone)
        {
            Task.Delay(1);
        }
        
        if(httpClient.isNetworkError || httpClient.isHttpError)
        {
            Debug.Log(httpClient.error);
        }
        else
        {
            string jsonResponse = httpClient.downloadHandler.text;
            Token authToken = JsonUtility.FromJson<Token>(jsonResponse);
            gameManager.Token = authToken.access_token;
        }
        httpClient.Dispose();
    }

    public void OnLogoutButtonClicked()
    {
        TryLogout();
    }

    private void TryLogout()
    {
        UnityWebRequest httpClient = new UnityWebRequest(httpServerAddress + "/api/Account/Logout", "POST");
        httpClient.SetRequestHeader("Authorization", "bearer " + gameManager.Token);
        httpClient.certificateHandler = new BypassCertificate();
        httpClient.SendWebRequest();

        while (!httpClient.isDone)
        {
            Task.Delay(1);
        }

        if (httpClient.isNetworkError || httpClient.isHttpError)
        {
            Debug.Log(httpClient.error);
        }
        else
        {
            messageBoardText.text += $"\n{httpClient.responseCode} Bye bye {gameManager.PlayerId}.";
            gameManager.Token = string.Empty;
            gameManager.PlayerId = string.Empty;
            loginButton.interactable = true;
            logoutButton.interactable = false;
        }
    }
}
