using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Survival Game/Item")]
public class ItemSO : ScriptableObject
{
    [Header("�⺻ ����")]
    public string itemName = "������";
    public Sprite itemIcon;
    public ItemType itemType;

    [Header("ȿ��")]
    public int healthEffect = 0;                //ü�� ȸ����
    public int hungerEffect = 0;                //����� ȸ����
    public int tempEffect = 0;                  //ü�� ȸ����

    [Header("����")]
    [TextArea(2, 3)]
    public string description = "������ ������ �Դϴ�. ";

    public enum ItemType
    {
        Food,                   //���� (����� ȸ��)
        Fuel,                   //���� (ü�� ȸ��)
        Meidicine               //�Ǿ�ǰ (ü�� ȸ��)
    }
}
