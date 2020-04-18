using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    // Start is called before the first frame update

    public Transform window;

    float criticAccDistance = 5f;
    Vector3 flyDirection;
    float speed = 0f;
    float accMin = 3f;
    float accMax = 15f;
    float acc;
    bool isAccelerating = false;
    bool isInSpace = false;

    void Start()
    {
        flyDirection = Vector3.Normalize(window.position - transform.position);
        isAccelerating = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isAccelerating)
        {
            print(Vector3.Distance(transform.position, window.position));
            acc = accMin + Mathf.Clamp(1 - Vector3.Distance(transform.position, window.position) / criticAccDistance, 0, 1) * (accMax - accMin);
            speed += acc * Time.deltaTime;
            transform.Translate(flyDirection * Time.deltaTime * speed, Space.World);
        }

        if (Vector3.Distance(transform.position, window.position) < 0.1f && !isInSpace)
        {
            isAccelerating = false;
            flyDirection = Quaternion.AngleAxis(Random.Range(-30f, 30f), Vector3.forward) * flyDirection;
            isInSpace = true;
        }

        if (isInSpace) 
        {
            transform.Translate(flyDirection * Time.deltaTime * speed, Space.World);
            transform.Rotate(0, 0, Time.deltaTime * 250f, Space.World);
        }

        if (Vector3.Distance(transform.position, Vector3.zero) > 40f)
        {
            Destroy(this.gameObject);
        }

    }

}
