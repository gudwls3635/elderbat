using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ElderFightSystem : MonoBehaviour
{
    public static ElderFightSystem instance;
    public GameObject bubble;
    public GameObject bubbles;
    public GameObject left, right, up, bottom; // 버블 한계선

    public Text moneyText,timeText;
    int[] attackMoney = new int[5];
    public int shield; // 노인의 현재 실드
    public int shieldOrigin; // 노인의 최대 실드
    public int shieldHealing; // 노인의 실드 회복량
    public bool shieldOn; // 노인의 실드가 on 인지
    public int hp; // 노인의 현재 체력
    public int hpOrigin; // 노인의 최대 체력

    public bool gameClear;

    public bool gameTime;
    public int time;
    float secTimer;

    public Image hpImg; // 노인의 체력바
    public Text hpText;
    public Image shieldImg; // 노인의 실드바
    public Text shieldText;

    public GameObject optionCan;
    public GameObject resultCan;
    // Start is called before the first frame update
    void Start()
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("ElderFightScene"));
        attackMoney = new int[] { 500, 1000, 5000, 10000, 50000 };
        gameTime = true;
        gameClear = false;
        secTimer = 0f;
        time = 60;

        hp = 5000;
        hpOrigin = 5000;

        shield = 1000;
        shieldOrigin = 1000;
        shieldHealing = 200;
        shieldOn = true;
        shieldImg.GetComponent<Image>().color = new Color(0, 0, 255);

        updateHP();
        updateShield();

        moneyText.text = GameManager.instance.money.ToString();
    }
    void Awake()
    {
        if (ElderFightSystem.instance == null)
        {
            ElderFightSystem.instance = this;
        }

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (gameTime)
        {
            if (time > 0)
            {
                secTimer += Time.deltaTime;
                if (secTimer >= 1)
                {
                    secTimer = 0;
                    time -= 1;
                    if(shieldOn == false)
                    {
                        shield += shieldHealing;
                        if (shield >= shieldOrigin)
                        {
                            shield = shieldOrigin;
                            shieldOn = true;
                            shieldImg.GetComponent<Image>().color = new Color(0, 0, 255);
                        }
                        updateShield();
                    }
                }
                timeText.text = time.ToString();
            }
            else
            {
                // 시간이 다 되었을 때
                timeText.text = "0";
                gameTime = false;
                timeOver();
            }


        }
    }
    void timeOver()
    {
        gameClear = false;
        resultCan.SetActive(true);
        resultCan.transform.GetChild(1).GetComponent<Text>().text = "실패";
    }
    public void attack(int money)
    {
        if (shieldOn)
        {
            // 실드가 켜져있다면
            shield -= money;
            if (shield <= 0)
            {
                shield = 0;
                shieldOn = false;
                shieldImg.GetComponent<Image>().color = new Color(0, 255, 0);
            }
            updateShield();

            // 받아치기
            int rand = Random.Range(0, 2);
            if (rand == 0)
            {
                GameObject tmp = Instantiate(bubble,new Vector3(1,1) ,transform.rotation,bubbles.transform);
                tmp.transform.localPosition = getRandomPos();
                tmp.GetComponent<Bubble>().setMoney(money);
            }
        }
        else
        {
            hp -= money;
            if (hp <= 0)
            {
                // 노인 체력 끝
                hp = 0;
                success();
            }
            updateHP();
        }
    }
    // 버블 한계선 내에 임의의 위치 구하기
    public Vector3 getRandomPos()
    {
        return new Vector3(
            Random.Range(left.transform.localPosition.x + bubble.GetComponent<RectTransform>().rect.width, right.transform.localPosition.x - bubble.GetComponent<RectTransform>().rect.width),
            Random.Range(bottom.transform.localPosition.y + bubble.GetComponent<RectTransform>().rect.height, up.transform.localPosition.y - bubble.GetComponent<RectTransform>().rect.height)
            );
    }
    public void attackMoneyBtn(Button btn)
    {
        int btnIndex = int.Parse(btn.name.Substring(3, 1));
        if ( GameManager.instance.money  >= attackMoney[btnIndex])
        {
            // 공격 가능
            GameManager.instance.money -= attackMoney[btnIndex];
            moneyText.text = GameManager.instance.money.ToString();

            attack(attackMoney[btnIndex]);
        }
    }
    
    public void success()
    {
        gameTime = false;
        gameClear = true;
        resultCan.SetActive(true);
        resultCan.transform.GetChild(1).GetComponent<Text>().text = "성공";
        
    }
    public void exitScene()
    {
        SceneManager.UnloadSceneAsync(3);
        if (gameClear)
        {
            GameManager.instance.returnElderFight(true);
        }
        else
        {
            GameManager.instance.returnElderFight(false);
        }
        
        
    }

    public void healingShield(int amount)
    {
        shield += amount;
        if(shield >= shieldOrigin)
        {
            shield = shieldOrigin;
            shieldOn = true;
            shieldImg.GetComponent<Image>().color = new Color(0, 0, 255);
        }
        updateShield();
    }
    // 옵션 창
    public void option()
    {
        if(optionCan.activeSelf == false)
        {
            gameTime = false;
            optionCan.SetActive(true);
        }
        
    }
    public void optionContinueBtn()
    {
        gameTime = true;
        optionCan.SetActive(false);
    }
    public void optionExitBtn()
    {
        gameTime = false;
        SceneManager.UnloadSceneAsync(3);
        GameManager.instance.returnElderFight(false);
    }
    public void updateHP()
    {
        hpImg.GetComponent<Image>().fillAmount = (float)hp / (float)hpOrigin;
        hpText.text = hp.ToString();
    }
    public void updateShield()
    {
        shieldImg.GetComponent<Image>().fillAmount = (float)shield / (float)shieldOrigin;
        shieldText.text = shield.ToString();
    }
}
