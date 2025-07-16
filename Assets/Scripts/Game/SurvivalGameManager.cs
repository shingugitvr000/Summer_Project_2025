using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SurvivalGameManager : MonoBehaviour
{
    [Header("�׷� ������ ���ø�")]
    public GroupMemberSO[] groupMembers;

    [Header("���� UI")]
    public Text dayText;                            //��¥ ǥ�� UI
    public Text[] memberStatusTexts;                //�ɹ� ���� ǥ�� Text
    public Button nextDayButton;                    //���� ��¥�� ����Ǵ� ��ư

    int currentDay;                                 //���� ��¥

    //��Ÿ�� ������
    private int[] memberHealth;
    private int[] memberHunger;
    private int[] memberBodyTemp;
   
    void Start()
    {

        currentDay = 1;

        InitializeGroup();
        UpdateUI();
        
        nextDayButton.onClick.AddListener(NextDay);
    }

    void InitializeGroup()
    {
        int memberCount = groupMembers.Length;              //�׷� �ɹ��� ���� ��ŭ �ο� �� �Ҵ�
        memberHealth = new int[memberCount];                //�׷� �ɹ� ���� ��ŭ �迭 �Ҵ�
        memberHunger = new int[memberCount];
        memberBodyTemp = new int[memberCount];      

        for(int i = 0; i < memberCount; i++) 
        {
            if (groupMembers[i] != null)                        //�׷� �ɹ����� ������ ���ڸ� �迭�� �ִ´�. 
            {
                memberHealth[i] = groupMembers[i].maxHealth;                
                memberHunger[i] = groupMembers[i].maxHunger;
                memberBodyTemp[i] = groupMembers[i].normalBodyTemp;
            }
        }
    }

    void UpdateUI()
    {
        dayText.text = $"Day {currentDay}";

        for (int i = 0; i < groupMembers.Length; i++)
        {
            if (groupMembers[i] != null && memberStatusTexts[i] != null)
            {
                GroupMemberSO member = groupMembers[i];

                //���� �޼��� ����
                string status = GetMemberStatus(i);

                memberStatusTexts[i].text =
                    $"{member.memberName} {status} \n" +
                    $"ü��   : {memberHealth[i]} \n" +
                    $"����� : {memberHunger[i]} \n" +
                    $"ü��   : {memberBodyTemp[i]} �� ";
            }


            UpdateTextColor(memberStatusTexts[i], memberHealth[i]);
        }

    }

    void ProcessDailyChange()                                   //�Ϸ簡 ������ ���� ��ȭ ���� �Լ� 
    {
        int baseHungerLoss = 15;                                //������� 15 ����
        int baseTempLoss = 1;                                   //1���� ����

        for(int i = 0; i < groupMembers.Length;i++)
        {
            if (groupMembers[i] == null) continue;                      //�׷��� 1���� ����ص� ��� ����

            GroupMemberSO member = groupMembers[i];

            //���̿� ���� ����� ����
            float hungerMultiplier = member.ageGroup == GroupMemberSO.AgeGroup.Child ? 0.8f : 1.0f;

            //���� ����
            memberHunger[i] -= Mathf.RoundToInt(baseHungerLoss * hungerMultiplier);             //�ɹ��� ����� ���� ����
            memberBodyTemp[i] -= Mathf.RoundToInt(baseTempLoss * member.coldResistance);       //�ɹ��� ���� ���׷�

            //�ǰ� üũ
            if (memberHunger[i] <= 0) memberHunger[i] -= 15;                        //���ָ�
            if (memberBodyTemp[i] <= 32) memberHealth[i] -= 10;                     //��ü���� (32�� ����)
            if (memberBodyTemp[i] <= 30) memberHealth[i] -= 20;                     //�ɰ��� ��ü���� 

            //�ּҰ� ����
            memberHunger[i] = Mathf.Max(0, memberHunger[i]);
            memberBodyTemp[i] = Mathf.Max(25, memberBodyTemp[i]);
            memberHealth[i] = Mathf.Max(0, memberHealth[i]);
        }
    }

    public void NextDay()
    {
        currentDay += 1;
        ProcessDailyChange();
        UpdateUI();
        CheckGameOver();
    }

    string GetMemberStatus(int memberIndex)
    {
        //��� üũ
        if (memberHealth[memberIndex] <= 0)
            return "(���)";

        //���� ������ ���º��� üũ
        if (memberBodyTemp[memberIndex] <= 30) return "(�ɰ��� ��ü����)";
        else if (memberHealth[memberIndex] <= 20) return "(����)";
        else if (memberHunger[memberIndex] <= 10) return "(���ָ�)";
        else if (memberBodyTemp[memberIndex] <= 32) return "(��ü����)";
        else if (memberHealth[memberIndex] <= 50) return "(����)";
        else if (memberHunger[memberIndex] <= 30) return "(�����)";
        else if (memberBodyTemp[memberIndex] <= 35) return "(����)";
        else return "(�ǰ�)";

    }

    void CheckGameOver()
    {
        int aliveCount = 0;

        for (int i = 0; i < memberHealth.Length; i++)
        {
            if (memberHealth[i] > 0) aliveCount++;
        }

        if (aliveCount == 0)
        {
            nextDayButton.interactable = false;
            Debug.Log("���� ����! ����� �������� Ȥ���� ��Ȳ�� �̰ܳ��� ���߽��ϴ�.");
        }
    }

    void UpdateTextColor(Text text, int health)
    {
        if (health <= 0)
            text.color = Color.gray;
        else if(health <= 20)
            text.color = Color.red;
        else if (health <= 50)
            text.color = Color.yellow;
        else 
            text.color = Color.white;
    }

   
    void Update()
    {
        
    }
}
