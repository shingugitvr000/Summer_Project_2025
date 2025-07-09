using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int health = 100;          //체력 변수 선언
        
    void Start()
    {
        GetComponent<Renderer>().material.color = Color.green;              //적을 초록색으로 만든다. 
    }

    public void TakeDamage(int damage)                      //데미지 받는 함수 설정
    {
        health -= damage;
        StartCoroutine(DamageEffect());                     //데미지 연출 실행

        if(health <= 0)                                     //죽는 연출을 위한 체력 검사 
        {   
            StartCoroutine(Die());                          //죽는 연출 실행
        }
    }

    IEnumerator DamageEffect()
    {
        GetComponent<Renderer>().material.color = Color.red;                //적을 빨간색으로 만든다.
        yield return new WaitForSeconds(0.2f);                              //0.2초 후에 
        GetComponent<Renderer>().material.color = Color.green;              //적을 초록색으로 만든다. 
    }

    IEnumerator Die()
    {
        GetComponent<Renderer>().material.color = Color.red;                //적을 빨간색으로 만든다.
        Vector3 startScale = transform.localScale;
        float timer = 0f;

        while (timer < 0.5f)                                            //0.5초 이하일때까지 반복 실행 한다. 
        {
            timer += Time.deltaTime;                                                        //시간이 늘어난다.
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, timer / 0.5f);    //스케일을 자연스럽에 줄인다. 
            yield return null;                                                              //매 프레임 실행된다. 
        }

        Destroy(gameObject);                                                //오브젝트를 파괴 한다. 
    }

}
