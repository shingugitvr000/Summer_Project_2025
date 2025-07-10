using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class JSONSaveExample : MonoBehaviour
{
    [Header("UI")]
    public InputField nameInput;                        //�̸� �Է� UI
    public Text levelText;                              //���� �ؽ�Ʈ
    public Text goldText;                               //�� �ؽ�Ʈ
    public Text playTimeText;                           //�÷��� �ð� �ؽ�Ʈ
    public Button saveButton;                           //���̺� ��ư
    public Button loadButton;                           //�ε� ��ư 

    PlayerData playerData;                              //�÷��̾� ������ Ŭ���� ����
    string saveFilePath;                                //���� ��� Ȯ�ο�

    // Start is called before the first frame update
    void Start()
    {
        //���� ���� ��� ����
        saveFilePath = Path.Combine(Application.persistentDataPath, "playerData.json");

        //������ �ʱ� 
        playerData = new PlayerData();          // new Ű����� ������ �ϴ°��̴�. 
        playerData.playerName = "���ο� �÷��̾�";
        playerData.level = 1;
        playerData.gold = 100;
        playerData.playtime = 0f;
        playerData.position = Vector3.zero;

        saveButton.onClick.AddListener(SaveToJson);
        loadButton.onClick.AddListener(LoadFromJSON);

        //�ڵ� �ε�
        LoadFromJSON();
        UpdateUI();

        Debug.Log(saveFilePath);                //���� ��� ǥ��

    }

    // Update is called once per frame
    void Update()
    {
        playerData.playtime += Time.deltaTime;          //�÷���Ÿ�� ���� 

        if (Input.GetKeyDown(KeyCode.L))                //������
        {
            playerData.level++;
            playerData.gold += 50;
        }

        if (Input.GetKeyDown(KeyCode.G))                //��� ȹ��
        {           
            playerData.gold += 10;
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        nameInput.text = playerData.playerName;
        levelText.text = "Lv : " + playerData.level;
        goldText.text = "Gold : " + playerData.gold;
        playTimeText.text = "PlayTime : " + playerData.playtime;
    }

    void SaveToJson()
    {
        playerData.playerName = nameInput.text;     // UI ���� �����Ϳ� ����

        string jsonData = JsonUtility.ToJson(playerData, true);  //JSON ���� ��ȯ

        File.WriteAllText(saveFilePath, jsonData);              //���Ͽ� ����

        Debug.Log("���� �Ϸ�");
    }

    void LoadFromJSON()
    {
        if (File.Exists(saveFilePath))                  //������ �����ϴ��� Ȯ��
        {
            string jsonData = File.ReadAllText(saveFilePath);       //JSON ���� �б�

            playerData = JsonUtility.FromJson<PlayerData>(jsonData);    //JSON ��ü�� ��ȯ

            Debug.Log("�ҷ����� �Ϸ�");
        }
        else
        {
            Debug.Log("���� ������ �����ϴ�. ");
        }

        UpdateUI();
    }
}
