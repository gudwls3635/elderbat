﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lotto : MonoBehaviour
{
    int state, count;
    string[] npcSelectItem = new string[] { "원짜리를 산다 (5초)", "그만둔다 (0초)" };
    int[,] npcSelectPercent = new int[,]  { { 5, 40, 100 }, { 100, -1, -1 } };
    int[] npcReward = new int[] {500,1000,5000,10000 };
    int npcTime = 5;
    public int id;
    public Sprite[] imgList = new Sprite[5];
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        checkClick();
    }
    public void initData()
    {
        id = GameManager.instance.npcInMap[1];
        transform.GetComponent<SpriteRenderer>().sprite = imgList[id];
        npcSelectItem[0] = npcReward[id] + npcSelectItem[0];
    }
    public void interactionNpc()
    {
        state = 1;
    }
    public void selectBtn(int currentNpc, int index)
    {
        int rewardIndex = -1;
        // 선택지 눌렀을 때 확률 계산
        int rand = UnityEngine.Random.Range(1, 101);
        for (int i = 0; i < 3; i++)
        {
            if (rand <= npcSelectPercent[index, i])
            {
                rewardIndex = i;
                break;
            }
        }
        // 선택지에 따른 행동 보상
        if (npcSelectItem[index].Contains("짜리를 산다"))
        {
            if (GameManager.instance.money < npcReward[id])
            {
                GameManager.instance.interationScreen.transform.GetChild(4).GetComponent<Text>().text = "돈이 부족합니다";
            }
            else
            {
                GameManager.instance.money -= npcReward[id];
                if(rewardIndex == 0) // 5배
                {
                    GameManager.instance.interationScreen.transform.GetChild(4).GetComponent<Text>().text = "보상 " + npcReward[id] * 5 +"원";
                    GameManager.instance.money += npcReward[id] * 5;
                }
                else if(rewardIndex == 1) // 본전
                {
                    GameManager.instance.interationScreen.transform.GetChild(4).GetComponent<Text>().text = "보상 " + npcReward[id] + "원";
                    GameManager.instance.money += npcReward[id];
                }
                else // 꽝
                {
                    GameManager.instance.interationScreen.transform.GetChild(4).GetComponent<Text>().text = "보상 0원";
                }
                
                
            }
            // 선택지를 누른 결과 시간 감소
            GameManager.instance.timer -= npcTime;
            if (GameManager.instance.timer < 0) GameManager.instance.timer = 0;
        }
        else
        {
            // 그만둔다
        }
        // 선택지 화면 닫기
        GameManager.instance.exitSelectBtn();
        state = 3;
    }
    void checkClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (state == 1) // npc와 interation 성공 후 선택지 띄우기
            {
                state = 2;
                count = GameManager.instance.makeSelectBtn(npcSelectItem); // 선택지 버튼 생성 및 이름들 설정
                GameManager.instance.adjustSelectBtn(count); // 선택지 버튼 위치 조정

            }
            else if (state == 3) // 선택지 누른 후 결과창 보여준 후
            {
                state = 0;
                GameManager.instance.interationScreen.SetActive(false);

                // 획득한 보상 UI에 업데이트
                GameManager.instance.updateMoney(0);
                GameManager.instance.gameTime = true;
                transform.GetComponent<Lotto>().enabled = false;
            }
        }
    }
}
