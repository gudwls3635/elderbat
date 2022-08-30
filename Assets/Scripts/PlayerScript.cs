using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public static PlayerScript instance;
    public List<string> items;
    public int itemLimit = 28;
    public float speed; // player의 스피드
    public int direction; // player의 방향 u: 0, d: 1, l: 2, r: 3
    public int[] ability = new int[5]; // 이동속도 관련, 몰러,몰러,아마도,있을 것

   
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i< 5;i++)
        {
            ability[i] = 1;
        }
        //speed = 3.5f; transform.postion으로 움직일 때
        speed = 150f;
        //items.Add("bottle", 0);
        direction = 1; // 초기 방향 설정
    }
    void Awake()
    {
        if (PlayerScript.instance == null)
        {
            PlayerScript.instance = this;
        }

    }
    // Update is called once per frame
    void Update()
    {

    }
    public void stop()
    {
        transform.GetComponent<Rigidbody2D>().velocity = new Vector3(0, 0);
        transform.GetComponent<Animator>().SetBool("move", false);

    }
    public void left()
    {
        direction = 2;
        transform.GetComponent<Rigidbody2D>().velocity = new Vector3(-1, 0) * speed*ability[0] * Time.deltaTime;
        //transform.localPosition += new Vector3(-1, 0) * speed * Time.deltaTime;
        transform.GetComponent<Animator>().SetInteger("udlr", 2);
        transform.GetComponent<Animator>().SetBool("move", true);
    }
    public void right()
    {
        direction = 3;
        transform.GetComponent<Rigidbody2D>().velocity = new Vector3(1, 0) * speed * ability[0] * Time.deltaTime;
        //transform.localPosition += new Vector3(1, 0) * speed * Time.deltaTime;
        transform.GetComponent<Animator>().SetInteger("udlr", 3);
        transform.GetComponent<Animator>().SetBool("move", true);
    }
    public void down()
    {
        direction = 1;
        transform.GetComponent<Rigidbody2D>().velocity = new Vector3(0, -1) * speed * ability[0] * Time.deltaTime;
        ///transform.localPosition += new Vector3(0, -1) * speed * Time.deltaTime;
        transform.GetComponent<Animator>().SetInteger("udlr", 1);
        transform.GetComponent<Animator>().SetBool("move", true);
    }
    public void up()
    {
        direction = 0;
        transform.GetComponent<Rigidbody2D>().velocity = new Vector3(0, 1) * speed * ability[0] * Time.deltaTime;
        //transform.localPosition += new Vector3(0, 1) * speed * Time.deltaTime;
        transform.GetComponent<Animator>().SetInteger("udlr", 0);
        transform.GetComponent<Animator>().SetBool("move", true);
    }
    public int getItemNum(string item)
    {
        int n = 0;
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].Equals(item))
            {
                n += 1;
            }
        }
        return n;
    }
    public int getItemIndex(string item)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].Equals(item))
            {
                return i;
            }
        }
        return -1;
    }
    public bool itemPush(string item)
    {
        if (itemLimit == items.Count)
            return false;
        int ch = 0;
        for (int i =0; i< items.Count;i++)
        {
            if(item.CompareTo(items[i]) <0)
            {
                items.Insert(i, item);
                ch = 1;
                break;
            }
        }
        if(ch==0)
        {
            items.Add(item);
        }
        return true;
    }
    public void itemPop(int index)
    {
        items.RemoveAt(index);
    }
    public void itemAllPop(string name)
    {
        items.RemoveAll(s => s.Equals(name));
    }
}
