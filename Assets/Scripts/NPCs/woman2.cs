using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class woman2 : MonoBehaviour
{
    // Start is called before the first frame update
    int state, count;
    int[,] npcSelectPercent = new int[,] { { 100, -1, -1 }, { 100, -1, -1 }, { -1, -1, -1 } };
    string[] npcSelectItem = new string[] { "책을 건네준다 (4초)", "그만둔다 (0초)" };
    int[,] npcReward = new int[,] { { 100, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };
    int npcTime = 4;
    public GameObject woman2MsgCanvas;
    public Canvas woman2InventoryCanvas;
    public GameObject content;
    public int selectItem;
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
        if (npcSelectItem[index].Contains("책을"))
        {
            // 인벤토리 열기
            showPlayerInventory();
            // 선택지를 누른 결과 시간 감소
            GameManager.instance.timer -= npcTime;
            if (GameManager.instance.timer < 0) GameManager.instance.timer = 0;

        }
        else if (npcSelectItem[index].Contains("그만둔다"))
        {
            state = 3;
        }
        // 선택지 화면 닫기
        GameManager.instance.exitSelectBtn();
        
    }
    void showPlayerInventory()
    {
        woman2InventoryCanvas.gameObject.SetActive(true);
        selectItem = -1;
        List<string> tmpItems = PlayerScript.instance.items;
        
        for (int i = 0; i < content.transform.childCount; i++)
        {
            Image tmpComponent = content.transform.GetChild(i).GetChild(0).GetComponent<Image>();
            if (i < tmpItems.Count)
            {
                GameData.itemName tmp = (GameData.itemName)Enum.Parse(typeof(GameData.itemName), tmpItems[i].ToString());
                tmpComponent.sprite = GameData.instance.itemImgs[(int)tmp];
            }
            else
            {
                tmpComponent.sprite = null;
            }
            tmpComponent.color = new Color(1f, 1f, 1f); // 전에 선택한곳 원래 색상으로
        }
    }
    public void selectedItem(Button button)
    {
        if(woman2MsgCanvas.activeSelf == false)
        {
            int oldSelectItem = selectItem;
            int ch = 0;
            // 몇번째 버튼을 선택했는지
            for (int i = 0; i < content.transform.childCount; i++)
            {
                if (content.transform.GetChild(i).name.Equals(button.name))
                {
                    if (i < PlayerScript.instance.items.Count) // 선택한 버튼에 아이템이 존재하는지
                    {
                        selectItem = i;
                        ch = 1;
                    }

                    break;
                }
            }
            if (ch == 1) // 버튼 선택이 잘 되었으면
            {
                if (oldSelectItem > -1)
                    content.transform.GetChild(oldSelectItem).GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f); // 전에 선택한곳 원래 색상으로

                button.transform.GetChild(0).GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f); // 선택한 버튼 색상 변경
            }
        }

    }
    public void inventoryBtn()
    {
        if(woman2MsgCanvas.activeSelf == false)
        {
            // 건네기 버튼을 눌렀을 때
            if (selectItem > -1)
            {
                string selectName = PlayerScript.instance.items[selectItem];
                content.transform.GetChild(selectItem).GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f); //선택한 버튼 원래 색상으로
                woman2InventoryCanvas.gameObject.SetActive(false);

                if (selectName.Equals("book")) // 책을 건넸을 때
                {
                    PlayerScript.instance.itemPop(selectItem);
                    GameManager.instance.interationScreen.transform.GetChild(4).GetComponent<Text>().text = "어머? 감사합니다! 이건 다 읽은 책인데... 드릴게요!";
                    transform.GetComponent<Animator>().SetInteger("igb", 1);
                }
                else // 책이 아닐 시
                {
                    GameManager.instance.interationScreen.transform.GetChild(4).GetComponent<Text>().text = "장난치세요? 전 시간이 없다구요.";
                    transform.GetComponent<Animator>().SetInteger("igb", 2);
                }
                state = 3;
            }
            else // 아이템을 선택 안했을 시
            {
                woman2MsgCanvas.transform.GetChild(2).GetComponent<Text>().text = "아이템을 선택해주세요";
                woman2MsgCanvas.SetActive(true);
            }
        }
       
    }
    public void woman2MsgExit()
    {
        woman2MsgCanvas.SetActive(false);
    }
    public void exitInventory()
    {
        if (woman2MsgCanvas.activeSelf == false)
        {
            woman2InventoryCanvas.gameObject.SetActive(false);
            state = 3;
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
                transform.GetComponent<woman2>().enabled = false;
            }
        }
    }

}
