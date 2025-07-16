using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SurvivalGameManager : MonoBehaviour
{
    [Header("그룹 구성원 템플릿")]
    public GroupMemberSO[] groupMembers;

    [Header("참조 UI")]
    public Text dayText;                            //날짜 표시 UI
    public Text[] memberStatusTexts;                //맴버 상태 표시 Text
    public Button nextDayButton;                    //다음 날짜로 변경되는 버튼

    int currentDay;                                 //현재 날짜

    //런타임 데이터
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
        int memberCount = groupMembers.Length;              //그룹 맴버의 길이 만큼 인원 수 할당
        memberHealth = new int[memberCount];                //그룹 맴버 길이 만큼 배열 할당
        memberHunger = new int[memberCount];
        memberBodyTemp = new int[memberCount];      

        for(int i = 0; i < memberCount; i++) 
        {
            if (groupMembers[i] != null)                        //그룹 맴버에서 정의한 숫자를 배열에 넣는다. 
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

                //상태 메세지 결정
                string status = GetMemberStatus(i);

                memberStatusTexts[i].text =
                    $"{member.memberName} {status} \n" +
                    $"체력   : {memberHealth[i]} \n" +
                    $"배고픔 : {memberHunger[i]} \n" +
                    $"체온   : {memberBodyTemp[i]} 도 ";
            }


            UpdateTextColor(memberStatusTexts[i], memberHealth[i]);
        }

    }

    void ProcessDailyChange()                                   //하루가 지날때 스텟 변화 설정 함수 
    {
        int baseHungerLoss = 15;                                //배고픔은 15 감소
        int baseTempLoss = 1;                                   //1도씩 감소

        for(int i = 0; i < groupMembers.Length;i++)
        {
            if (groupMembers[i] == null) continue;                      //그룹중 1명이 사망해도 계속 진행

            GroupMemberSO member = groupMembers[i];

            //나이에 따른 배고픔 조정
            float hungerMultiplier = member.ageGroup == GroupMemberSO.AgeGroup.Child ? 0.8f : 1.0f;

            //상태 감소
            memberHunger[i] -= Mathf.RoundToInt(baseHungerLoss * hungerMultiplier);             //맴버별 배고픔 저항 설정
            memberBodyTemp[i] -= Mathf.RoundToInt(baseTempLoss * member.coldResistance);       //맴버별 추위 저항력

            //건강 체크
            if (memberHunger[i] <= 0) memberHunger[i] -= 15;                        //굶주림
            if (memberBodyTemp[i] <= 32) memberHealth[i] -= 10;                     //저체온증 (32도 이하)
            if (memberBodyTemp[i] <= 30) memberHealth[i] -= 20;                     //심각한 저체온증 

            //최소값 제한
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
        //사망 체크
        if (memberHealth[memberIndex] <= 0)
            return "(사망)";

        //가장 위험한 상태부터 체크
        if (memberBodyTemp[memberIndex] <= 30) return "(심각한 저체온증)";
        else if (memberHealth[memberIndex] <= 20) return "(위험)";
        else if (memberHunger[memberIndex] <= 10) return "(굶주림)";
        else if (memberBodyTemp[memberIndex] <= 32) return "(저체온증)";
        else if (memberHealth[memberIndex] <= 50) return "(약함)";
        else if (memberHunger[memberIndex] <= 30) return "(배고픔)";
        else if (memberBodyTemp[memberIndex] <= 35) return "(추위)";
        else return "(건강)";

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
            Debug.Log("게임 오버! 모드은 구성원이 혹독한 상황을 이겨내지 못했습니다.");
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
