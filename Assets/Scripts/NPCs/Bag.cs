using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bag : MonoBehaviour
{
    // Start is called before the first frame update
    int state, count;
    int[,] npcSelectPercent = new int[,] { { 100, -1, -1 }, { 100, -1, -1 }, { -1, -1, -1 } };
    string[] npcSelectItem = new string[] { "가진다 (2초)", "경찰서에 맡긴다 (5초)", "그만둔다 (0초)" };
    int[,] npcReward = new int[,] { { 100, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };
    int[] npcTime = new int[] { 2,5,0};
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
        if (npcSelectItem[index].Contains("가진다"))
        {
            bool success = PlayerScript.instance.itemPush("bag");
            if ( success ) //  가방을 얻었을 때
            {
                transform.position = new Vector3(999, 999); // 가방 스테이지에서 사라짐
                GameManager.instance.interationScreen.transform.GetChild(4).GetComponent<Text>().text = "가방을 얻었습니다";

            }
            else // 인벤토리가 꽉찼을 때
            {
                GameManager.instance.interationScreen.transform.GetChild(4).GetComponent<Text>().text = "인벤토리가 꽉찼습니다";
            }

            // 선택지를 누른 결과 시간 감소
            GameManager.instance.timer -= npcTime[0];
            if (GameManager.instance.timer < 0) GameManager.instance.timer = 0;
        }
        else if (npcSelectItem[index].Contains("경찰서에"))
        {
            GameManager.instance.interationScreen.transform.GetChild(4).GetComponent<Text>().text = "파출소에 맡겼다. 마음이 가벼워졌다.";
            GameManager.instance.npcEvent[9] = true; // 이벤트성공 이후 부자 출현
            transform.position = new Vector3(999, 999); // 가방 스테이지에서 사라짐
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
                transform.GetComponent<Bag>().enabled = false;
            }
        }
    }
}
