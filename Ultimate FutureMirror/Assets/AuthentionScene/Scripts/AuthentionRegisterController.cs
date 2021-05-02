using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AuthentionRegisterController : MonoBehaviour
{
    [Header("Input from Box Register")]
    [SerializeField] private InputField _textLogin = null;
    [SerializeField] private int _lenghtLoginMin=3;
    [SerializeField] private InputField _textEmail = null;
    [SerializeField] private int _lenghtEmailMin = 6;
    [SerializeField] private InputField _textPass = null;
    [SerializeField] private int _lenghtPassMin = 8;
    [SerializeField] private InputField _textRepPass = null;

    [Header("Variable Message Box")]
    [SerializeField]private GameObject _messageBoxControllerObj = null;
    [SerializeField]private MessageBoxController _messageBoxController = null;


    #region Validation Input Filed
    #region Validation Password
    private bool IsPassRepIsCorrcet()
    {
        #region Is Componnet Input Field is null
        if (_textPass == null)
        {
            Debug.LogError("Input Text Password is NULL");
            return false;
        }
        if (_textRepPass==null)
        {
            Debug.LogError("Input Text RepPassword is NULL");
            return false ;
        }
        #endregion

        if (_textPass.text == _textRepPass.text)
            return true;
        else
            return false;
    }
    #endregion
    #region Validation Login
    private bool IsLoginCorrect()
    {
        if (_textLogin.text.Length>= _lenghtLoginMin)
        {
            return true;
        }
        return false;
    }
    #endregion
    #region Validation Email
    private bool IsEmailCorrect()
    {
        if (_textEmail.text.Length >= _lenghtEmailMin)
            return true;
        else
            return false;   
    }
    #endregion
    #endregion
    #region Execiute Form Data Base
    public void ExeciuteRequestToDataBase()
    {
        if (IsPassRepIsCorrcet() && IsLoginCorrect() && IsEmailCorrect())
            Register(_textLogin.text, _textPass.text, _textEmail.text);
        else
        {
            if (_messageBoxControllerObj != null)
            {
                _messageBoxControllerObj.SetActive(true);
                _messageBoxController = _messageBoxControllerObj.GetComponent<MessageBoxController>();
                _messageBoxController.DisplayTextMessageBox("Compleat fields!");
            }       
        }
           
    }
    IEnumerator Register(string usernameReg, string passwordReg, string mailReg)
    {
        WWWForm form = new WWWForm();
        form.AddField("registerUser", usernameReg);
        form.AddField("registerPass", passwordReg);
        form.AddField("registerMail", mailReg);

        using (UnityWebRequest www = UnityWebRequest.Post("https://unityfsadsa.000webhostapp.com/gameUnity/Register.php", form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                //Display Error
                if (_messageBoxControllerObj != null)
                {
                    _messageBoxControllerObj.SetActive(true);
                    _messageBoxController = _messageBoxControllerObj.GetComponent<MessageBoxController>();
                    _messageBoxController.DisplayTextMessageBox(www.error);
                }               
            }
            else
            {
                //Display Request Correct From Data Base
                //Display Error
                if (_messageBoxControllerObj != null)
                {
                    _messageBoxControllerObj.SetActive(true);
                    _messageBoxController = _messageBoxControllerObj.GetComponent<MessageBoxController>();
                    _messageBoxController.DisplayTextMessageBox(www.downloadHandler.text);
                }               
            }
        }
    }
    #endregion
}
