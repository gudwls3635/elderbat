using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoliceStation : MonoBehaviour
{
    // Start is called before the first frame update
    int state, count;
    int[,] npcSelectPercent = new int[,] { { 100, -1, -1 }, { 100, -1, -1 }, { -1, -1, -1 } };
    string[] npcSelectItem = new string[] { "수배자 리스트를 확인한다. (2초)", "분실물을 맡긴다. (3초)", "그만둔다 (0초)" };
    int[,] npcReward = new int[,] { { 100, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };
    int[] npcTime = new int[] { 2, 3, 0 };
    public string lostItem; // 분실물 아이템
    public GameObject wantedCanvas;
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
        //경찰서 분실물 아이템 지정
       lostItem = "bag";
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
        if (npcSelectItem[index].Contains("수배자"))
        {
            wantedCanvas.SetActive(true); // 수배자 리스트 창 On
            GameManager.instance.interationScreen.transform.GetChild(4).GetComponent<Text>().text = "수배자 000. 죄목: 000. 이 녀석이구나.";
            GameManager.instance.npcEvent[7] = true; // 수배자 리스트 확인 이후 수배범 생성
            // 선택지를 누른 결과 시간 감소
            GameManager.instance.timer -= npcTime[0];
            if (GameManager.instance.timer < 0) GameManager.instance.timer = 0;
        }
        else if (npcSelectItem[index].Contains("분실물을"))
        {
            if(PlayerScript.instance.getItemIndex(lostItem) == -1) // 인벤토리에 분실물이 없을 때
            {
                GameManager.instance.interationScreen.transform.GetChild(4).GetComponent<Text>().text = "분실물을 가지고 있지 않다";
            }
            else
            {
                PlayerScript.instance.itemPop(PlayerScript.instance.getItemIndex(lostItem));
                GameManager.instance.interationScreen.transform.GetChild(4).GetComponent<Text>().text = "분실물을 경찰서에 맡겼다";
                GameManager.instance.npcEvent[9] = true; // 가방 이벤트 성공 이후 부자 출현
            }
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
                wantedCanvas.SetActive(false);// 수배자 리스트 창 Off

                state = 0;
                GameManager.instance.interationScreen.SetActive(false);

                // 획득한 보상 UI에 업데이트
                GameManager.instance.updateMoney(0);
                GameManager.instance.gameTime = true;
                transform.GetComponent<PoliceStation>().enabled = false;
            }
        }
    }
}
