using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public Camera playerCamera;
    public int state; // 0: 움직이는 상태 1:interationScreen 떠있는 상태 2: 선택지 떠있는 상태 3: 선택지 누른 상태 4: 노인과 상호작용일 때
    public Text timerText,levelText,moneyText;
    public bool gameTime;
    public float timer;
    public int level;
    public int money,previousMoney;
    public static GameManager instance;
    public GameObject elder;
    public GameObject npcs;
    public GameObject player;
    float interactionDistance;
    public GameObject selectScreen, interationScreen;
    bool eventResult;
    Vector3 playerOriginPostion;
    public GameObject topUICanvas;
    public int batNeedMoney; // 실제 방망이 가격
    public int batAgreeMoney; // 방망이 합의 가격
    public bool stageClear;

    public GameObject npcsObject; 
    public int npcCount; // npc의 종류가 총 얼마나 있는지
    public GameObject[] npcList = new GameObject[20]; // npc 프리팹
    public List<GameObject> npcListInMap; // 맵에 있는 NPC 목록
    public int[] npcInMap ; // npc가 맵에 얼마나 있는지
    public int[] npcInMapLimit; // npc가 맵에 얼마나 있는지 한계
    public bool[] npcEvent; // npc 이벤트 성공했는지
    public Vector2[] npcPos; // npc 처음 위치 저장

    public int currentNpc; // 현재 몇번째 npc 인지 저장
    public Queue<GameObject> interactionSelectBtnPool = new Queue<GameObject>();
    public GameObject npcSelectBtns; // 선택지 버튼묶음
    public GameObject interactionSelectBtn; // 선택지 버튼

    public GameObject wall; // 맵 구역들의 collide

    // npc 관련 항목 나중에 csv 로 정리
    public string[] npcName ;
    public string[][] npcConversation= new string[20][];
    //public string[,] npcSelectItem ;
    
    // Start is called before the first frame update
    void Start()
    {
        interactionDistance = 2f;
        npcCount = GameData.instance.npcCount;
        npcInMap = new int[npcCount];
        npcInMapLimit = new int[npcCount];
        npcEvent = new bool[npcCount]; // npc 이벤트 성공했는지
        npcPos = new Vector2[npcCount]; // npc 처음 위치 저장
        npcName = new string[] { "자판기", "복권 판매기", "건물 사이" ,"전당포","거지","고물상","만물상","파출소","수배범","가방","아이1","아이2","아이3","남자1", "남자2", "남자3", "여자1", "여자2", "여자3" ,"부자"};
        npcConversation = new string[20][] { new string[] { "자판기가 있다..." }, new string[] { "복권 판매점이다. 복권을 살까?" }, new string[] { "빈 병이 보인다." }, new string[] { "전당포가 있다. 물건을 맡겨볼까?" }, new string[] { "거지가 구걸을 하고 있다" } , new string[] { "고물상이 있다. 병을 팔 수 있는 것 같다." }, new string[] { "오늘은 을/를 판다..." }, 
            new string[] { "파출소 앞이다. 무엇을 할까?"}, new string[] { "... 사람 잘못 보셨습니다.", "... 사람 잘못 보셨다니까요." }, new string[] {"가방이 떨어져 있다. 누가 잃어버린걸까?" }, new string[] { "이동속도는 올릴수록 빨라져요!", "능력치2은 2와 관련되어 있어요", "능력치3은 3과 관련되어 있어요" }, new string[] { "빈 병을 더 모아야 하는데... 어디 없을까 ?"}, new string[] { "미니카가 가지고 싶어... 근데 너무 비싸", "우와! 미니카다! 저 아저씨 부럽다..." }, new string[] { "만물상은 물건을 팔죠.하루에 하나만 판답니다.", "전당포엔 물건을 맡기고 돈을 받을 수 있어요. 찾을 땐 돈이 드니 조심하세요" }, 
            new string[] {"결혼반지가 대체 어디갔지? 큰일났네..." }, new string[] { "나도 기타만 있으면 명가수가 될 수 있다고! 기타만 있으면.."}, new string[] { "파출소엔 분실물을 맡길 수 있어요!", "빈 병을 모았다면 고물상에 가보세요!"}, new string[] { "시험 공부는 너무 힘들어. 책 값도 장난이 아니네…"}, new string[] { "청소기가 필요한데 큰일났네… 비쌀텐데"}, new string[] { "어머.. 명품 가방인데 대체 어디 갔을까?" } };
        /*npcSelectItem = new string[,] { { "뒤져본다 (3초)", "그만둔다 (0초)" ,""},
                                        { "100원짜리를 산다 (5초)","1000원짜리를 산다 (5초)","그만둔다 (0초)"},
                                        { "병을 줍는다 (2초)", "그만둔다 (0초)" ,""},
                                        { "물건을 맡긴다 (3초)", "물건을 찾는다 (3초)" ,"그만둔다 (0초)"},
                                        { "돈을 들고 달아난다. (2초)", "적선한다 (2초)" ,"그만둔다 (0초)"},
                                        { "빈 병을 판다 (3초)", "아르바이트를 한다 (5초)" ,"그만둔다 (0초)"},
                                        { "을 산다 (2초)", "아르바이트를 한다 (5초)" ,"그만둔다 (0초)"},
                                        { "수배자 리스트를 확인한다. (2초)", "분실물을 맡긴다. (3초)" ,"그만둔다 (0초)"},
                                        { "잡아서 파출소에 넘긴다 (5초)", "그만둔다 (0초)" ,""},
                                        { "가진다 (2초)", "경찰서에 맡긴다 (5초)" ,"그만둔다 (0초)"},
                                        { "", "" ,""},
                                        { "가진 빈 병을 모두 건넨다(2초)", "그만둔다 (0초)" ,""},
                                        { "미니카를 건넨다 (2초)", "그만둔다 (0초)" ,""},
                                        { "", "" ,""},
                                        { "혹시 이건가요? (2초)", "그만둔다 (0초)" ,""},
                                        { "기타를 건넨다 (4초)", "그만둔다 (0초)" ,""},
                                        { "", "" ,""},
                                        { "책을 건네준다 (4초)", "그만둔다 (0초)" ,""},
                                        { "청소기를 건넨다 (3초)", "그만둔다 (0초)" ,""},
                                        { "가방을 건넨다 (4초)", "그만둔다 (0초)" ,""}};*/

        initInteractionSelectBtns();
        level = 1;
        DontDestroyOnLoad(this.gameObject);
        playerOriginPostion = player.transform.position;
        setGame();

        //테스트
        PlayerScript.instance.itemPush("gitar");
        PlayerScript.instance.itemPush("bag");
        PlayerScript.instance.itemPush("cleaner");
        PlayerScript.instance.itemPush("book");
    }
    void Awake()
    {
        if (GameManager.instance == null)
        {
            GameManager.instance = this;
        }

    }
    // Update is called once per frame
    void Update()
    {
        timeStart();
        checkClick();

     }
    
    private GameObject createNewInteractionBtn()
    {
        GameObject newObj = Instantiate(interactionSelectBtn);
        newObj.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { selectBtn(newObj.GetComponent<UnityEngine.UI.Button>()); } );
        newObj.SetActive(false);
        return newObj;
    }
    public void initInteractionSelectBtns()
    {
        for (int i = 0; i < 10; i++)
        {
            interactionSelectBtnPool.Enqueue(createNewInteractionBtn());
        }
    }
    public GameObject getInteractionSelectBtn()
    {
        if (interactionSelectBtnPool.Count > 0)
        {
            GameObject obj = interactionSelectBtnPool.Dequeue();
            obj.transform.SetParent(npcSelectBtns.transform);
            return obj;
        }else
        {
            return null;
        }
    }
    public void returnInteractionSelectBtn(GameObject obj)
    {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(transform.parent);
        interactionSelectBtnPool.Enqueue(obj);
    }
    void checkClick()
    {
        if (Input.GetMouseButtonDown(0) && SceneManager.GetActiveScene().name.Equals("GameScene"))
        {
            if(state==4) // 노인과 interation 성공 후
            {
                /*
                if(money >= batAgreeMoney)
                {
                    interationScreen.SetActive(false);
                    money -= batAgreeMoney;
                    state = 0;
                    stageClearFunc();
                }else
                {
                    // 오차 대사
                    interationScreen.transform.GetChild(4).GetComponent<Text>().text = "오차 범위 대사"; // Text
                    state = 5;
                }*/
                SceneManager.LoadScene("ElderFightScene", LoadSceneMode.Additive);
            }
            else if(state==5)
            {
                // 오차 범위 대사 닫기
                state = 0;
                interationScreen.SetActive(false);
                gameTime = true;
                
            }else if(state== 6) // 노인과 fight 후
            {
                if(stageClear)
                {
                    // fight 결과 성공
                    state = 0;
                    interationScreen.SetActive(false);
                    stageClearFunc();
                }
                else
                {
                    // fight 결과 실패
                    state = 0;
                    interationScreen.SetActive(false);
                    gameTime = true;
                }
            }
        }
    }
    // 선택지 버튼 위치 조정
    public void adjustSelectBtn(int count)
    {
        if (count % 2 == 0) // 선택지가 짝수개 일 떄
        {
            for (int j = 0; j <= (count / 2 - 1); j++)
            {
                npcSelectBtns.transform.GetChild(j).localPosition = new Vector3(0, 130 + (j - (count / 2 - 1)) * 260 * -1);
                npcSelectBtns.transform.GetChild(j).gameObject.SetActive(true);
            }
            for (int j = count / 2; j <= count - 1; j++)
            {
                npcSelectBtns.transform.GetChild(j).localPosition = new Vector3(0, -130 + (j - count / 2) * 260 * -1);
                npcSelectBtns.transform.GetChild(j).gameObject.SetActive(true);
            }
        }
        else // 홀수개 일 때
        {
            for (int i = 0; i < count; i++)
            {
                npcSelectBtns.transform.GetChild(i).localPosition = new Vector3(0, (i - count / 2) * 260 * -1);
                npcSelectBtns.transform.GetChild(i).gameObject.SetActive(true);
            }
        }

        selectScreen.SetActive(true);
    }
    // 선택지 버튼 생성 및 이름들 설정
    public int makeSelectBtn(string[] npcSelectItem)
    {
        /*int count = 0;  이전에 게임매니저에서 선택지를 만들었을 때
        for (int i = 0; i < npcSelectItem.GetLength(1); i++)
        {
            if (!npcSelectItem[currentNpc, i].Equals(""))
            {
                count += 1;
                GameObject obj = getInteractionSelectBtn();
                obj.name = "selectBtn" + i;
                obj.GetComponent<RectTransform>().localScale = new Vector3(1, 1);
                obj.transform.GetChild(0).GetComponent<Text>().text = npcSelectItem[currentNpc, i];
            }
        }
        return count;*/
        int count = 0;
        for (int i = 0; i < npcSelectItem.Length; i++)
        {
            if (!npcSelectItem[ i].Equals(""))
            {
                count += 1;
                GameObject obj = getInteractionSelectBtn();
                obj.name = "selectBtn" + i;
                obj.GetComponent<RectTransform>().localScale = new Vector3(1, 1);
                obj.transform.GetChild(0).GetComponent<Text>().text = npcSelectItem[ i];
            }
        }
        return count;
    }
    // 스테이지 생성 함수
    public void setGame()
    {

        createMap();

        timer = 180f;  // 스테이지 타이머 설정
        // 실제 방망이 가격 설정
        batNeedMoney = GameData.instance.levelBatMoney[level];
        /*for ( int i =2;i<=level;i++)
        {
            if ((i - 1) % 2 == 1)
            {
                batNeedMoney += 500 * ((i - 1) / 2 + 1);
            }
            else
            {
                batNeedMoney += 500 * ((i - 1) / 2);
            }
        }*/

        //batAgreeMoney = (int)UnityEngine.Random.Range(batNeedMoney * 1.5f, batNeedMoney * 1.5f * 1.5f);  // 방망이 합의 가격 설정
        batAgreeMoney = GameData.instance.levelAgreeMoney[level];

        if(level ==1)
        {
            money += GameData.instance.levelInitMoney[level]; // 초기 자금 수령
        }
        else
        {
            if (stageClear)
            {
                money += GameData.instance.levelInitMoney[level]; // 초기 자금 수령
            }
        }
        
        previousMoney = money; // 스테이지 시작할 때 자금

        // 플레이어 위치 조정
        player.transform.position = playerOriginPostion;
        player.GetComponent<Animator>().SetInteger("udlr", 1);
        playerCamera.transform.position = player.transform.position;

        // UI 업데이트
        state = 0;
        timerText.text = timer.ToString();
        levelText.text = "Lv " + level.ToString();
        moneyText.text = money.ToString();
        
        npcList[9].transform.position = npcPos[9]; // 가방 원래 위치로
                
        gameTime = true;
        Debug.Log(npcListInMap[npcListInMap.Count-1]);
    }
    public int[,] board = new int[54, 58];
    // NPC 위치 지정
    public int[] buildingCheck;
    public int[] personCheck;
    public List<int> mapBlock; // 현재 열린 구역들
    public Dictionary<GameObject,int> npcDistrict; // npc들이 어느 구역에 있는지
    public void createMap()
    {
        // 생성한 NPC 다 삭제
        for(int i = 0; i< npcListInMap.Count; i++)
        {
            Destroy(npcListInMap[i]);
        }
        npcListInMap = new List<GameObject>();

        npcDistrict = new Dictionary<GameObject, int>();
        npcInMap = new int[npcCount];
        npcInMapLimit = new int[npcCount];
        for (int i = 0; i < npcCount; i++)
            npcInMapLimit[i] = 1;
        npcInMapLimit[1] = 4;
        // x:53 y:57 구역 0 ~ 14, 12: 초기 구역
        board = new int[54, 58];
        for(int i = 0; i<54;i++)
        {
            for(int j = 0; j<58;j++)
            {
                board[i, j] = -1;
            }
        }
        List<int> npcCreateKind = new List<int>();
        // level에 따라 생성할 npc 종류
        for (int i = 0; i < GameData.instance.npcCount; i++)
        {
            if (level >= GameData.instance.npcDataList[i].level)
            {
                if(i== 8)// 수배범 생성일 때
                {
                    if (npcEvent[7]) // 파출소에서 수배자 리스트 봤을 시
                    {
                        int rand = UnityEngine.Random.Range(1, 51);
                        if (rand < npcList[8].GetComponent<Wanted>().wantedCreatePer)
                        {
                            npcCreateKind.Add(i);
                        }
                    }
                }else if(i==19) // 부자 생성일 때
                {
                    // 가방 경찰서에 맡겼을 때 부자 출현
                    if (npcEvent[9])
                        npcCreateKind.Add(i);
                }else
                {
                    npcCreateKind.Add(i);
                }
            }
                
        }
        // level에 따라 해금 되는 구역
        int[,] mapBoard = new int[3, 5];
        mapBlock = new List<int>();
        for(int i = 0; i< GameData.instance.stageBlockN;i++)
        {
            if (GameData.instance.stageBlock[i] <= level)
            {
                mapBlock.Add(i);
                mapBoard[i/5,i%5]= 1;
            }
        }
        int[,] visit = new int[3, 5];
        unlockStageLimit(mapBoard,2,2, ref visit);
        // 건물 npc type:0, 인물 npc type:1
        buildingCheck = new int[15];
        personCheck = new int[15];
        // 0: 자판기는 위치 고정
        board[24, 49] = 0;  board[28,53]= -2; board[28, 46] = -3;
        npcCreateKind.Remove(0); npcListInMap.Add(Instantiate(npcList[0], new Vector3(24, 49), Quaternion.identity));
        npcListInMap[0].GetComponent<NpcData>().initData();
        buildingCheck[12] = 1; personCheck[12] = 1; npcDistrict.Add(npcListInMap[0],12);
        Debug.Log(npcListInMap[0]);
        while (true)
        {
            int ch = 0;
            for (int i = 0; i < npcCreateKind.Count; i++)
            {
                int limitCh = 0;
                int buildingType = GameData.instance.npcDataList[npcCreateKind[i]].type;
                if (buildingType == 0) // 만들 npc가 건물일 때 구역의 건물부분이 남아있는지
                {
                    for (int j = 0; j < mapBlock.Count; j++)
                    {
                        if (buildingCheck[mapBlock[j]] == 0)
                        {
                            limitCh = 1;
                            break;
                        }
                    }
                }
                else
                {
                    // 만들 npc가 인물일 때 구역의 건물부분이 남아있는지
                    for (int j = 0; j < mapBlock.Count; j++)
                    {
                        if (personCheck[mapBlock[j]] == 0)
                        {
                            limitCh = 1;
                            break;
                        }
                    }
                }
                if(limitCh==0) // 해당 종류의 구역의 공간이 없어 npc를 못 만들 경우
                {
                    npcCreateKind.RemoveAt(i);
                    i -= 1;
                }
            }
            if (npcCreateKind.Count == 0) // 더이상 만들 npc가 없을 때
                ch = 1;
            if (ch == 1)
                break;
            int randIndex = UnityEngine.Random.Range(0, npcCreateKind.Count); // NPC 선택
            GameData.npcData npc = GameData.instance.npcDataList[npcCreateKind[randIndex]];
            int x, y, district=-1;
            if(npc.type==0) // 만들 npc 가 건물일 때
            {
                if(npc.index==1 && npcInMap[npcCreateKind[randIndex]]==0) // 맨 처음 복권상점일 때
                {
                    x = 32;  y = 49; board[32, 49] = 1;
                }
                else
                {
                    int rand;
                    do
                    {
                        rand = UnityEngine.Random.Range(0, mapBlock.Count);
                    } while (buildingCheck[mapBlock[rand]] == 1);
                    district = mapBlock[rand];
                    buildingCheck[district] = 1;
                    x = UnityEngine.Random.Range(district % 5 * 11 + npc.x / 2, district % 5 * 11 + 10 - (npc.x / 2)); // 가로 구역당 10칸 중간에 1칸 짝수 크기 건물인 경우 
                    y = UnityEngine.Random.Range(district / 5 * 19 + npc.y / 2, district / 5 * 19 + 9 - (npc.y / 2));
                }
            }
            else // 인물 일 때
            {
                int rand;
                do
                {
                    rand = UnityEngine.Random.Range(0, mapBlock.Count);
                } while (personCheck[mapBlock[rand]] == 1);
                district = mapBlock[rand];
                personCheck[district] = 1;
                x = UnityEngine.Random.Range(district % 5 * 11 + npc.x / 2, district % 5 * 11 + 10 - (npc.x / 2)); // 가로 구역당 10칸 중간에 1칸 짝수 크기 건물인 경우 
                y = UnityEngine.Random.Range(district / 5 * 19+10 + npc.y / 2, district / 5 * 19+10 + 9 - (npc.y / 2));
            }
            npcListInMap.Add(Instantiate(npcList[npcCreateKind[randIndex]], new Vector3(x, y), Quaternion.identity)); // npc 생성
            npcListInMap[npcListInMap.Count-1].GetComponent<NpcData>().initData(); // npc 초기값
            npcDistrict.Add(npcListInMap[npcListInMap.Count - 1], district);
            npcInMap[npcCreateKind[randIndex]] += 1;
            if(npcInMap[npcCreateKind[randIndex]] == npcInMapLimit[npcCreateKind[randIndex]])
            {
                npcCreateKind.RemoveAt(randIndex);
            }
        }
    }
    // 스테이지 영역 해금하기
    void unlockStageLimit(int [,] mapBoard,int r,int c,ref int [,] visit)
    {
        // 0 up 1 down 2 left 3 right 
        visit[r, c] = 1;
        int[] x = new int[] {0,0,-1,1 };
        int[] y = new int[] { 1,-1, 0, 0 };
        for (int i = 0; i < 4;i++)
        {
            if(r + y[i] >=0 && r + y[i] <3 && c + x[i]  >=0 && c + x[i]  < 5&& mapBoard[r + y[i], c + x[i]] ==1)
            {
                wall.transform.GetChild(r * 5 + c).transform.GetChild(i).gameObject.SetActive(false);
                if(visit[r + y[i], c + x[i]] ==0)
                    unlockStageLimit(mapBoard, r + y[i], c + x[i],ref visit);
            }
        }
    }
    // 돈 UI 업데이트
    public void updateMoney(int plusMoney)
    {
        money += plusMoney;
        moneyText.text = money.ToString();
    }
    // 타이머 함수
    float secTimer=0f;
    void timeStart()
    {
        if(gameTime==true)
        {
            if(timer >0)
            {
                secTimer += Time.deltaTime;
                if(secTimer >= 1)
                {
                    secTimer = 0;
                    timer -= 1;
                }
                timerText.text = timer.ToString();
            }
            else
            {
                // 시간이 다 되었을 때
                timerText.text = "0";
                gameTime = false;
                timeOver();
            }
            
            
        }
        
    }
    int npcListInMapIndex;
    // 선택 버튼 눌렀을 때 함수
    public void clickSelectBtn()
    {
        if(gameTime == true)
        {
            // npc들과 상호작용 거리인지
            int index = -1;
            float min = 999999f;
            npcListInMapIndex=-1;
            Debug.Log(npcListInMap.Count);
            for (int i = 0; i < npcListInMap.Count; i++)
            {
                if (npcListInMap[i].activeSelf == true)
                {
                    Debug.Log(npcListInMap[i].name);
                    float tmp = (npcListInMap[i].transform.position+ new Vector3(npcListInMap[i].GetComponent<RectTransform>().rect.xMax, npcListInMap[i].GetComponent<RectTransform>().rect.yMax) - player.transform.position).sqrMagnitude;
                    if (tmp <= interactionDistance && tmp < min)
                    {
                        min = tmp;
                        index = npcListInMap[i].GetComponent<NpcData>().index;
                        currentNpc = index; // 현재 상호작용하는 npc 저장
                        npcListInMapIndex = i;
                    }
                    tmp = (npcListInMap[i].transform.position + new Vector3(npcListInMap[i].GetComponent<RectTransform>().rect.xMin, npcListInMap[i].GetComponent<RectTransform>().rect.yMax) - player.transform.position).sqrMagnitude;
                    if (tmp <= interactionDistance && tmp < min)
                    {
                        min = tmp;
                        index = npcListInMap[i].GetComponent<NpcData>().index;
                        currentNpc = index; // 현재 상호작용하는 npc 저장
                        npcListInMapIndex = i;
                    }
                    tmp = (npcListInMap[i].transform.position + new Vector3(npcListInMap[i].GetComponent<RectTransform>().rect.xMax, npcListInMap[i].GetComponent<RectTransform>().rect.yMin) - player.transform.position).sqrMagnitude;
                    if (tmp <= interactionDistance && tmp < min)
                    {
                        min = tmp;
                        index = npcListInMap[i].GetComponent<NpcData>().index;
                        currentNpc = index; // 현재 상호작용하는 npc 저장
                        npcListInMapIndex = i;
                    }
                    tmp = (npcListInMap[i].transform.position + new Vector3(npcListInMap[i].GetComponent<RectTransform>().rect.xMin, npcListInMap[i].GetComponent<RectTransform>().rect.yMin) - player.transform.position).sqrMagnitude;
                    if (tmp <= interactionDistance && tmp < min)
                    {
                        min = tmp;
                        index = npcListInMap[i].GetComponent<NpcData>().index;
                        currentNpc = index; // 현재 상호작용하는 npc 저장
                        npcListInMapIndex = i;
                    }
                }
                
            }
            if (index != -1)
            {
                if(index == 8) // 수배범 일 때
                {
                    if(npcListInMap[npcListInMapIndex].GetComponent<Wanted>().stack < 3) // 스택 3번 이상일 때
                    {
                        startInteration(index, npcConversation[index][0]);
                    }
                    else
                    {
                        startInteration(index, npcConversation[index][1]);
                    }
                }else if(index == 10) // 아이1일 때
                {
                    startInteration(index, npcConversation[index][UnityEngine.Random.Range(0,npcConversation[index].Length)]);
                }else if(index == 12) // 아이3일 때
                {
                    if( PlayerScript.instance.getItemIndex("minicar")== -1 ) // 미니카 없을 때
                    {
                        startInteration(index, npcConversation[index][0]);
                    }
                    else
                    {
                        startInteration(index, npcConversation[index][1]);
                    }
                }else if(index == 13) // 남자1일 때
                {
                    startInteration(index, npcConversation[index][UnityEngine.Random.Range(0, npcConversation[index].Length)]);
                }
                else if (index == 16) // 여자1일 때
                {
                    startInteration(index, npcConversation[index][UnityEngine.Random.Range(0, npcConversation[index].Length)]);
                }
                else
                {
                    startInteration(index, npcConversation[index][0]);
                }
                
            }else
            {
                // 노인과 상호작용 거리인지
                if((elder.transform.position + new Vector3(elder.GetComponent<RectTransform>().rect.xMin, elder.GetComponent<RectTransform>().rect.yMax)  - player.transform.position).sqrMagnitude <= interactionDistance)
                {
                    currentNpc = -1; // 노인과 상호작용한다
                    startInteration(-1, "");
                }else if ((elder.transform.position + new Vector3(elder.GetComponent<RectTransform>().rect.xMax, elder.GetComponent<RectTransform>().rect.yMax) - player.transform.position).sqrMagnitude <= interactionDistance)
                {
                    currentNpc = -1; // 노인과 상호작용한다
                    startInteration(-1, "");
                }else if ((elder.transform.position + new Vector3(elder.GetComponent<RectTransform>().rect.xMin, elder.GetComponent<RectTransform>().rect.yMin) - player.transform.position).sqrMagnitude <= interactionDistance)
                {
                    currentNpc = -1; // 노인과 상호작용한다
                    startInteration(-1, "");
                }else if ((elder.transform.position + new Vector3(elder.GetComponent<RectTransform>().rect.xMax, elder.GetComponent<RectTransform>().rect.yMin) - player.transform.position).sqrMagnitude <= interactionDistance)
                {
                    currentNpc = -1; // 노인과 상호작용한다
                    startInteration(-1, "");
                }
            }
        }
        
    }
    // NPC와 상호작용이 되었을 때
    public void startInteration(int index,string conversation)
    {

        gameTime = false;
        interationScreen.SetActive(true);
        if (index > -1)
        {
            // npc와 상호작용일 때
            interationScreen.transform.GetChild(3).GetComponent<Text>().text = npcName[index];// npc 이름
            interationScreen.transform.GetChild(4).GetComponent<Text>().text = conversation;// npc 첫 대사
            switch (npcName[index])
            {
                case "자판기":
                    npcListInMap[npcListInMapIndex].GetComponent<Vending>().enabled = true;
                    npcListInMap[npcListInMapIndex].GetComponent<Vending>().interactionNpc();
                    break;
                case "복권 판매기":
                    npcListInMap[npcListInMapIndex].GetComponent<Lotto>().enabled = true;
                    npcListInMap[npcListInMapIndex].GetComponent<Lotto>().interactionNpc();
                    break;
                case "건물 사이":
                    npcListInMap[npcListInMapIndex].GetComponent<BuildingSide>().enabled = true;
                    npcListInMap[npcListInMapIndex].GetComponent<BuildingSide>().interactionNpc();
                    break;
                case "전당포":
                    npcListInMap[npcListInMapIndex].GetComponent<PawnShop>().enabled = true;
                    npcListInMap[npcListInMapIndex].GetComponent<PawnShop>().interactionNpc();
                    break;
                case "거지":
                    npcListInMap[npcListInMapIndex].GetComponent<Poor>().enabled = true;
                    npcListInMap[npcListInMapIndex].GetComponent<Poor>().interactionNpc();
                    break;
                case "고물상":
                    npcListInMap[npcListInMapIndex].GetComponent<JunkMan>().enabled = true;
                    npcListInMap[npcListInMapIndex].GetComponent<JunkMan>().interactionNpc();
                    break;
                case "만물상":
                    npcListInMap[npcListInMapIndex].GetComponent<Manmulsang>().enabled = true;
                    npcListInMap[npcListInMapIndex].GetComponent<Manmulsang>().interactionNpc();
                    break;
                case "파출소":
                    npcListInMap[npcListInMapIndex].GetComponent<PoliceStation>().enabled = true;
                    npcListInMap[npcListInMapIndex].GetComponent<PoliceStation>().interactionNpc();
                    break;
                case "수배범":
                    npcListInMap[npcListInMapIndex].GetComponent<Wanted>().enabled = true;
                    npcListInMap[npcListInMapIndex].GetComponent<Wanted>().interactionNpc();
                    break;
                case "가방":
                    npcListInMap[npcListInMapIndex].GetComponent<Bag>().enabled = true;
                    npcListInMap[npcListInMapIndex].GetComponent<Bag>().interactionNpc();
                    break;
                case "아이1":
                    npcListInMap[npcListInMapIndex].GetComponent<Child1>().enabled = true;
                    npcListInMap[npcListInMapIndex].GetComponent<Child1>().interactionNpc();
                    break;
                case "아이2":
                    npcListInMap[npcListInMapIndex].GetComponent<Child2>().enabled = true;
                    npcListInMap[npcListInMapIndex].GetComponent<Child2>().interactionNpc();
                    break;
                case "아이3":
                    npcListInMap[npcListInMapIndex].GetComponent<Child3>().enabled = true;
                    npcListInMap[npcListInMapIndex].GetComponent<Child3>().interactionNpc();
                    break;
                case "남자1":
                    npcListInMap[npcListInMapIndex].GetComponent<Man1>().enabled = true;
                    npcListInMap[npcListInMapIndex].GetComponent<Man1>().interactionNpc();
                    break;
                case "남자2":
                    npcListInMap[npcListInMapIndex].GetComponent<Man2>().enabled = true;
                    npcListInMap[npcListInMapIndex].GetComponent<Man2>().interactionNpc();
                    break;
                case "남자3":
                    npcListInMap[npcListInMapIndex].GetComponent<Man3>().enabled = true;
                    npcListInMap[npcListInMapIndex].GetComponent<Man3>().interactionNpc();
                    break;
                case "여자1":
                    npcListInMap[npcListInMapIndex].GetComponent<woman1>().enabled = true;
                    npcListInMap[npcListInMapIndex].GetComponent<woman1>().interactionNpc();
                    break;
                case "여자2":
                    npcListInMap[npcListInMapIndex].GetComponent<woman2>().enabled = true;
                    npcListInMap[npcListInMapIndex].GetComponent<woman2>().interactionNpc();
                    break;
                case "여자3":
                    npcListInMap[npcListInMapIndex].GetComponent<woman3>().enabled = true;
                    npcListInMap[npcListInMapIndex].GetComponent<woman3>().interactionNpc();
                    break;
                case "부자":
                    npcListInMap[npcListInMapIndex].GetComponent<Rich>().enabled = true;
                    npcListInMap[npcListInMapIndex].GetComponent<Rich>().interactionNpc();
                    break;
                
            }
            //state = 1;
        }else
        {
            // 노인과 상호작용일 때
            interationScreen.transform.GetChild(3).GetComponent<Text>().text = "노인";// Name
            interationScreen.transform.GetChild(4).GetComponent<Text>().text = "대사";// Text
            state = 4;
        }
    }
    // 선택지 버튼 눌렀을 때 함수
    public void selectBtn(UnityEngine.UI.Button btn)
    {
        // 버튼 인덱스 가져오기 name: selectBtn0
        int index = Convert.ToInt32(btn.name.Substring(9));
        switch (currentNpc)
        {
            case 0:
                npcListInMap[npcListInMapIndex].GetComponent<Vending>().selectBtn(currentNpc,index);
                break;
            case 1:
                npcListInMap[npcListInMapIndex].GetComponent<Lotto>().selectBtn(currentNpc, index);
                break;
            case 2:
                npcListInMap[npcListInMapIndex].GetComponent<BuildingSide>().selectBtn(currentNpc, index);
                break;
            case 3:
                npcListInMap[npcListInMapIndex].GetComponent<PawnShop>().selectBtn(currentNpc, index);
                break;
            case 4:
                npcListInMap[npcListInMapIndex].GetComponent<Poor>().selectBtn(currentNpc, index);
                break;
            case 5:
                npcListInMap[npcListInMapIndex].GetComponent<JunkMan>().selectBtn(currentNpc, index);
                break;
            case 6:
                npcListInMap[npcListInMapIndex].GetComponent<Manmulsang>().selectBtn(currentNpc, index);
                break;
            case 7:
                npcListInMap[npcListInMapIndex].GetComponent<PoliceStation>().selectBtn(currentNpc, index);
                break;
            case 8:
                npcListInMap[npcListInMapIndex].GetComponent<Wanted>().selectBtn(currentNpc, index);
                break;
            case 9:
                npcListInMap[npcListInMapIndex].GetComponent<Bag>().selectBtn(currentNpc, index);
                break;
            case 10:
                npcListInMap[npcListInMapIndex].GetComponent<Child1>().selectBtn(currentNpc, index);
                break;
            case 11:
                npcListInMap[npcListInMapIndex].GetComponent<Child2>().selectBtn(currentNpc, index);
                break;
            case 12:
                npcListInMap[npcListInMapIndex].GetComponent<Child3>().selectBtn(currentNpc, index);
                break;
            case 13:
                npcListInMap[npcListInMapIndex].GetComponent<Man1>().selectBtn(currentNpc, index);
                break;
            case 14:
                npcListInMap[npcListInMapIndex].GetComponent<Man2>().selectBtn(currentNpc, index);
                break;
            case 15:
                npcListInMap[npcListInMapIndex].GetComponent<Man3>().selectBtn(currentNpc, index);
                break;
            case 16:
                npcListInMap[npcListInMapIndex].GetComponent<woman1>().selectBtn(currentNpc, index);
                break;
            case 17:
                npcListInMap[npcListInMapIndex].GetComponent<woman2>().selectBtn(currentNpc, index);
                break;
            case 18:
                npcListInMap[npcListInMapIndex].GetComponent<woman3>().selectBtn(currentNpc, index);
                break;
            case 19:
                npcListInMap[npcListInMapIndex].GetComponent<Rich>().selectBtn(currentNpc, index);
                break;
        }
    }
    // 선택지 화면 닫기
    public void exitSelectBtn()
    {
        while (npcSelectBtns.transform.childCount > 0)
        {
            returnInteractionSelectBtn(npcSelectBtns.transform.GetChild(0).gameObject);
        }
        selectScreen.SetActive(false);
    }
    public void timeOver()
    {
        topUICanvas.SetActive(true);
       /* if(money >= batNeedMoney)
        {
            // 방망이 살 수 있을 때
            topUICanvas.transform.GetChild(1).GetComponent<Text>().text = "성공"; // 텍스트
            stageClear = true;
            money = 0;
        }else
        {
            // 방망이 못 살 때
            topUICanvas.transform.GetChild(1).GetComponent<Text>().text = "실패"; // 텍스트
            stageClear = false;
            //money = previousMoney; 이전 돈으로 돌아가게 했는데 레벨1로 돌아가야 할 듯
        }*/
        topUICanvas.transform.GetChild(1).GetComponent<Text>().text = "실패"; // 텍스트
        stageClear = false;
    }
    public void stageClearFunc()
    {
        topUICanvas.SetActive(true);
        topUICanvas.transform.GetChild(1).GetComponent<Text>().text = "성공"; // 텍스트
        stageClear = true;
    }
    public void clickNextBtn()
    {
        topUICanvas.SetActive(false);
        if (stageClear)
        {
            // 스테이지를 깼을 때
            level += 1;
            if (level > 20)
                level = 20;
            SceneManager.LoadScene("StageRestScene", LoadSceneMode.Additive);
        }
        else
        {
            // 스테이지를 실패 했을 때
            SceneManager.LoadScene("StartScene");
            Destroy(transform.gameObject);
            // 레벨 1로 돌아가는 것
            /*level = 1; 
            money = 0;
            setGame();*/
        }
    }
    public void reStart()
    {
        setGame();
    }

    public void returnElderFight(bool result)
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("GameScene"));
        updateMoney(0);
        if (result)
        {
            // 승리
            stageClear = true;
            interationScreen.transform.GetChild(4).GetComponent<Text>().text = "성공"; // Text
        }
        else
        {
            // 패배
            stageClear = false;
            interationScreen.transform.GetChild(4).GetComponent<Text>().text = "실패"; // Text
        }
        state = 6;
    }
    // 테스트 버튼
    public void testBtn()
    {
        board[24, 49] = -1; board[32, 49] = 1; board[28, 53] = -2; board[28, 46] = -3;
        player.transform.localPosition = new Vector2(28, 46);
        for(int i = 0; i< npcListInMap.Count; i++)
        {
            if( !(npcListInMap[i].GetComponent<NpcData>().index== 1 || (npcListInMap[i].GetComponent<NpcData>().index == 0 && npcListInMap[i].GetComponent<Lotto>().id==1)))
            {
                if (i < 8)
                    npcListInMap[i].transform.localPosition = new Vector2(5 + i * 9, 43);
                else
                    npcList[i].transform.localPosition = new Vector2(8 + i * 2, 38);
            }
            
        }   
    }

}
