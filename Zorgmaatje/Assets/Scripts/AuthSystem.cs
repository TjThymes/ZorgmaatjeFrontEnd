using UnityEngine;
using Unity;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text;
using AuthModels;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using JetBrains.Annotations;
using System;
using UnityEngine.Rendering;
    [Serializable]
    public class RegisterUser
    {
        public string Fullname;
        public string Password;
        public string Email;
        public string AccountType;
    }
    [Serializable]
    public class LoginUser
    {
        public string Email;
        public string Password;
    }
public class AuthSystem : MonoBehaviour
{
    private string baseUrl = "https://bluepath-dee5akbebjdnemeu.westeurope-01.azurewebsites.net/api/";
    public GameObject LoginButton,LoginPassword,LoginEmail;
    public GameObject RegisterButton, RegisterPassword, RegisterUsername, RegisterEmail, RegisterKindType,RegisterVoogdType,RegisterZorgType;
    public GameObject ICEButton, ICEScreen, ICECloseButton;
    
    public TMP_Text feedbackTextRegister,feedbackTextLogin;
     
    public string selectedAccountType = "Kind";//Kind-Ouder-Zorgverlener
    public void ToggleICEScreen()
    {
        ICEScreen.SetActive(!ICEScreen.activeInHierarchy);
    }
    public void SetAccountType(GameObject ButtonClicked)
    {
        //selectedAccountType = ButtonClicked.transform.GetChild(1).GetComponent<TMP_Text>().text;
        List<GameObject> Buttons = new List<GameObject>
        {
            RegisterKindType,
            RegisterVoogdType,
            RegisterZorgType
        };
       foreach (GameObject v in Buttons)
       {
            UnityEngine.UI.Image colorComponent = v.GetComponent<UnityEngine.UI.Image>();
            if (v==ButtonClicked)
            {
                selectedAccountType=v.name;
                colorComponent.color = new Color32(255,255,225,255);
            }
            else
            {
                colorComponent.color = new Color32(255,255,255,100);
            }
        
       }
       
        
    }
    public void Register()
    {
        TMP_InputField usernameInputReg,emailInputReg,passwordInputReg;
        usernameInputReg = RegisterUsername.GetComponent<TMP_InputField>();
        emailInputReg = RegisterEmail.GetComponent<TMP_InputField>();
        passwordInputReg = RegisterPassword.GetComponent<TMP_InputField>();
       
        if (string.IsNullOrWhiteSpace(usernameInputReg.text) ||
            string.IsNullOrWhiteSpace(emailInputReg.text) ||
            string.IsNullOrWhiteSpace(passwordInputReg.text))
        {
            feedbackTextRegister.text = "❌ Please fill in all fields!";
            return;
        }


        RegisterUser newUser = new RegisterUser
        {
            Fullname = usernameInputReg.text,
            Email = emailInputReg.text,
            Password = passwordInputReg.text,
            AccountType = selectedAccountType
        };

        NetworkManager.Instance.MakeRequest<PostResult>("auth/register", RequestType.POST, newUser, response => {
            if (response != null)
            {
                Debug.Log("User registered: " + usernameInputReg.text);
            }
            else
            {
                feedbackTextRegister.text = "❌ Registration failed.";
            }
        });

    }
    public void Login()
    {
        TMP_InputField emailInputLogin,passwordInputLogin;
        emailInputLogin = LoginEmail.GetComponent<TMP_InputField>();
        passwordInputLogin = LoginPassword.GetComponent<TMP_InputField>();
       
        if (string.IsNullOrWhiteSpace(emailInputLogin.text) ||
            string.IsNullOrWhiteSpace(passwordInputLogin.text))
        {
            feedbackTextRegister.text = "❌ Please fill in all fields!";
            return;
        }
 
 
        LoginUser newUser = new LoginUser
        {
            Email = emailInputLogin.text,
            Password = passwordInputLogin.text,
        };


        NetworkManager.Instance.MakeRequest<PostResult>("auth/login", RequestType.POST, newUser, response => {
            if (response != null)
            {
                Debug.Log("User logged in: " + emailInputLogin.text);
                Authorized();
            }
            else
            {
                feedbackTextLogin.text = "❌ Login failed.";
            }
        });
    }
    private void Authorized()
    {

       SceneManager.LoadScene("HomeScene"); //switch scene 
    }

 
    private IEnumerator RegisterUser(RegisterUser userData)
    {
        string jsonData = JsonUtility.ToJson(userData);
        using (UnityWebRequest request = new UnityWebRequest(baseUrl + "auth/register", "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("User registered: " + jsonData);
                Authorized();
            }
            else
            {
                Debug.LogError(request.downloadHandler.error);
                string responseText = request.downloadHandler.text;
                Debug.LogError("Registration Error: " + responseText);
 
                if (responseText.Contains("Username is already taken"))
                {
                    feedbackTextRegister.text = "❌ Username is already taken!";
                }
                else if (responseText.Contains("Email is already registered"))
                {
                    feedbackTextRegister.text = "❌ Email is already registered!";
                }
                else
                {
                    feedbackTextRegister.text = "❌ Registration failed: " + request.error;
                }
 
            }
        }
    }
     private IEnumerator LoginUser(LoginUser userData)
    {
        string jsonData = JsonUtility.ToJson(userData);
        using (UnityWebRequest request = new UnityWebRequest(baseUrl + "auth/login", "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("User logged in: " + jsonData);
                Authorized();
            }
            else
            {
                Debug.LogError(request.downloadHandler.error);
                string responseText = request.downloadHandler.text;
                Debug.LogError("Registration Error: " + responseText);
 
                if (responseText.Contains("Username is already taken"))
                {
                    feedbackTextRegister.text = "❌ Username is already taken!";
                }
                else if (responseText.Contains("Email is already registered"))
                {
                    feedbackTextRegister.text = "❌ Email is already registered!";
                }
                else
                {
                    feedbackTextRegister.text = "❌ Registration failed: " + request.error;
                }
 
            }
        }
    }
}
