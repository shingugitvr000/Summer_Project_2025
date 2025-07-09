using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int health = 100;          //ü�� ���� ����
        
    void Start()
    {
        GetComponent<Renderer>().material.color = Color.green;              //���� �ʷϻ����� �����. 
    }

    public void TakeDamage(int damage)                      //������ �޴� �Լ� ����
    {
        health -= damage;
        StartCoroutine(DamageEffect());                     //������ ���� ����

        if(health <= 0)                                     //�״� ������ ���� ü�� �˻� 
        {   
            StartCoroutine(Die());                          //�״� ���� ����
        }
    }

    IEnumerator DamageEffect()
    {
        GetComponent<Renderer>().material.color = Color.red;                //���� ���������� �����.
        yield return new WaitForSeconds(0.2f);                              //0.2�� �Ŀ� 
        GetComponent<Renderer>().material.color = Color.green;              //���� �ʷϻ����� �����. 
    }

    IEnumerator Die()
    {
        GetComponent<Renderer>().material.color = Color.red;                //���� ���������� �����.
        Vector3 startScale = transform.localScale;
        float timer = 0f;

        while (timer < 0.5f)                                            //0.5�� �����϶����� �ݺ� ���� �Ѵ�. 
        {
            timer += Time.deltaTime;                                                        //�ð��� �þ��.
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, timer / 0.5f);    //�������� �ڿ������� ���δ�. 
            yield return null;                                                              //�� ������ ����ȴ�. 
        }

        Destroy(gameObject);                                                //������Ʈ�� �ı� �Ѵ�. 
    }

}
