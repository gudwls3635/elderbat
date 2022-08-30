using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoyStick : MonoBehaviour
{
    public Camera main;
    public GameObject player;
    Vector2 originPos; // 조이스틱 원래 위치
    Vector3 joyStickVec; // 조이스틱 벡터
    float radius; // 조이스틱 넘어가지 못하는 거리
    bool move; // 플레이어가 움직이는 지 확인
    int moveDirection; // 플레이어가 어느 방향으로 움직이는지 u:0, d:1, l:2, r:3
    float dis;
    float minDis;
    // Start is called before the first frame update
    void Start()
    {
        originPos = transform.localPosition;
        move = false;
        minDis=0.5f;
        // 조이스틱 반지름 측정
        radius = 0.6f;

        
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.instance.gameTime == true)
        {
            if (move && dis >minDis)
            {
                if (moveDirection == 0)
                {
                    player.GetComponent<PlayerScript>().up();
                }
                else if (moveDirection == 1)
                {
                    player.GetComponent<PlayerScript>().down();
                }
                else if (moveDirection == 2)
                {
                    player.GetComponent<PlayerScript>().left();
                }
                else if (moveDirection == 3)
                {
                    player.GetComponent<PlayerScript>().right();
                }
            }
            else
            {
                player.GetComponent<PlayerScript>().stop();
            }
        }
        
    }
    public void drag(BaseEventData _data)
    {
        
        if (GameManager.instance.gameTime == true)
        {
            Vector2 pos = main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 transformPos = transform.parent.position;
            joyStickVec = ((Vector3)pos - (Vector3)transformPos).normalized;
            dis = Vector2.Distance(pos, transformPos);
            // 거리가 반지름보다 작으면 조이스틱을 현재 터치하고 있는 곳으로 이동.
            if (dis < radius)
            {
                transform.position = transformPos + (Vector2)joyStickVec * dis;
            }
            else // 거리가 반지름보다 커지면 조이스틱을 반지름의 크기만큼만 이동.
            {
                transform.position = transformPos + (Vector2)joyStickVec * radius;
            }

            // 조이스틱 벡터를 통해 방향을 정한다
            float degree = Mathf.Atan2(joyStickVec.y, joyStickVec.x) * Mathf.Rad2Deg;
            if (degree >= 45 && degree <= 135)
            {
                //up
                moveDirection = 0;
            }
            else if (degree <= -45 && degree >= -135)
            {
                //down
                moveDirection = 1;
            }
            else if (degree < 45 && degree > -45)
            {
                //right
                moveDirection = 3;
            }
            else if (degree > 135 || degree < -135)
            {
                //left
                moveDirection = 2;
            }
            
            move = true;
        }
        else
        {
            transform.localPosition = originPos;
            move = false;
        }
    }
    public void endDrag()
    {
        transform.localPosition = originPos;
        move = false;
    }
}
