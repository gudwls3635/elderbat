using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingSide : MonoBehaviour
{
    int state, count;
    string[] npcSelectItem = new string[] { "병을 줍는다 (2초)", "그만둔다 (0초)", "" };
    int[,] npcSelectPercent = new int[,] { { 100, -1, -1 }, { 100, -1, -1}, { -1, -1, -1 } };
    string[,] npcReward = new string[,] { { "bottle", "", "" }, { "", "", "" }, { "", "", "" } };
    int npcTime = 2;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void initData()
    {

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
        if (npcSelectItem[index].Contains("병을 줍는다"))
        {
            bool success = PlayerScript.instance.itemPush("bottle");
            if(success)
            {
                GameManager.instance.interationScreen.transform.GetChild(4).GetComponent<Text>().text = "빈 병을 획득했습니다";
                changePos();
            }
            else
            {
                GameManager.instance.interationScreen.transform.GetChild(4).GetComponent<Text>().text = "인벤토리가 꽉찼습니다";
            }
            // 선택지를 누른 결과 시간 감소
            GameManager.instance.timer -= npcTime;
            if (GameManager.instance.timer < 0) GameManager.instance.timer = 0;
        }
        // 선택지 화면 닫기
        GameManager.instance.exitSelectBtn();
        state = 3;
    }
    void changePos()
    {
        GameManager.instance.buildingCheck[GameManager.instance.npcDistrict[transform.gameObject]] = 0;
        int limitCh = 0;
        for (int i = 0; i < GameManager.instance.mapBlock.Count; i++)
        {
            if (GameManager.instance.buildingCheck[GameManager.instance.mapBlock[i]] == 0)
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
            } while (GameManager.instance.buildingCheck[GameManager.instance.mapBlock[rand]] == 1);
            int district = GameManager.instance.mapBlock[rand];
            GameManager.instance.buildingCheck[district] = 1;
            GameData.npcData npc = GameData.instance.npcDataList[2];
            GameManager.instance.npcDistrict[transform.gameObject] = district;
            int x = Random.Range(district % 5 * 11 + npc.x / 2, district % 5 * 11 + 10 - (npc.x / 2)); // 가로 구역당 10칸 중간에 1칸 짝수 크기 건물인 경우 
            int y = Random.Range(district / 5 * 19 + npc.y / 2, district / 5 * 19 + 9 - (npc.y / 2));
            transform.position = new Vector3(x, y);
        }
       
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
                transform.GetComponent<BuildingSide>().enabled = false;
            }
        }
    }
}
