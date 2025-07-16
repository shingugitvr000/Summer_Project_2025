using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New GroupMember" , menuName = "Survival Game/Group Member")]
public class GroupMemberSO : ScriptableObject
{
    [Header("�⺻ ����")]
    public string memberName = "������";
    public Sprite protrait;                         //�ʻ�ȭ
    public Gender gender = Gender.Male;
    public AgeGroup ageGroup = AgeGroup.Audlt;

    [Header("�⺻ ����")]
    [Range(50, 100)]
    public int maxHealth = 100;                 //ü��
    [Range(50, 100)]
    public int maxHunger = 100;                 //�����
    [Range(36, 38)]
    public int normalBodyTemp = 37;                 //���� 37��(���� ü��)

    [Header("Ư��")]
    [Range(0.5f, 2.0f)]
    public float coldResistance = 1.0f;                 //���� ���׷�
    [Range(0.5f, 2.0f)]
    public float foodEfficiency = 1.0f;                 //���� ȿ��
    [Range(0.8f, 1.5f)]
    public float recoveryRate = 1.0f;                   //ȸ����

    [Header("����")]
    [TextArea(2, 3)]
    public string description = "�׷� ������ �Դϴ�.";

    public enum Gender
    {
        Male,                   //����
        Female                  //����
    }
    public enum AgeGroup
    {
        Child,                  //����
        Audlt,                  //�
        Elder                   //����
    }
}
