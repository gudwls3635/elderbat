using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class right : MonoBehaviour
{
    public GameObject elder;
    public float speed = 1f; // 노인의 스피드
    // Start is called before the first frame update
    void Start()
    {
        Physics.queriesHitTriggers = true;
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnMouseOver()
    {
        if (GameManager.instance.gameTime)
            elder.transform.localPosition += new Vector3(1, 0) * speed * Time.deltaTime;
    }
}
