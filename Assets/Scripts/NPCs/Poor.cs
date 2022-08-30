using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Poor : MonoBehaviour
{
    // Start is called before the first frame update
    int state, count;
    int[,] npcSelectPercent = new int[,] { { 100, -1, -1 }, { 100, -1, -1 }, { 100, -1, -1 } };
    public string[] npcSelectItem = new string[] { "돈을 들고 달아난다. (2초)", "적선한다 (2초)", "그만둔다 (0초)" }; // 내기를 한다 (2초)
    int[] npcTime =new int[] { 2,30};
    public int walletMoney;
    public int stack = 0;
    public GameObject poorCanvas;
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
        transform.GetComponent<Animator>().SetInteger("igb", 0);
        walletMoney = GameData.instance.levelInitMoney[GameManager.instance.level] * 2;
        stack = 0;
        npcSelectItem[1] = "적선한다 (2초)";
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
        if (npcSelectItem[index].Contains("돈을 들고 달아난다"))
        {
            int policePer = Random.Range(1, 101);
            if(policePer <=50)
            {
                poorCanvas.SetActive(true);
                poorCanvas.transform.GetChild(1).gameObject.SetActive(true);
                // 경찰에 잡힘
                GameManager.instance.interationScreen.transform.GetChild(4).GetComponent<Text>().text = "돈을 훔치다 경찰에 잡혔다";
                // 선택지를 누른 결과 시간 감소
                GameManager.instance.timer -= npcTime[1];
                if (GameManager.instance.timer < 0) GameManager.instance.timer = 0;
            }
            else
            {
                int randomMoney = Random.Range(1, 11) * 500;
                if (walletMoney < randomMoney)
                    randomMoney = walletMoney;
                GameManager.instance.money += randomMoney;
                walletMoney -= randomMoney;
                GameManager.instance.interationScreen.transform.GetChild(4).GetComponent<Text>().text = randomMoney.ToString() + "원을 훔쳤다";
                // 선택지를 누른 결과 시간 감소
                GameManager.instance.timer -= npcTime[0];
                if (GameManager.instance.timer < 0) GameManager.instance.timer = 0;
                transform.GetComponent<Animator>().SetInteger("igb", 2);
            }
            
        }
        else if (npcSelectItem[index].Contains("적선한다"))
        {
            if (GameManager.instance.money < 500)
            {
                GameManager.instance.interationScreen.transform.GetChild(4).GetComponent<Text>().text = "돈이 부족합니다";
            }
            else
            {
                GameManager.instance.money -= 500; walletMoney += 500; stack += 1;
                if(stack >= 10)
                {
                    stack = 10;
                    npcSelectItem[1] = "내기를 한다 (2초)";
                }
                GameManager.instance.interationScreen.transform.GetChild(4).GetComponent<Text>().text = "불쌍한 거지에게 500원을 줬다";
                transform.GetComponent<Animator>().SetInteger("igb", 1);
            }
            // 선택지를 누른 결과 시간 감소
            GameManager.instance.timer -= npcTime[0];
            if (GameManager.instance.timer < 0) GameManager.instance.timer = 0;
        }else if(npcSelectItem[index].Contains("내기를"))
        {
            poorCanvas.SetActive(true);
            poorCanvas.transform.GetChild(0).gameObject.SetActive(true);

        }
        else if (npcSelectItem[index].Contains("그만둔다"))
        {

        }
        // 선택지 화면 닫기
        GameManager.instance.exitSelectBtn();
        state = 3;
    }
    void checkClick()
    {
        if (Input.GetMouseButtonDown(0) && poorCanvas.transform.GetChild(0).gameObject.activeSelf ==false)
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
                poorCanvas.SetActive(false);
                poorCanvas.transform.GetChild(1).gameObject.SetActive(false);
                GameManager.instance.interationScreen.SetActive(false);

                // 획득한 보상 UI에 업데이트
                GameManager.instance.updateMoney(0);
                GameManager.instance.gameTime = true;
                transform.GetComponent<Poor>().enabled = false;
            }
        }
    }
    public void choice()
    {
        int rand = Random.Range(0, 3);
        int money = (int)(GameManager.instance.money * 0.1f);
        if (rand==0) // 성공
        {
            GameManager.instance.money += money;
            GameManager.instance.interationScreen.transform.GetChild(4).GetComponent<Text>().text = money + "원을 획득했다";
        }
        else // 실패
        {
            GameManager.instance.money -= money;
            GameManager.instance.interationScreen.transform.GetChild(4).GetComponent<Text>().text = money + "원을 잃었다";
        }
        poorCanvas.transform.GetChild(0).gameObject.SetActive(false);
        poorCanvas.SetActive(false);
        
    }
    
}
