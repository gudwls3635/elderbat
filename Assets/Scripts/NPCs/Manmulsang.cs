using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manmulsang : MonoBehaviour
{
    // Start is called before the first frame update
    int state, count;
    int[,] npcSelectPercent = new int[,] { { 100, -1, -1 }, { 100, -1, -1 }, { -1, -1, -1 } };
    string[] npcSelectItem = new string[] { "을/를 산다 (2초)", "아르바이트를 한다 (5초)", "그만둔다 (0초)" };
    int todayItem;
    int[] npcTime = new int[] { 3, 5, 0 };
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
        //만물상 아이템 지정
        int random = Random.Range(0, GameData.instance.itemDatas.Count);
        todayItemSelect(random);
    }
    public void todayItemSelect(int index)
    {
        todayItem = index;
        npcSelectItem[0] = GameData.instance.itemDatas[index].korName+npcSelectItem[0]; // Manmulsang 스크립트
        GameManager.instance.npcConversation[6][0] = "오늘은 " + GameData.instance.itemDatas[index].korName + "을/를 판다...";
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
        if (npcSelectItem[index].Contains("을/를 산다"))
        {
            int cost = GameData.instance.itemDatas[todayItem].cost;
            if ( GameManager.instance.money < cost )
            {
                GameManager.instance.interationScreen.transform.GetChild(4).GetComponent<Text>().text = "돈이 부족합니다...";
               
            }
            else
            {
                bool success = PlayerScript.instance.itemPush("bottle");
                if (success)
                {
                    GameManager.instance.updateMoney(-1 * cost);
                    GameManager.instance.interationScreen.transform.GetChild(4).GetComponent<Text>().text = GameData.instance.itemDatas[todayItem].korName + "을/를 구입했습니다";
                }
                else
                {
                    GameManager.instance.interationScreen.transform.GetChild(4).GetComponent<Text>().text = "인벤토리가 꽉찼습니다";
                }
            }
            
            // 선택지를 누른 결과 시간 감소
            GameManager.instance.timer -= npcTime[0];
            if (GameManager.instance.timer < 0) GameManager.instance.timer = 0;
        }
        else if (npcSelectItem[index].Contains("아르바이트를 한다"))
        {
            GameManager.instance.interationScreen.transform.GetChild(4).GetComponent<Text>().text = "… 오늘은 여기까지만 하자. \"능력치2\"가 더 필요할 것 같다";
            GameManager.instance.money += PlayerScript.instance.ability[1] * 1000;
            // 선택지를 누른 결과 시간 감소
            GameManager.instance.timer -= npcTime[1];
            if (GameManager.instance.timer < 0) GameManager.instance.timer = 0;
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
                transform.GetComponent<Manmulsang>().enabled = false;
            }
        }
    }
}
