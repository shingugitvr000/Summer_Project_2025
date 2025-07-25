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
    }


    void UpdateExpeditionInfo()                 //Ž�� ������ ǥ���ϴ� �Լ� 
    {
        if(currentExpedition != null)
        {
            expeditionInfoText.text = $"Ž�� : {currentExpedition.expeditionName}\n" +
                                        $"{currentExpedition.description}" +
                                        $"�⺻ ������ : {currentExpedition.baseSuccessRate}%";
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

        //������ ��� (ExpeditionSO�� �⺻ ����� + �ɹ� ���ʽ�]
        int memberBouns = 0;
        int finalSuccessRate = currentExpedition.baseSuccessRate + memberBouns;
        finalSuccessRate = Mathf.Clamp(finalSuccessRate, 5, 95);
        
        bool success = Random.Range(1,101) <= finalSuccessRate;

        if (success)
        {
            //���� : ExpeditionSO�� ���� ����
            gameManager.food += currentExpedition.sucessFoodReward;
            gameManager.fuel += currentExpedition.successFuelReward;
            gameManager.medicine += currentExpedition.successMedicineReward;

            //Ž�� �Ϸ� �� �ɹ� �ణ �Ƿ�
            gameManager.memberHunger[memberIndex] -= 5;

            resultText.text = $"{member.memberName} {currentExpedition.expeditionName} ���� ! (������ : {finalSuccessRate}%)\n" +
                $"���� + {currentExpedition.sucessFoodReward}, ���� + {currentExpedition.successFuelReward}, " +
                $"�Ǿ�ǰ +{currentExpedition.successMedicineReward}";

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
}
