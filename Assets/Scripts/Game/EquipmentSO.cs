using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Survival Game/EquipmentSO")]
public class EquipmentSO : ScriptableObject
{
    [Header("��� ����")]
    public string equipmentName = "�Ǽ�";

    [Header("Ž�� ���ʽ�")]
    [Range(0, 30)]
    public int successBouns = 0;                        //������ ���ʽ�
    [Range(0, 3)]
    public int rewardBonus = 0;                         //���� ����

    [Header("������")]
    [Range(1, 10)]
    public int maxDurability = 1;                       //�ִ� ������ (�� �� ��� ����)

    [Header("����")]
    public string description = "�⺻ ����";

}
