using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public class itemData
    {
        public string korName;
        public string name;
        public int cost;
    }
    public List<itemData> itemDatas = new List<itemData>();
    public enum itemName { bottle, minicar, wedding, gitar, cleaner, bag, book }
    public Sprite[] itemImgs = new Sprite[10];
    public class npcData
    {
        public int index;
        public int type; // 0:건물 1:인물
        public int x, y; // 크기
        public GameObject obj;
        public int level; // 어느 스테이지 부터 나오는지
        public npcData(int index, int type, int x,int y,int level)
        {
            this.index = index;
            this.type = type;
            this.x=x; this.y = y;
            this.level = level;
        }
    }
    [HideInInspector] public List<Dictionary<string, object>> dic;

    public static GameData instance;
    public int[] levelAgreeMoney ;
    public int[] levelInitMoney;
    public int[] levelBatMoney;
    int levelLimit;

    public npcData[] npcDataList ; // npc 목록
    public GameObject[] npcList ;
    public int npcCount; // npc의 종류가 총 얼마나 있는지
    public int[] stageBlock; // 구역이 언제 해금되는지
    public int stageBlockN; // 구역이 몇개있는지
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        itemDatas.Add(new itemData()
        {
            name = "bottle",
            korName = "빈 병",
            cost = 100
        });
        itemDatas.Add(new itemData()
        {
            name = "minicar",
            korName = "미니카",
            cost = 100
        });
        itemDatas.Add(new itemData()
        {
            name = "wedding",
            korName = "결혼반지",
            cost = 100
        });
        itemDatas.Add(new itemData()
        {
            name = "gitar",
            korName = "기타",
            cost = 100
        });
        itemDatas.Add(new itemData()
        {
            name = "cleaner",
            korName = "청소기",
            cost = 100
        });
        itemDatas.Add(new itemData()
        {
            name = "bag",
            korName = "가방",
            cost = 100
        });
        itemDatas.Add(new itemData()
        {
            name = "book",
            korName = "책",
            cost = 100
        });
        dic = CSVReader.Read("data");
        levelLimit = 21; // 20 + 1 
        levelAgreeMoney= new int [levelLimit];
        levelInitMoney = new int [levelLimit];
        levelBatMoney = new int[levelLimit];
        for (int i = 1; i <= 20; i++)
        {
            levelInitMoney[i] = (int)dic[i - 1]["initMoney"];
            levelBatMoney[i] = (int)dic[i - 1]["batMoney"];
            levelAgreeMoney[i] = (int)dic[i - 1]["agreeMoney"];
        }
        npcCount = 20;
        npcDataList = new npcData[npcCount];
        for (int i = 1; i <= 20; i++)
        {
            npcDataList[i - 1] = new npcData(i-1, (int)dic[i - 1]["npcType"], (int)dic[i - 1]["npcSizeX"], (int)dic[i - 1]["npcSizeY"], (int)dic[i - 1]["npcLevel"]);
        }
        stageBlockN = 15;
        stageBlock = new int[stageBlockN];
        for(int i = 1; i<=stageBlockN;i++)
        {
            stageBlock[i-1] = (int)dic[i - 1]["openLevel"];
        }
    }
    void Awake()
    {
        if (GameData.instance == null)
        {
            GameData.instance = this;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public int getCostItem(string name)
    {
        for (int i = 0; i < itemDatas.Count; i++)
        {
            if (itemDatas[i].name.Equals(name))
            {

                return itemDatas[i].cost;
            }
        }
        return -1;
    }
    public string getKorItem(string name)
    {
        for (int i = 0; i < itemDatas.Count; i++)
        {
            if (itemDatas[i].name.Equals(name))
            {
                return itemDatas[i].korName;
            }
        }
        return "";
    }
}
