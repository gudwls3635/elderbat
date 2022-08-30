using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Man1 : MonoBehaviour
{
    // Start is called before the first frame update
    int state, count;
    int[,] npcSelectPercent = new int[,] { { 50, 100, -1 }, { 100, -1, -1 }, { -1, -1, -1 } };
    string[] npcSelectItem = new string[] { "뒤져본다 (3초)", "그만둔다 (0초)", "" };
    int[,] npcReward = new int[,] { { 100, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };
    int npcTime = 3;
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

    }
    public void interactionNpc()
    {
        state = 1;
    }
    public void selectBtn(int currentNpc, int index)
    {
        /*
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
        if (npcSelectItem[index].Contains("뒤져본다"))
        {
            GameManager.instance.money += npcReward[index, rewardIndex];
            GameManager.instance.interationScreen.transform.GetChild(4).GetComponent<Text>().text = "보상 " + npcReward[index, rewardIndex];
            // 선택지를 누른 결과 시간 감소
            GameManager.instance.timer -= npcTime;
            if (GameManager.instance.timer < 0) GameManager.instance.timer = 0;
        }
        // 선택지 화면 닫기
        GameManager.instance.exitSelectBtn();
        state = 3;
        */
    }
    void checkClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (state == 1) // npc와 interation 성공 후 선택지 띄우기
            {
                /*
                state = 2;
                count = GameManager.instance.makeSelectBtn(); // 선택지 버튼 생성 및 이름들 설정
                GameManager.instance.adjustSelectBtn(count); // 선택지 버튼 위치 조정
                */
                state = 0;
                GameManager.instance.interationScreen.SetActive(false);

                // 획득한 보상 UI에 업데이트
                GameManager.instance.updateMoney(0);
                GameManager.instance.gameTime = true;
                transform.GetComponent<Man1>().enabled = false;

            }
            else if (state == 3) // 선택지 누른 후 결과창 보여준 후
            {
                /*
                state = 0;
                GameManager.instance.interationScreen.SetActive(false);

                // 획득한 보상 UI에 업데이트
                GameManager.instance.updateMoney(0);
                GameManager.instance.gameTime = true;
                transform.GetComponent<Vending>().enabled = false;
                */
            }
        }
    }
}
