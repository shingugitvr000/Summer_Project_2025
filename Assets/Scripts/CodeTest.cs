using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodeTest : MonoBehaviour
{
    [Header("�̵� ����")]

    [Tooltip("�� ������ �ӵ� ���� ���� �Ѵ�.")]
    [SerializeField] private float speed;

    [SerializeField] private float speed_01;

    [Range(0, 100)]
    public int health;

    [Space(50)]
    [Header("������Ʈ ����")]
    public GameObject temp;



    // Start is called before the first frame update
    void Start()
    {
        FuntionTest_01();

        int myNumber = 10;
    
    }

    void FuntionTest_01()                   //���� �Ķ���ͳ� ���� ���� �ʿ� ������ 
    {
     
    }

    int FuntionTest_02(int num)
    {
        return num + 10;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
