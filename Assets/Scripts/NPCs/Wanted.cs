using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Wanted : MonoBehaviour
{
    // Start is called before the first frame update
    public int wantedCreatePer; // 수배자 생성 확률
    public int stack; // 수배자 스택
    public int wantedCost; // 현상금
    int state, count;
    int[,] npcSelectPercent = new int[,] { { 100, -1, -1 }, { 100, -1, -1 }, { -1, -1, -1 } };
    string[] npcSelectItem = new string[] { "잡아서 파출소에 넘긴다 (5초)", "그만둔다 (0초)"};
    int[,] npcReward = new int[,] { { 100, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };
    int[] npcTime = new int[] { 5, 0, 0 };
    bool capture;
    public GameObject wantedCan;
    
    void Start()
    {
        wantedCost = 1000;
        wantedCreatePer = 50; // 생성확률
    }

    // Update is called once per frame
    void Update()
    {
        checkClick();
    }
    public void initData()
    {
        // 수배자 생성
        wantedCreatePer = 50; // 생성확률
    }
    public void interactionNpc()
    {
        state = 1;
    }
    public void selectBtn(int currentNpc, int index) // 잡혔는지
    {
        if(capture ==false)
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
            if (npcSelectItem[index].Contains("잡아서"))
            {
                
                GameManager.instance.money += wantedCost;// 현상금 자금에 추가 
                //wantedCan.SetActive(true);
                GameManager.instance.interationScreen.transform.GetChild(4).GetComponent<Text>().text = "수배자를 파출소에 넘겼다\n현상금 : " + wantedCost;
                transform.GetComponent<Animator>().SetBool("capture", true);
                capture = true;
                stack = 0; // 스택초기화

                // 선택지를 누른 결과 시간 감소
                GameManager.instance.timer -= npcTime[0];
                if (GameManager.instance.timer < 0) GameManager.instance.timer = 0;
            }
            else if (npcSelectItem[index].Contains("그만둔다"))
            {

            }
            // 선택지 화면 닫기
            GameManager.instance.exitSelectBtn();
            state = 3;
        }
        
    }
    void checkClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (state == 1) // npc와 interation 성공 후 선택지 띄우기
            {
                if(stack >= 3) // 스택 3번 이면
                {
                    state = 2;
                    count = GameManager.instance.makeSelectBtn(npcSelectItem); // 선택지 버튼 생성 및 이름들 설정
                    GameManager.instance.adjustSelectBtn(count); // 선택지 버튼 위치 조정

                }else
                {
                    stack += 1; //스택증가
                    changePos(); // 장소변경

                    state = 0;
                    GameManager.instance.interationScreen.SetActive(false);

                    // 시간 소비
                    GameManager.instance.timer -= 2;
                    if (GameManager.instance.timer < 0) GameManager.instance.timer = 0;

                    // 획득한 보상 UI에 업데이트
                    GameManager.instance.updateMoney(0);
                    GameManager.instance.gameTime = true;
                    transform.GetComponent<Wanted>().enabled = false;
                }
                
            }
            else if (state == 3) // 선택지 누른 후 결과창 보여준 후
            {

                state = 0;
                GameManager.instance.interationScreen.SetActive(false);
                if(capture == true)
                {
                    StartCoroutine(disappear());
                }
                    
                //wantedCan.SetActive(false);

                // 획득한 보상 UI에 업데이트
                GameManager.instance.updateMoney(0);
                GameManager.instance.gameTime = true;
                transform.GetComponent<Wanted>().enabled = false;
            }
        }
    }
    public void changePos()
    {
        GameManager.instance.personCheck[GameManager.instance.npcDistrict[transform.gameObject]] = 0;
        int limitCh = 0;
        for (int i = 0; i < GameManager.instance.mapBlock.Count; i++)
        {
            if (GameManager.instance.personCheck[GameManager.instance.mapBlock[i]] == 0)
            {
                limitCh = 1;
                break;
            }
        }
        if (limitCh == 1) // 구역의 공간이 있을 시
        {
            int rand;
            do
            {
                rand = Random.Range(0, GameManager.instance.mapBlock.Count);
            } while (GameManager.instance.personCheck[GameManager.instance.mapBlock[rand]] == 1);
            int district = GameManager.instance.mapBlock[rand];
            GameManager.instance.personCheck[district] = 1;
            GameData.npcData npc = GameData.instance.npcDataList[8];
            GameManager.instance.npcDistrict[transform.gameObject] = district;
            int x = Random.Range(district % 5 * 11 + npc.x / 2, district % 5 * 11 + 10 - (npc.x / 2)); // 가로 구역당 10칸 중간에 1칸 짝수 크기 건물인 경우 
            int y = Random.Range(district / 5 * 19 + 10 + npc.y / 2, district / 5 * 19 + 10 + 9 - (npc.y / 2));
            transform.position = new Vector3(x, y);
        }
    }
    IEnumerator disappear()
    {
        yield return new WaitForSeconds(1.5f);
        while (true)
        {
            if (GameManager.instance.gameTime)
                break;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        transform.position = new Vector3(999, 999);// 스테이지에서 없애기
    }
}
