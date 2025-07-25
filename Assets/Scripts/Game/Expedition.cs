using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Expedition : MonoBehaviour
{
    [Header("탐방 데이터")]
    public ExpeditionSO[] expeditions;                  //탐방 종류들

    [Header("탐방 UI")]
    public Button expeditionButton;                     //탐방 시작 버튼
    public Button[] memberButtons;                      //맴버 선택 버튼들
    public GameObject memberSelectPanel;                //맴버 선택 패널
    public Text expeditionInfoText;                     //선택된 탐방 정보
    public Text resultText;                             //결과 표시 텍스트 

    private SurvivalGameManager gameManager;
    private ExpeditionSO currentExpedition;             //현재 선택된 탐방

    public void Start()
    {
        gameManager = GetComponent<SurvivalGameManager>();

        memberSelectPanel.SetActive(false);
        resultText.text = "";
        expeditionInfoText.text = "";

        expeditionButton.onClick.AddListener(OpenMemberSelect);         //탐방 버튼 클릭 시 맴버 선택 패널 열기

        for (int i = 0; i < memberButtons.Length; i++)
        {
            int memberIndex = i;
            memberButtons[i].onClick.AddListener(()=> StartExpedition(memberIndex));
        }
    }


    void UpdateExpeditionInfo()                 //탐방 정보를 표시하는 함수 
    {
        if(currentExpedition != null)
        {
            expeditionInfoText.text = $"탐방 : {currentExpedition.expeditionName}\n" +
                                        $"{currentExpedition.description}" +
                                        $"기본 성공률 : {currentExpedition.baseSuccessRate}%";
        }
    }

    void UpdateMemberButtons()                  //맴버 버튼 업데이트 정보
    {
        for (int i = 0; i < memberButtons.Length && i < gameManager.groupMembers.Length; i++)
        {
            GroupMemberSO member = gameManager.groupMembers[i];
            bool canGo = gameManager.memberHealth[i] > 20;              //체력 20 이상일때 탐방 가능

            Text buttonText = memberButtons[i].GetComponentInChildren<Text>();
            buttonText.text = $"{member.memberName} \n 체력 : {gameManager.memberHealth[i]}";
            memberButtons[i].interactable = canGo;
        }
    }

    public void OpenMemberSelect()
    {
        //새로운 탐방 랜덤 선택
        if (expeditions.Length > 0)
        {
            currentExpedition = expeditions[Random.Range(0, expeditions.Length)];
            UpdateExpeditionInfo();
        }

        memberSelectPanel.SetActive(true);
        UpdateMemberButtons();  
    }

    public void StartExpedition(int memberIndex)
    {
        if (currentExpedition == null) return;

        memberSelectPanel.SetActive(false);

        GroupMemberSO member = gameManager.groupMembers[memberIndex];

        //성공률 계산 (ExpeditionSO의 기본 성고률 + 맴버 보너스]
        int memberBouns = 0;
        int finalSuccessRate = currentExpedition.baseSuccessRate + memberBouns;
        finalSuccessRate = Mathf.Clamp(finalSuccessRate, 5, 95);
        
        bool success = Random.Range(1,101) <= finalSuccessRate;

        if (success)
        {
            //성공 : ExpeditionSO의 보상 적용
            gameManager.food += currentExpedition.sucessFoodReward;
            gameManager.fuel += currentExpedition.successFuelReward;
            gameManager.medicine += currentExpedition.successMedicineReward;

            //탐방 완료 한 맴버 약간 피로
            gameManager.memberHunger[memberIndex] -= 5;

            resultText.text = $"{member.memberName} {currentExpedition.expeditionName} 성공 ! (성공률 : {finalSuccessRate}%)\n" +
                $"음식 + {currentExpedition.sucessFoodReward}, 연료 + {currentExpedition.successFuelReward}, " +
                $"의약품 +{currentExpedition.successMedicineReward}";

            resultText.color = Color.green;

        }
        else
        {
            //실패 : ExpeditionSO의 패널티 적용
            gameManager.memberHealth[memberIndex] += currentExpedition.failHealthPenalty;
            gameManager.memberHunger[memberIndex] += currentExpedition.failHungerPenalty;
            gameManager.memberBodyTemp[memberIndex] += currentExpedition.failTempPenalty;

            resultText.text = $"{member.memberName} {currentExpedition.expeditionName} 실패 ! (성공률 : {finalSuccessRate}%)\n" +
                $"체력 -{currentExpedition.failHealthPenalty}, 배고픔 -{currentExpedition.failHungerPenalty}, " +
                $"온도 -{currentExpedition.failTempPenalty}";

            resultText.color = Color.red;
        }

        //최소값 보정

        GroupMemberSO memberSO = gameManager.groupMembers[memberIndex];
        gameManager.memberHunger[memberIndex] = Mathf.Max(0, gameManager.memberHunger[memberIndex]);
        gameManager.memberBodyTemp[memberIndex] = Mathf.Max(25, gameManager.memberBodyTemp[memberIndex]);
        gameManager.memberHealth[memberIndex] = Mathf.Max(0, gameManager.memberHealth[memberIndex]);

        gameManager.UpdateUI();

        //3초후 결과 텍스트 사라지게
        Invoke("ClearResultText", 3f);
    }

    void ClearResultText()
    {
        resultText.text = "";
    }
}
