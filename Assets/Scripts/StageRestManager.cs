using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageRestManager : MonoBehaviour
{
    public Text stageText, moneyText;
    public Text[] abilityText = new Text[5];
    int money;
    int[] abilityOrigin = new int[5];
    int[] abilityIncrease = new int[5];
    int[] abilityCost = new int[5];
    public Button[] minus = new Button[5];
    public GameObject msgCan;
    // Start is called before the first frame update
    void Start()
    {
        //SceneManager.SetActiveScene(SceneManager.GetSceneByName("StageRestScene"));
        money = GameManager.instance.money;
        moneyText.text = money.ToString();
        stageText.text = "Level " + GameManager.instance.level.ToString();
        for(int i =0;i<5;i++)
        {
            abilityOrigin[i] = PlayerScript.instance.ability[i];
            abilityText[i].text = abilityOrigin[i].ToString() ;
        }
        for (int i = 0; i < 5; i++)
        {
            abilityCost[i]=500;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void plusBtn(Button btn)
    {
        if (msgCan.activeSelf == false)
        {
            int index = int.Parse(btn.name.Substring(4));
            if (money < abilityCost[index])
            {
                msgCan.SetActive(true);
            }
            else
            {
                money -= abilityCost[index];
                moneyText.text = money.ToString();
                abilityIncrease[index] += 1;
                abilityText[index].text = (abilityOrigin[index] + abilityIncrease[index]).ToString();
                minus[index].gameObject.SetActive(true);
            }
        }
    }
    public void minusBtn(Button btn)
    {
        if(msgCan.activeSelf==false)
        {
            int index = int.Parse(btn.name.Substring(5));
            if (abilityIncrease[index] > 0)
            {
                money += abilityCost[index];
                moneyText.text = money.ToString();
                abilityIncrease[index] -= 1;
                if (abilityIncrease[index] < 1)
                    btn.gameObject.SetActive(false);
                abilityText[index].text = (abilityOrigin[index] + abilityIncrease[index]).ToString();
            }
            
        }
        
    }
    // 메인화면으로 나갈 때
    public void exitBtn() 
    {

    }
    // 다음 스테이지 버튼
    public void nextBtn()
    {
        if(msgCan.activeSelf == false)
        {
            GameManager.instance.money = money;
            for (int i = 0; i < 5; i++)
            {
                PlayerScript.instance.ability[i] += abilityIncrease[i];
            }
            SceneManager.UnloadSceneAsync(2);
            GameManager.instance.reStart();
        }
        
    }
    // 메세지 확인 버튼
    public void msgCanOK()
    {
        msgCan.SetActive(false);
    }
}
