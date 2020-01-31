
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text;
using System;
using System.Collections;
using System.Threading.Tasks;

public class Register : MonoBehaviour
{
    // Cached references
    public InputField emailInputField;
    public InputField passwordInputField;
    public InputField confirmPasswordInputField;
    public Button registerButton;
    public Text messageBoardText;
    public GameManager gameManager;

    string httpServer;
    private void Start()
    {
        httpServer = gameManager.GetHttpServer();
    }

    public void OnRegisterButtonClick()
    {
        StartCoroutine(RegisterUser());

        //UnityWebRequest www = UnityWebRequest.Get(httpServer);
        //www.SendWebRequest();
        //while (!www.isDone)
        //{
        //    Task.Delay(10);
        //}
        //if (www.isNetworkError || www.isHttpError)
        //{
        //    throw new Exception("OnRegisterButtonClick: Network Error");
        //}
        //messageBoardText.text = "\n" + www.responseCode;
        //Debug.Log(www.downloadHandler.text + " " + www.isHttpError + " " + www.responseCode + " " + www.isNetworkError);

        //www.Dispose();
    }

    public IEnumerator RegisterUser()
    {
        UnityWebRequest httpClient = new UnityWebRequest(httpServer + "/api/Account/Register", "POST");
        UserRegister newUser = new UserRegister();
        newUser.Email = emailInputField.text;
        newUser.Password = passwordInputField.text;
        newUser.ConfirmPassword = confirmPasswordInputField.text;

        string jsonData = JsonUtility.ToJson(newUser);
        byte[] dataToSend = Encoding.UTF8.GetBytes(jsonData);
        httpClient.uploadHandler = new UploadHandlerRaw(dataToSend);

        httpClient.SetRequestHeader("Content-Type", "application/json");

        httpClient.certificateHandler = new BypassCertificate();

        yield return httpClient.SendWebRequest();

        if (httpClient.isNetworkError || httpClient.isHttpError)
        {
            throw new Exception("OnRegisterButtonClick: Network Error" + httpClient.error);
        }
        messageBoardText.text = "\n" + httpClient.responseCode;

        httpClient.Dispose();


    }
}
