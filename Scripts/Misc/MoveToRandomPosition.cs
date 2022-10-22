using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToRandomPosition : MonoBehaviour
{
    public float moveRange = 10;
    public float smoothTranslate = 1;
    public float timerTime = 3;
    private Vector3 moveToPos;

    private float timer;

    void Start()
    {
        moveToPos = transform.position;
        timer = timerTime;
        SetRandomMoveToPos();
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            timer = timerTime;
            SetRandomMoveToPos();
        }
        transform.position = Vector3.Lerp(transform.position, moveToPos, Time.deltaTime * smoothTranslate);
    }

    private void SetRandomMoveToPos()
    {
        moveToPos = new Vector3(Random.Range(-moveRange, moveRange), 2.471f, Random.Range(-moveRange, moveRange));
    }
}
