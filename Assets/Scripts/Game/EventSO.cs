using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Event", menuName = "Survival Game/Event")]
public class EventSO : ScriptableObject
{
    [Header("�̺�Ʈ ����")]
    public string eventTitle = "�̺�Ʈ �߻�!";
    [TextArea(3, 5)]
    public string eventDescription = "���� ���� �Ͼ���ϴ�.";

    [Header("�ڿ� ��ȭ")]
    public int foodChange = 0;
    public int fuelChange = 0;
    public int medicineChange = 0;

    [Header("��� ���� ��ȭ")]
    public int healthChange = 0;
    public int hungerChange = 0;
    public int tempChange = 0;

    [Header("�߻� Ȯ��")]
    [Range(1, 100)]
    public int probability = 30;
}
