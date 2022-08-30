using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bubble : MonoBehaviour
{
    int state; // 0 : 빨강 1: 초록 
    int time;
    float secTimer;
    int money;
    Vector3 dest;
    // Start is called before the first frame update
    void Start()
    {
        state = 0;
        time = 6;
        secTimer = 0;
       
    }

    // Update is called once per frame
    void Update()
    {
        if (ElderFightSystem.instance.gameTime)
        {
            if (time > 0)
            {
                secTimer += Time.deltaTime;
                if (secTimer >= 1)
                {
                    secTimer = 0;
                    time -= 1;
                    if(time == 3) // 빨강 -> 초록
                    {
                        state = 1;
                        transform.GetComponent<Image>().color = new Color(0, 255, 0);
                    }else if(time == 0) // 초록 -> 사라짐 
                    {
                        state = 2;
                        Destroy(transform.gameObject);
                    }
                }
                
            }
            move();
        }
    }
    public void click()
    {
        if(state == 0)
        {
            ElderFightSystem.instance.healingShield(money);
            Destroy(transform.gameObject);
        }
        else if(state == 1)
        {
            ElderFightSystem.instance.attack(money);
            Destroy(transform.gameObject);
        }
    }
    public void setMoney(int money)
    {
        this.money = money;
        transform.GetChild(0).GetComponent<Text>().text = money.ToString();
        dest = transform.localPosition;
    }
    float t;
    void move()
    {
        t += Time.deltaTime*0.2f;
        //transform.localPosition = Vector3.Lerp(transform.localPosition, dest, t);
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, dest, 5f);
        if (Vector3.Distance(dest,transform.localPosition) <=20)
        {
            t = 0;
            dest = ElderFightSystem.instance.getRandomPos();
        }
    }
}
