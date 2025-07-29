using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Expedition : MonoBehaviour
{
    [Header("Ž�� ������")]
    public ExpeditionSO[] expeditions;                  //Ž�� ������

    [Header("Ž�� UI")]
    public Button expeditionButton;                     //Ž�� ���� ��ư
    public Button[] memberButtons;                      //�ɹ� ���� ��ư��
    public GameObject memberSelectPanel;                //�ɹ� ���� �г�
    public Text expeditionInfoText;                     //���õ� Ž�� ����
    public Text resultText;                             //��� ǥ�� �ؽ�Ʈ 

    private SurvivalGameManager gameManager;
    private ExpeditionSO currentExpedition;             //���� ���õ� Ž��

    [Header("��� �ý���")]
    public EquipmentSO[] availableEquipments;           //��� ������ ��� �迭
    public Dropdown equipmentDropdown;                  //��Ӵٿ� UI

    public int selectedEquipmentIndex = 0;              //���õ� ��� Index
    public int[] equipmentDurability;                   //�� ����� ������

    public void Start()
    {
        gameManager = GetComponent<SurvivalGameManager>();

        memberSelectPanel.SetActive(false);
        resultText.text = "";
        expeditionInfoText.text = "";

        expeditionButton.onClick.AddListener(OpenMemberSelect);         //Ž�� ��ư Ŭ�� �� �ɹ� ���� �г� ����

        for (int i = 0; i < memberButtons.Length; i++)
        {
            int memberIndex = i;
            memberButtons[i].onClick.AddListener(()=> StartExpedition(memberIndex));
        }

        //������ �迭 �ʱ�ȭ 
        InititalizeEquipmentDurability();

        //��Ӵٿ� ���� �߰�
        SetupEquipmentDropdown();
        equipmentDropdown.onValueChanged.AddListener(OnEquipmentChanged);       //��� �ٿ� ������ ����ɶ� �Լ��� ȣ�� �Ѵ�. 
    }

    void OnEquipmentChanged(int equipmentIndex)
    {
        selectedEquipmentIndex = equipmentIndex;
        UpdateExpeditionInfo();
    }


    void UpdateExpeditionInfo()                 //Ž�� ������ ǥ���ϴ� �Լ� 
    {
        if(currentExpedition != null)
        {
            EquipmentSO selectedEquip = availableEquipments[selectedEquipmentIndex];

            //�η��� ���� ���ʽ� ����
            int equipBouse = (selectedEquipmentIndex > 0 && equipmentDurability[selectedEquipmentIndex] <= 0) ? 0 : selectedEquip.successBouns;
            int totalSuccessRate = currentExpedition.baseSuccessRate + equipBouse;

            string durabilityInfo = "";

            if(selectedEquipmentIndex > 0)
            {
                if (equipmentDurability[selectedEquipmentIndex] <= 0) durabilityInfo = "(�η��� ���� - ȿ�� ����)";
                else durabilityInfo = $"(������ : {equipmentDurability[selectedEquipmentIndex]}/{selectedEquip.maxDurability})";                
            }

            expeditionInfoText.text = $"Ž�� : {currentExpedition.expeditionName}\n" +
                                        $"{currentExpedition.description}\n" +
                                        $"�⺻ ������ : {currentExpedition.baseSuccessRate}%\n" +
                                        $"��� ���ʽ� : +{equipBouse}%\n {durabilityInfo}\n" +
                                        $"���� ������ : {totalSuccessRate}%";
        }
    }

    void UpdateMemberButtons()                  //�ɹ� ��ư ������Ʈ ����
    {
        for (int i = 0; i < memberButtons.Length && i < gameManager.groupMembers.Length; i++)
        {
            GroupMemberSO member = gameManager.groupMembers[i];
            bool canGo = gameManager.memberHealth[i] > 20;              //ü�� 20 �̻��϶� Ž�� ����

            Text buttonText = memberButtons[i].GetComponentInChildren<Text>();
            buttonText.text = $"{member.memberName} \n ü�� : {gameManager.memberHealth[i]}";
            memberButtons[i].interactable = canGo;
        }
    }

    public void OpenMemberSelect()
    {
        //���ο� Ž�� ���� ����
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

        //�η��� ���� ȿ�� ����
        bool equipmentBroken = selectedEquipmentIndex > 0 && equipmentDurability[selectedEquipmentIndex] <= 0;
        int equipBouus  = equipmentBroken ? 0 : selectedEquip.successBouns;
        int rewardBonus = equipmentBroken ? 0 : selectedEquip.rewardBonus;

        //������ ��� (ExpeditionSO�� �⺻ ������ + ��� ���ʽ�]
        int finalSuccessRate = currentExpedition.baseSuccessRate + equipBouus;
        finalSuccessRate = Mathf.Clamp(finalSuccessRate, 5, 95);
        
        bool success = Random.Range(1,101) <= finalSuccessRate;

        //��� ������ ���� (�Ǽ� ����, �η����� ���� ���)
        if (selectedEquipmentIndex > 0 && !equipmentBroken)
        {
            equipmentDurability[selectedEquipmentIndex] -= 1;       //������ 1 ���� 
            SetupEquipmentDropdown();                               //��Ӵٿ� ������Ʈ
        }

        if (success)
        {
            //���� : ExpeditionSO�� ���� ����
            gameManager.food += currentExpedition.sucessFoodReward + rewardBonus;
            gameManager.fuel += currentExpedition.successFuelReward + rewardBonus;
            gameManager.medicine += currentExpedition.successMedicineReward + rewardBonus;

            //Ž�� �Ϸ� �� �ɹ� �ణ �Ƿ�
            gameManager.memberHunger[memberIndex] -= 5;

            resultText.text = $"{member.memberName} {currentExpedition.expeditionName} ���� ! (������ : {finalSuccessRate}%)\n" +
                $"���� + {currentExpedition.sucessFoodReward + rewardBonus}, ���� + {currentExpedition.successFuelReward + rewardBonus}, " +
                $"�Ǿ�ǰ +{currentExpedition.successMedicineReward + rewardBonus}";

            resultText.color = Color.green;

        }
        else
        {
            //���� : ExpeditionSO�� �г�Ƽ ����
            gameManager.memberHealth[memberIndex] += currentExpedition.failHealthPenalty;
            gameManager.memberHunger[memberIndex] += currentExpedition.failHungerPenalty;
            gameManager.memberBodyTemp[memberIndex] += currentExpedition.failTempPenalty;

            resultText.text = $"{member.memberName} {currentExpedition.expeditionName} ���� ! (������ : {finalSuccessRate}%)\n" +
                $"ü�� -{currentExpedition.failHealthPenalty}, ����� -{currentExpedition.failHungerPenalty}, " +
                $"�µ� -{currentExpedition.failTempPenalty}";

            resultText.color = Color.red;
        }

        //�ּҰ� ����

        GroupMemberSO memberSO = gameManager.groupMembers[memberIndex];
        gameManager.memberHunger[memberIndex] = Mathf.Max(0, gameManager.memberHunger[memberIndex]);
        gameManager.memberBodyTemp[memberIndex] = Mathf.Max(25, gameManager.memberBodyTemp[memberIndex]);
        gameManager.memberHealth[memberIndex] = Mathf.Max(0, gameManager.memberHealth[memberIndex]);

        gameManager.UpdateUI();

        //3���� ��� �ؽ�Ʈ �������
        Invoke("ClearResultText", 3f);
    }

    void ClearResultText()
    {
        resultText.text = "";
    }

    void InititalizeEquipmentDurability()               //��� ������ ���� �ϴ� �Լ�
    {
        equipmentDurability = new int [availableEquipments.Length];         //��� ���� ��ŭ �迭 ���� (���� ����)
       
        for(int i = 0; i < availableEquipments.Length; i++)
        {
            equipmentDurability[i] = availableEquipments[i].maxDurability;      //��밡���� �������� �迭�� �־��ش�. 
        }
    }

    void SetupEquipmentDropdown()                                   //��Ӵٿ� �޴� ����
    {
        equipmentDropdown.options.Clear();                          //�ɼ��� �ʱ�ȭ �����ش�. 

        //��� �ɼǵ��� ��Ӵٿ �߰� (������ ����)
        for (int i = 0; i < availableEquipments.Length; i++)
        {
            string equipName = availableEquipments[i].equipmentName;        //�̸��� �����´�.

            //�������� 0�̸� (�η���) ǥ�� , �Ǽ� (�ε��� 0)�� ���� 
            if(i == 0)
            {
                equipmentDropdown.options.Add(new Dropdown.OptionData(equipName));      //�Ǽ��� �׳� �Ǽո� ǥ��
            }
            else if (equipmentDurability[i] <= 0)
            {
                equipmentDropdown.options.Add(new Dropdown.OptionData($"{equipName} (�η���)"));      //�Ǽ��� �׳� �Ǽո� ǥ��
            }
            else
            {
                equipmentDropdown.options.Add(new Dropdown.OptionData($"{equipName} ({equipmentDurability[i]} / {availableEquipments[i].maxDurability})"));
            }
        }

        equipmentDropdown.value = 0;                    //ù ��° ���(�Ǽ�) ����
        equipmentDropdown.RefreshShownValue();          //������ ������ ������� �������� ���� �Լ��� ���� �����Ѵ�. 
    }
}
