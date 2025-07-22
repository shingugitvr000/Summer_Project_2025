using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SurvivalGameManager : MonoBehaviour
{
    [Header("�׷� ������ ���ø�")]
    public GroupMemberSO[] groupMembers;

    [Header("������ ���ø�")]
    public ItemSO foodItem;                         //���� ������ SO
    public ItemSO fuelItem;                         //���� ������ SO
    public ItemSO medicineItem;                     //�Ǿ�ǰ ������ SO

    [Header("���� UI")]
    public Text dayText;                            //��¥ ǥ�� UI
    public Text[] memberStatusTexts;                //�ɹ� ���� ǥ�� Text
    public Button nextDayButton;                    //���� ��¥�� ����Ǵ� ��ư
    public Text inventoryText;                      //�κ��丮 ǥ��

    [Header("������ ��ư")]
    public Button feedButton;                       //���� �ֱ�
    public Button heatButton;                       //���� �ϱ�
    public Button healButton;                       //ġ�� �ϱ� 

    [Header("���� ����")]
    int currentDay;                                 //���� ��¥
    public int food = 5;                            //���� ����
    public int fuel = 3;                            //���� ����
    public int medicine = 4;                        //�Ǿ�ǰ ���� 

    [Header("Ư�� �ɹ� ������ �Ҹ� ��ư")]
    public Button[] individualFoodButtons;              //������ ���� ���� ��ư��
    public Button[] individualHealButtons;              //������ ���� ġ�� ��ư��

    [Header("�̺�Ʈ �ý���")]
    public EventSO[] events;                            //�̺�Ʈ ���
    public GameObject eventPopup;                       //�̺�Ʈ �˾� �г�
    public Text eventTitleText;                         //�̺�Ʈ ����
    public Text eventDescriptionText;                   //�̺�Ʈ ����
    public Button eventConfirmButton;                   //�̺�Ʈ �ݱ�(Ȯ��) ��ư 

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
        feedButton.onClick.AddListener(UseFoodItem);
        heatButton.onClick.AddListener(UseFuelItem);
        healButton.onClick.AddListener(UseMedicineItem);

        for (int i = 0; i < individualFoodButtons.Length && i < groupMembers.Length; i++)
        {
            int memberIndex = i;            //Ŭ���� ���� �ذ� (�׳� i�� �־������ 4,4,4,4) �̷��� ��
            individualFoodButtons[i].onClick.AddListener(() => GiveFoodToMember(memberIndex));
        }

        eventPopup.SetActive(false);
        eventConfirmButton.onClick.AddListener(CloseEventPopup);

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

        inventoryText.text = $"����   : {food} �� \n" +
                             $"����   : {fuel} �� \n" +
                             $"�Ǿ�ǰ : {medicine} �� \n";

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
        CheckRandomEvent();                                     //�̺�Ʈ üũ
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

    public void UseFoodItem()                                                   //���� ������ ���
    {
        if (food <= 0 || foodItem == null) return;                              //���� ���� ó�� 

        food--;
        UseItemOnAllMembers(foodItem);
        UpdateUI();
    }

    public void UseFuelItem()                                                   //���� ������ ���
    {
        if (fuel <= 0 || fuelItem == null) return;                              //���� ���� ó�� 

        fuel--;
        UseItemOnAllMembers(fuelItem);
        UpdateUI();
    }

    public void UseMedicineItem()                                                   //�Ǿ�ǰ ������ ���
    {
        if(medicine <= 0 || medicineItem == null) return;                              //���� ���� ó�� 

        medicine--;
        UseItemOnAllMembers(medicineItem);
        UpdateUI();
    }

    void UseItemOnAllMembers(ItemSO item)
    {
        for (int i = 0; i < groupMembers.Length; i++)
        {
            if (groupMembers[i] != null && memberHealth[i] > 0)                 //����ִ� ������
            {
                ApplyItemEffect(i, item);
            }

        }
    }

    //Ư�� �������Ը� ���� �ֱ� 
    public void GiveFoodToMember(int memberIndex)
    {
        if (food <= 0 || foodItem == null) return;
        if (memberHealth[memberIndex] <= 0) return;

        food--;
        ApplyItemEffect(memberIndex, foodItem);
        UpdateUI();
    }
    //Ư�� ������ ġ���ϱ� 
    public void HealMember(int memberIndex)
    {
        if (medicine <= 0 || medicineItem == null) return;
        if (memberHealth[memberIndex] <= 0) return;

        medicine--;
        ApplyItemEffect(memberIndex, medicineItem);
        UpdateUI();
    }

    //��� �������� ���� ��ġ �Լ�
    void ApplyItemEffect(int memberIndex, ItemSO item)
    {
        GroupMemberSO member = groupMembers[memberIndex];

        //���� Ư�� �����ؼ� ������ ȿ�� ��� 
        int actualHealth = Mathf.RoundToInt(item.healthEffect * member.recoveryRate);
        int actualHunger = Mathf.RoundToInt(item.hungerEffect * member.foodEfficiency);
        int actaulTemp = item.tempEffect;

        //ȿ�� ����
        memberHealth[memberIndex] += actualHealth;
        memberHunger[memberIndex] += actualHunger;
        memberBodyTemp[memberIndex] += actaulTemp;

        //�ִ�ġ ����
        memberHealth[memberIndex] = Mathf.Min(memberHealth[memberIndex], member.maxHealth);
        memberHunger[memberIndex] = Mathf.Min(memberHunger[memberIndex], member.maxHunger);
        memberBodyTemp[memberIndex] = Mathf.Min(memberBodyTemp[memberIndex], member.normalBodyTemp);
    }

    //�̺�Ʈ�� ���� ���� ��ġ �Լ�
    void ApplyEventEffects(EventSO eventSO)
    {
        //�ڿ� ��ȭ
        food += eventSO.foodChange;
        fuel += eventSO.fuelChange;
        medicine += eventSO.medicineChange;

        //�ڿ� �ּҰ� ����
        food = Mathf.Max(0, food);
        fuel = Mathf.Max(0, fuel);
        medicine = Mathf.Max(0, medicine);

        //��� ����ִ� ������� ���� ��ȭ ����
        for(int i = 0; i < groupMembers.Length; i++)
        {
            if (groupMembers[i] != null && memberHealth[i]> 0)
            {
                memberHealth[i] += eventSO.healthChange;
                memberHunger[i] += eventSO.hungerChange;
                memberBodyTemp[i] += eventSO.tempChange;

                //���� �� ����
                GroupMemberSO member = groupMembers[i];
                memberHealth[i] = Mathf.Clamp(memberHealth[i], 0, member.maxHealth);
                memberHunger[i] = Mathf.Clamp(memberHunger[i], 0, member.maxHunger);
                memberBodyTemp[i] = Mathf.Clamp(memberBodyTemp[i], 0, member.normalBodyTemp);
            }
        }
    }

    void ShowEventPopup(EventSO eventSO)
    {
        //�˾� Ȱ��ȭ 
        eventPopup.SetActive(true);

        //�ؽ�Ʈ ����
        eventTitleText.text = eventSO.eventTitle;
        eventDescriptionText.text = eventSO.eventDescription;

        //�̺�Ʈ ȿ�� ����
        ApplyEventEffects(eventSO);

        //���� ���� �Ͻ�����
        nextDayButton.interactable = false;
    }

    public void CloseEventPopup()
    {
        eventPopup.SetActive(false);
        nextDayButton.interactable = true;
        UpdateUI();
    }

    void CheckRandomEvent()
    {
        int totalProbability = 0;

        //��ü Ȯ�� �� ���ϱ� 
        for(int i = 0; i < events.Length; i++)
        {
            totalProbability += events[i].probability; 
        }

        if (totalProbability == 0)
            return;                     //��� �̺�Ʈ Ȯ���� 0�̸� �̺�Ʈ ����

        int roll = Random.Range(1, totalProbability + 1 + 50);          //��ü Ȯ�� ���ϱ⿡ �ƹ��͵� ���� Ȯ�� 50����ġ
        int cumualtive = 0;

        for(int i = 0; i < events.Length;i++)
        {
            cumualtive += events[i].probability;
            if(roll <= cumualtive)
            {
                ShowEventPopup(events[i]);  
                return;
            }
        }
    }

}
