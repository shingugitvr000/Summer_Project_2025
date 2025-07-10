using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class JSONSaveExample : MonoBehaviour
{
    [Header("UI")]
    public InputField nameInput;                        //이름 입력 UI
    public Text levelText;                              //레벨 텍스트
    public Text goldText;                               //돈 텍스트
    public Text playTimeText;                           //플레이 시간 텍스트
    public Button saveButton;                           //세이브 버튼
    public Button loadButton;                           //로드 버튼 

    PlayerData playerData;                              //플레이어 데이터 클래스 선언
    string saveFilePath;                                //저장 경로 확인용

    // Start is called before the first frame update
    void Start()
    {
        //저장 파일 경로 설정
        saveFilePath = Path.Combine(Application.persistentDataPath, "playerData.json");

        //데이터 초기 
        playerData = new PlayerData();          // new 키워드는 생성을 하는것이다. 
        playerData.playerName = "새로운 플레이어";
        playerData.level = 1;
        playerData.gold = 100;
        playerData.playtime = 0f;
        playerData.position = Vector3.zero;

        saveButton.onClick.AddListener(SaveToJson);
        loadButton.onClick.AddListener(LoadFromJSON);

        //자동 로드
        LoadFromJSON();
        UpdateUI();

        Debug.Log(saveFilePath);                //저장 경로 표시

    }

    // Update is called once per frame
    void Update()
    {
        playerData.playtime += Time.deltaTime;          //플레이타임 증가 

        if (Input.GetKeyDown(KeyCode.L))                //레벨업
        {
            playerData.level++;
            playerData.gold += 50;
        }

        if (Input.GetKeyDown(KeyCode.G))                //골드 획득
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
        playerData.playerName = nameInput.text;     // UI 값을 데이터에 저장

        string jsonData = JsonUtility.ToJson(playerData, true);  //JSON 으로 변환

        File.WriteAllText(saveFilePath, jsonData);              //파일에 저장

        Debug.Log("저장 완료");
    }

    void LoadFromJSON()
    {
        if (File.Exists(saveFilePath))                  //파일이 존재하는지 확인
        {
            string jsonData = File.ReadAllText(saveFilePath);       //JSON 파일 읽기

            playerData = JsonUtility.FromJson<PlayerData>(jsonData);    //JSON 객체로 변환

            Debug.Log("불러오기 완료");
        }
        else
        {
            Debug.Log("저장 파일이 없습니다. ");
        }

        UpdateUI();
    }
}
