using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public Sprite[] itemImgs = new Sprite[5];
    public Canvas inventoryCanvas;
    public GameObject content;
    // Start is called before the first frame update
    void Start()
    {
        itemImgs=GameData.instance.itemImgs;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void showInventory()
    {
        GameManager.instance.gameTime = false;
        inventoryCanvas.gameObject.SetActive(true);
        List<string> tmpItems = transform.GetComponent<PlayerScript>().items;

        for (int i =0; i< content.transform.childCount;i++)
        {
            if( i < tmpItems.Count)
            {
                GameData.itemName tmp = (GameData.itemName)Enum.Parse(typeof(GameData.itemName), tmpItems[i].ToString());
                content.transform.GetChild(i).GetChild(0).GetComponent<Image>().sprite = itemImgs[(int)tmp];
            }else
            {
                content.transform.GetChild(i).GetChild(0).GetComponent<Image>().sprite =null;
            }
            
        }

    }

    public void exitInventory()
    {
        inventoryCanvas.gameObject.SetActive(false);
        GameManager.instance.gameTime = true;
    }
}
