using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DayTimer : MonoBehaviour
{
    public float timer = 5f;
    public float rotationSpeed = 180f;

    private bool isTimerRunning = true;

    void Update()
    {
        if (isTimerRunning)
        {
            timer -= Time.deltaTime;

            transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);

            if (timer <= 0f)
            {
                isTimerRunning = false;
                timer = 0f;
                Debug.Log("끝");
            }
        }
    }
}
