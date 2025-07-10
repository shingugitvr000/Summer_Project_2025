using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimplePlayerPrefs : MonoBehaviour
{
    public InputField nameInput;                        //�۾��� �Է� ���� �� �ִ� ui
    public Text scoreText;                              //���ھ� ui Text
    public Button saveButton;                           //���� ��ư
    public Button loadButton;                           //�ε� ��ư

    int currentScore = 0;                               //���� ���ھ�
   
    void Start()
    {
        saveButton.onClick.AddListener(SaveData);                   //���̺� ��ư�� �������� SaveData �Լ��� ���� �Ѵ�. 
        loadButton.onClick.AddListener(LoadData);                   //�ε� ������ ��ư�� �������� LoadData �Լ��� ���� �Ѵ�. 

        LoadData();                                                 //���� �Ҷ� �ڵ� �ε�
    }

    
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))                             //�����̽��� ������ ���� 10���� �ö󰣴�. 
        {
            currentScore += 10;
            scoreText.text = "score " + currentScore;
        }
    }

    void SaveData()                 //������ ���� �Լ�
    {
        PlayerPrefs.SetString("PlayerName", nameInput.text);            //�÷��̾� �̸��� UI�� �Է¹޾Ƽ� "PlayerName" �̸� ���� Ű�� ���� 
        PlayerPrefs.SetInt("HighScore" ,currentScore);                  //���� ���ھ� ���� "HighScore" �̸� ���� Ű�� ���� 
        PlayerPrefs.Save();

        Debug.Log("���� �Ϸ�");
    }

    void LoadData()
    {
        string savedName = PlayerPrefs.GetString("PlayerName" , "PlayerName");  //PlayerName Ű���� �����͸� �����´�.
        int savedSocre = PlayerPrefs.GetInt("HighScore", 0);                     //HighScore Ű���� �����͸� �����´�.

        nameInput.text = savedName;                                         //�ؽ�Ʈ�� ����� �̸� ���� �����´�.
        currentScore = savedSocre;                                          //����� ������ ����� �����͸� �����´�.
        scoreText.text = "score " + currentScore;                           //UI ������Ʈ�� �Ѵ�. 

        Debug.Log("�ҷ����� �Ϸ�");

    }
}
