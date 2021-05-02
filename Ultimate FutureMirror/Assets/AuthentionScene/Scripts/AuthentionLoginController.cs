using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AuthentionLoginController : MonoBehaviour
{
    [Header("Text for Authention")]
    [SerializeField] private InputField _textLogin = null;
    [SerializeField] private int _lenghtLoginMin = 3;
    [SerializeField] private InputField _textPass = null;
    [SerializeField] private int _lenghtPassMin = 8;

    [Header("Variable Message Box")]
    [SerializeField] private GameObject _messageBoxControllerObj = null;
    [SerializeField] private MessageBoxController _messageBoxController = null;

    #region Validation Filed
    private void ValidationMessageBoxController()
    {
        if (_messageBoxController != null)
        {
            return;
        }
        else
        {
            if (_messageBoxControllerObj != null)
            {
                _messageBoxController = _messageBoxControllerObj.GetComponent<MessageBoxController>();
            }
        }

    }
    #region Validation Login
    private bool IsLoginCorrect()
    {
        if (_textLogin.text.Length >= _lenghtLoginMin)
            return true;
        else
            return false;
    }
    #endregion
    #region Valiadtion Password
    private bool IsPasswordCorrect()
    {
        if (_textPass.text.Length >= _lenghtPassMin)
            return true;
        else
            return false;
    }
    #endregion
    #endregion

    #region Execiute Login With DataBase 
    public void ExeciuteLoginPressButtonLogin()
    {
        if (IsLoginCorrect() && IsPasswordCorrect())
            StartCoroutine(Login(_textLogin.text, _textPass.text));
        else if (!IsLoginCorrect() && !IsPasswordCorrect())
        {
            _messageBoxControllerObj.SetActive(true);
            ValidationMessageBoxController();
            _messageBoxController.DisplayTextMessageBox("Complete all fields!");
        }
        else if (!IsLoginCorrect())
        {
            _messageBoxControllerObj.SetActive(true);
            ValidationMessageBoxController();
            _messageBoxController.DisplayTextMessageBox("Your Login is to short minimum sign:[" + _lenghtLoginMin + "]!");
        }
        else if (!IsPasswordCorrect())
        {
            _messageBoxControllerObj.SetActive(true);
            ValidationMessageBoxController();
            _messageBoxController.DisplayTextMessageBox("Your password does not meet the requirements, minimum sign:[" + _lenghtPassMin + "]!");
        }
    }
    IEnumerator Login(string username, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("loginUser", username);
        form.AddField("loginPass", password);

        using (UnityWebRequest www = UnityWebRequest.Post("https://unityfsadsa.000webhostapp.com/gameUnity/Login.php", form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                _messageBoxController.DisplayTextMessageBox(www.error);
            }
            else
            {
                if (www.downloadHandler.text == "Login Success")
                {
                    //Login Succes Request From DB
                    _messageBoxControllerObj.SetActive(true);
                    ValidationMessageBoxController();
                    _messageBoxController.DisplayTextMessageBox(www.downloadHandler.text);
                    //PlayerPrefs.SetString("UsernameLogin", username);
                    //PlayerPrefs.SetString("UsernamePassword", password);
                }
                else if (www.downloadHandler.text == "Input activate code from email")
                {
                    //Login Succes Input Activate Code From Email Request from DB
                    _messageBoxControllerObj.SetActive(true);
                    ValidationMessageBoxController();
                    _messageBoxController.DisplayTextMessageBox(www.downloadHandler.text);
                }
                _messageBoxControllerObj.SetActive(true);
                ValidationMessageBoxController();
                _messageBoxController.DisplayTextMessageBox(www.downloadHandler.text);
            }
        }
    }
    #endregion
}
