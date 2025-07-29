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

    [Header("장비 시스템")]
    public EquipmentSO[] availableEquipments;           //사용 가능한 장비 배열
    public Dropdown equipmentDropdown;                  //드롭다운 UI

    public int selectedEquipmentIndex = 0;              //선택된 장비 Index
    public int[] equipmentDurability;                   //각 장비의 내구도

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

        //내구도 배열 초기화 
        InititalizeEquipmentDurability();

        //드롭다운 설정 추가
        SetupEquipmentDropdown();
        equipmentDropdown.onValueChanged.AddListener(OnEquipmentChanged);       //드롭 다운 선택이 변경될때 함수를 호출 한다. 
    }

    void OnEquipmentChanged(int equipmentIndex)
    {
        selectedEquipmentIndex = equipmentIndex;
        UpdateExpeditionInfo();
    }


    void UpdateExpeditionInfo()                 //탐방 정보를 표시하는 함수 
    {
        if(currentExpedition != null)
        {
            EquipmentSO selectedEquip = availableEquipments[selectedEquipmentIndex];

            //부러진 장비는 보너스 없음
            int equipBouse = (selectedEquipmentIndex > 0 && equipmentDurability[selectedEquipmentIndex] <= 0) ? 0 : selectedEquip.successBouns;
            int totalSuccessRate = currentExpedition.baseSuccessRate + equipBouse;

            string durabilityInfo = "";

            if(selectedEquipmentIndex > 0)
            {
                if (equipmentDurability[selectedEquipmentIndex] <= 0) durabilityInfo = "(부러진 상태 - 효과 없음)";
                else durabilityInfo = $"(내구도 : {equipmentDurability[selectedEquipmentIndex]}/{selectedEquip.maxDurability})";                
            }

            expeditionInfoText.text = $"탐방 : {currentExpedition.expeditionName}\n" +
                                        $"{currentExpedition.description}\n" +
                                        $"기본 성공률 : {currentExpedition.baseSuccessRate}%\n" +
                                        $"장비 보너스 : +{equipBouse}%\n {durabilityInfo}\n" +
                                        $"최종 성공률 : {totalSuccessRate}%";
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
        EquipmentSO selectedEquip = availableEquipments[selectedEquipmentIndex];

        //부러진 장비는 효과 없음
        bool equipmentBroken = selectedEquipmentIndex > 0 && equipmentDurability[selectedEquipmentIndex] <= 0;
        int equipBouus  = equipmentBroken ? 0 : selectedEquip.successBouns;
        int rewardBonus = equipmentBroken ? 0 : selectedEquip.rewardBonus;

        //성공률 계산 (ExpeditionSO의 기본 성공률 + 장비 보너스]
        int finalSuccessRate = currentExpedition.baseSuccessRate + equipBouus;
        finalSuccessRate = Mathf.Clamp(finalSuccessRate, 5, 95);
        
        bool success = Random.Range(1,101) <= finalSuccessRate;

        //장비 내구도 감소 (맨손 제외, 부러지지 않은 장비만)
        if (selectedEquipmentIndex > 0 && !equipmentBroken)
        {
            equipmentDurability[selectedEquipmentIndex] -= 1;       //내구도 1 감소 
            SetupEquipmentDropdown();                               //드롭다운 업데이트
        }

        if (success)
        {
            //성공 : ExpeditionSO의 보상 적용
            gameManager.food += currentExpedition.sucessFoodReward + rewardBonus;
            gameManager.fuel += currentExpedition.successFuelReward + rewardBonus;
            gameManager.medicine += currentExpedition.successMedicineReward + rewardBonus;

            //탐방 완료 한 맴버 약간 피로
            gameManager.memberHunger[memberIndex] -= 5;

            resultText.text = $"{member.memberName} {currentExpedition.expeditionName} 성공 ! (성공률 : {finalSuccessRate}%)\n" +
                $"음식 + {currentExpedition.sucessFoodReward + rewardBonus}, 연료 + {currentExpedition.successFuelReward + rewardBonus}, " +
                $"의약품 +{currentExpedition.successMedicineReward + rewardBonus}";

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

    void InititalizeEquipmentDurability()               //장비 내구도 셋팅 하는 함수
    {
        equipmentDurability = new int [availableEquipments.Length];         //장비 숫자 만큼 배열 선언 (동적 선언)
       
        for(int i = 0; i < availableEquipments.Length; i++)
        {
            equipmentDurability[i] = availableEquipments[i].maxDurability;      //사용가능한 내구도를 배열에 넣어준다. 
        }
    }

    void SetupEquipmentDropdown()                                   //드롭다운 메뉴 설정
    {
        equipmentDropdown.options.Clear();                          //옵션을 초기화 시켜준다. 

        //장비 옵션들을 드롭다운에 추가 (내구도 포함)
        for (int i = 0; i < availableEquipments.Length; i++)
        {
            string equipName = availableEquipments[i].equipmentName;        //이름을 가져온다.

            //내구도가 0이면 (부러진) 표시 , 맨손 (인덱스 0)은 제외 
            if(i == 0)
            {
                equipmentDropdown.options.Add(new Dropdown.OptionData(equipName));      //맨손은 그냥 맨손만 표시
            }
            else if (equipmentDurability[i] <= 0)
            {
                equipmentDropdown.options.Add(new Dropdown.OptionData($"{equipName} (부러진)"));      //맨손은 그냥 맨손만 표시
            }
            else
            {
                equipmentDropdown.options.Add(new Dropdown.OptionData($"{equipName} ({equipmentDurability[i]} / {availableEquipments[i].maxDurability})"));
            }
        }

        equipmentDropdown.value = 0;                    //첫 번째 장비(맨손) 선택
        equipmentDropdown.RefreshShownValue();          //데이터 변경이 있을경우 보여지는 값을 함수로 통해 리셋한다. 
    }
}
