using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class PawnShop : MonoBehaviour
{
    // Start is called before the first frame update
    public List<string> items;
    int itemLimit = 28;
    public int selectItem;
    public Canvas pawnInventoryCanvas;
    public GameObject MsgCanvas,pawnMsgCanvas;
    public GameObject content;
    int btnKind; // 인벤토리 0: 맡기기, 1: 찾기
    int state, count;
    int[,] npcSelectPercent = new int[,] { { 100, -1, -1 }, { 100, -1, -1 }, { 100, -1, -1 } };
    string[] npcSelectItem = new string[] { "물건을 맡긴다 (3초)", "물건을 찾는다 (3초)", "그만둔다 (0초)" };
    int npcTime = 3;
    void Start()
    {
        selectItem = -1;
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
        if (npcSelectItem[index].Contains("물건을 맡긴다"))
        {
            // 인벤토리 열기
            showPlayerInventory();
            // 선택지를 누른 결과 시간 감소
            GameManager.instance.timer -= npcTime;
            if (GameManager.instance.timer < 0) GameManager.instance.timer = 0;
        }else if (npcSelectItem[index].Contains("물건을 찾는다"))
        {
            // 전당포 인벤토리 열기
            showPawnInventory();
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
        pawnInventoryCanvas.gameObject.SetActive(true);
        btnKind = 0;
        pawnInventoryCanvas.transform.GetChild(3).GetChild(0).GetComponent<Text>().text = "맡기기";
        List<string> tmpItems = PlayerScript.instance.items;
        for (int i = 0; i < content.transform.childCount; i++)
        {
            UnityEngine.UI.Image tmpComponent = content.transform.GetChild(i).GetChild(0).GetComponent<Image>();
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
    void showPawnInventory()
    {
        pawnInventoryCanvas.gameObject.SetActive(true);
        btnKind = 1;
        pawnInventoryCanvas.transform.GetChild(3).GetChild(0).GetComponent<Text>().text = "찾기";
        for (int i = 0; i < content.transform.childCount; i++)
        {
            UnityEngine.UI.Image tmpComponent = content.transform.GetChild(i).GetChild(0).GetComponent<Image>();
            if (i < items.Count)
            {
                GameData.itemName tmp = (GameData.itemName)Enum.Parse(typeof(GameData.itemName), items[i].ToString());
                tmpComponent.sprite = GameData.instance.itemImgs[(int)tmp];
            }
            else
            {
                tmpComponent.sprite = null;
            }
            tmpComponent.color = new Color(1f, 1f, 1f); // 전에 선택한곳 원래 색상으로
        }
    }
    public void selectedItem(UnityEngine.UI.Button button)
    {
        if(MsgCanvas.activeSelf == false && pawnMsgCanvas.activeSelf==false)
        {
            int oldSelectItem = selectItem;
            int ch = 0;
            // 몇번째 버튼을 선택했는지
            int itemCount;
            if (btnKind == 0) // 맡기기
            {
                itemCount = PlayerScript.instance.items.Count;
            }
            else // 찾기
            {
                itemCount = items.Count;
            }
            for (int i = 0; i < content.transform.childCount; i++)
            {
                if (content.transform.GetChild(i).name.Equals(button.name))
                {
                    if (i < itemCount) // 선택한 버튼에 아이템이 존재하는지
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
        if(MsgCanvas.activeSelf == false && pawnMsgCanvas.activeSelf == false)
        {
            content.transform.parent.parent.GetComponent<ScrollRect>().enabled=false; // 스크롤 안되게
            if (btnKind == 0) // 맡기기
            {
                if (selectItem > -1)
                {
                    string selectName = PlayerScript.instance.items[selectItem];
                    pawnMsgCanvas.SetActive(true);
                    pawnMsgCanvas.transform.GetChild(1).GetComponent<Text>().text = GameData.instance.getKorItem(selectName) + " 맡기시겠습니까?\n보상 " + GameData.instance.getCostItem(selectName) + "원";

                }
                else // 아이템을 선택 안했을 시
                {
                    MsgCanvas.transform.GetChild(2).GetComponent<Text>().text = "아이템을 선택해주세요";
                    MsgCanvas.SetActive(true);
                }
            }
            else // 찾기
            {
                if (selectItem > -1)
                {
                    pawnMsgCanvas.SetActive(true);
                    pawnMsgCanvas.transform.GetChild(1).GetComponent<Text>().text = GameData.instance.getKorItem(items[selectItem]) + " 찾으시겠습니까?\n지불 " + GameData.instance.getCostItem(items[selectItem]) * 1.1f + "원";

                }
                else // 아이템을 선택 안했을 시
                {
                    MsgCanvas.transform.GetChild(2).GetComponent<Text>().text = "아이템을 선택해주세요";
                    MsgCanvas.SetActive(true);
                }
            }
        }
        
    }

    public void pawnMsgYes()
    {

        if(btnKind==0) // 맡기기 확인
        {
            int cost = GameData.instance.getCostItem(PlayerScript.instance.items[selectItem]);
            pawnMsgCanvas.SetActive(false);
            itemPush(PlayerScript.instance.items[selectItem]);
            PlayerScript.instance.itemPop(selectItem);
            showPlayerInventory();
            GameManager.instance.interationScreen.transform.GetChild(4).GetComponent<Text>().text = "물건을 맡겼다";

            content.transform.parent.parent.GetComponent<ScrollRect>().enabled = true; // 스크롤 가능하게
            selectItem = -1;
            GameManager.instance.updateMoney((int)(cost * 0.9f)); // 아이템 가격 0.9 만큼 이득
        }
        else // 찾기 확인
        {
            pawnMsgCanvas.SetActive(false);
            if (PlayerScript.instance.items.Count < PlayerScript.instance.itemLimit)
            {
                int cost = GameData.instance.getCostItem(items[selectItem]);
                PlayerScript.instance.itemPush(items[selectItem]);
                itemPop(selectItem);
                showPawnInventory();
                GameManager.instance.interationScreen.transform.GetChild(4).GetComponent<Text>().text = "물건을 찾았다";

                content.transform.parent.parent.GetComponent<ScrollRect>().enabled = true; // 스크롤 가능하게
                selectItem = -1;
                GameManager.instance.updateMoney( -1* (int)(cost *1.1f )); // 아이템 1.1 가격만큼 지출
            }
            else  // 플레이어 인벤토리가 꽉찼을 시
            {
                MsgCanvas.transform.GetChild(2).GetComponent<Text>().text = "인벤토리가 꽉찼습니다";
                MsgCanvas.SetActive(true);
            }
        }
        
    }
    public void pawnMsgNo()
    {
        pawnMsgCanvas.SetActive(false);
        content.transform.parent.parent.GetComponent<ScrollRect>().enabled = true; // 스크롤 가능하게
        content.transform.GetChild(selectItem).GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f); //선택한 버튼 원래 색상으로
        selectItem = -1;
    }
    public void exitInventory()
    {
        if(MsgCanvas.activeSelf == false && pawnMsgCanvas.activeSelf == false)
        {
            pawnInventoryCanvas.gameObject.SetActive(false);
            state = 3;
        }
        
    }
    public void ok()
    {
        content.transform.parent.parent.GetComponent<ScrollRect>().enabled = true; // 스크롤 가능하게
        MsgCanvas.SetActive(false);
    }
    void itemPush(string item)
    {
        int ch = 0;
        for (int i = 0; i < items.Count; i++)
        {
            if (item.CompareTo(items[i]) < 0)
            {
                items.Insert(i, item);
                ch = 1;
                break;
            }
        }
        if (ch == 0)
        {
            items.Add(item);
        }
    }
    void itemPop(int index)
    {
        items.RemoveAt(index);
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
                transform.GetComponent<PawnShop>().enabled = false;
            }
        }
    }

}
