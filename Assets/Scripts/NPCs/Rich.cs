using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rich : MonoBehaviour
{
    // Start is called before the first frame update
    int state, count;
    int[,] npcSelectPercent = new int[,] { { 100, -1, -1 }, { 100, -1, -1 }, { -1, -1, -1 } };
    string[] npcSelectItem = new string[] { "가방을 건넨다 (4초)", "그만둔다 (0초)", "" };
    int[,] npcReward = new int[,] { { 100, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };
    int npcTime = 4;
    void Start()
    {
    }
    public void initData()
    {
        transform.GetComponent<Animator>().SetInteger("igb", 0);
    }
    // Update is called once per frame
    void Update()
    {
        checkClick();
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
        if (npcSelectItem[index].Contains("가방"))
        {
            if (PlayerScript.instance.getItemIndex("bag") == -1) //  가방이 없을 때
            {
                GameManager.instance.interationScreen.transform.GetChild(4).GetComponent<Text>().text = "가방이 어디 있다는거죠? 이래서 서민들은....";
                transform.GetComponent<Animator>().SetInteger("igb", 2);

            }
            else // 가방이 있을 때
            {
                bool success = PlayerScript.instance.itemPush("cleaner");
                if( success )
                {
                    PlayerScript.instance.itemPop(PlayerScript.instance.getItemIndex("bag"));
                    GameManager.instance.interationScreen.transform.GetChild(4).GetComponent<Text>().text = "어머 내 가방! 정말 고마워요. 우리 회사 청소긴데.. 보답으로 드리죠 호호호";
                    transform.GetComponent<Animator>().SetInteger("igb", 1);
                }
                else
                {
                    GameManager.instance.interationScreen.transform.GetChild(4).GetComponent<Text>().text = "인벤토리가 꽉찼습니다";
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
                transform.GetComponent<Rich>().enabled = false;
            }
        }
    }
}
