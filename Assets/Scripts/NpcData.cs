using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcData : MonoBehaviour
{
    public int index;
    public int type; // 0:건물 1:인물
    public int x, y; // 크기
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void makeData(int index, int type)
    {
        this.index = index;
        this.type = type;

    }
    public void initData()
    {
        Debug.Log(index);
        switch (index)
        {
            case 0:
                transform.GetComponent<Vending>().initData();
                break;
            case 1:
                transform.GetComponent<Lotto>().initData();
                break;
            case 2:
                transform.GetComponent<BuildingSide>().initData();
                break;
            case 3:
                transform.GetComponent<PawnShop>().initData();
                break;
            case 4:
                transform.GetComponent<Poor>().initData();
                break;
            case 5:
                transform.GetComponent<JunkMan>().initData();
                break;
            case 6:
                transform.GetComponent<Manmulsang>().initData();
                break;
            case 7:
                transform.GetComponent<PoliceStation>().initData();
                break;
            case 8:
                transform.GetComponent<Wanted>().initData();
                break;
            case 9:
                transform.GetComponent<Bag>().initData();
                break;
            case 10:
                transform.GetComponent<Child1>().initData();
                break;
            case 11:
                transform.GetComponent<Child2>().initData();
                break;
            case 12:
                transform.GetComponent<Child3>().initData();
                break;
            case 13:
                transform.GetComponent<Man1>().initData();
                break;
            case 14:
                transform.GetComponent<Man2>().initData();
                break;
            case 15:
                transform.GetComponent<Man3>().initData();
                break;
            case 16:
                transform.GetComponent<woman1>().initData();
                break;
            case 17:
                transform.GetComponent<woman2>().initData();
                break;
            case 18:
                transform.GetComponent<woman3>().initData();
                break;
            case 19:
                transform.GetComponent<Rich>().initData();
                break;
        }
    }
}
