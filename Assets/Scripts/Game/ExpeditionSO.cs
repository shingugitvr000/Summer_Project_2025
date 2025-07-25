using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Expedition", menuName = "Survival Game/Expedition")]
public class ExpeditionSO : ScriptableObject
{
    [Header("Ž�� �⺻ ����")]
    public string expeditionName = "�� Ž��";
    [TextArea(2, 3)]
    public string description = "��ó ���� Ž�� �Ͽ� �ڿ��� ã���ϴ�.";

    [Header("���̵�")]
    [Range(1, 5)]
    public int difficulty = 2;              //1: ����, 5: �ſ� �����

    [Header("���� �� ����")]
    public int sucessFoodReward = 3;
    public int successFuelReward = 2;
    public int successMedicineReward = 1;

    [Header("���� �� ��� Ƽ")]
    public int failHealthPenalty = -20;
    public int failHungerPenalty = -10;
    public int failTempPenalty = -2;

    [Header("�⺻ ������")]
    [Range(10, 90)]
    public int baseSuccessRate = 60;                    //�⺻ ������ ���� 

}
