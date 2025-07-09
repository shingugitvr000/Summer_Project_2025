using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("공격 설정")]
    [SerializeField] private float attackRange = 5f;                //공격 범위는 5
    [SerializeField] private int damage = 30;                       //데미지는 30

    [Header("반복문 선택 옵션")]
    [SerializeField] private int loopType = 0;                      //0 = Foreach, 1 = For, 2 = While , 3= DoWhile

    void Update()
    {
        float h = Input.GetAxis("Horizontal");                              //수평 이동 감지
        float v = Input.GetAxis("Vertical");                                //수직 이동 감지
        transform.Translate(new Vector3(h, 0, v) * 5f * Time.deltaTime);    //해당 오브젝트 움직임 설정 

        if (Input.GetKeyDown(KeyCode.Space))
        {
            AreaAttack();
        }
    }

    void AreaAttack()
    {
        //범위 내 적 찾기
        List<Enemy> enemies = new List<Enemy>();                            //적 리스트를 선언 한다. 
        Collider[] colliders = Physics.OverlapSphere(transform.position, attackRange);      //공격 범위 안에 있는 콜라이더들을 가져온다. 

        foreach(Collider col in colliders)                                      //foreach 문으로 colliders 배열에 있는 모든 오브젝트에 접근해서
        {
            Enemy enmey = col.GetComponent<Enemy>();                            //Enemy 컴포넌트를 받아오고
            if( enmey != null ) enemies.Add(enmey);                             //Enmey 컴포넌트가 있을 경우 list 배열에 추가한다. 
        }

        switch (loopType)
        {
            case 0: //foreach
                foreach (Enemy enemy in enemies)                                //enemies 리스트에 있는 모든 오브젝트에 접근해서
                {
                    enemy.TakeDamage(damage);                                   //데미지를 준다. 
                }
                break;
            case 1: //For                                                       //For 문으로 변환
                for(int i = 0; i < enemies.Count; i++)
                {
                    enemies[i].TakeDamage(damage);
                }
                break;
            case 2: //While 
                int j = 0;                                                      //While 문으로 변환
                while (j < enemies.Count)
                {
                    enemies[j].TakeDamage(damage);
                    j++;
                }
                break;
            case 3: //DoWhile
                if (enemies.Count > 0)                                          //DoWhile 문으로 변환
                {
                    int k = 0;
                    do
                    {
                        enemies[k].TakeDamage(damage);
                        k++;
                    }
                    while (k < enemies.Count);
                }
                break;
            
        }

    }

    void OnDrawGizmos()                                                     //기즈모를 통해서 공격 범위를 빨간색 구체 와이어로 표시 한다. 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
