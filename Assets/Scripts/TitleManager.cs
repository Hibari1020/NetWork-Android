using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public InputField _inputField;
    public Button _loginButton;
    public Text _unenableText;
    
    // Start is called before the first frame update
    void Start()
    {
        _loginButton.onClick.AddListener(OnClickLoginButton);
    }

    public void OnClickLoginButton()
    {
        string _inputText = _inputField.text;
        int _characterCount = _inputText.Length;

        if(_characterCount >= 1 && _characterCount <= 6)
        {
            UserLoginName.userName = _inputText;
             SceneManager.LoadScene("PlayScene");
        }
        else 
        {
           _unenableText.text = "1文字以上6文字以下にしてください";

        }
    }

}
