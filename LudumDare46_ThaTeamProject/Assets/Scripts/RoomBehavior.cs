using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBehavior : MonoBehaviour
{

    public Light[] lights;
    public Light redLight;

    bool isLightRotating = false;
    float rotationSpeed = 370f;

    bool isSmallFire = false;
    public bool isBigFire = false;

    public GameObject[] smallFire;
    public GameObject[] bigFire;

    float fireExpendTime = 2f;
    float firePropagMinTime = 2f;
    float firePropagMaxTime = 4f;

    public GameObject[] sideRoom;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isLightRotating)
        {
            redLight.gameObject.transform.Rotate(Vector3.right, rotationSpeed * Time.deltaTime, Space.World);
        }
    }

    public void FireSmallStart()
    {
        isSmallFire = true;

        foreach (GameObject sFire in smallFire)
        {
            sFire.SetActive(true);
        }

        StartCoroutine(FireBigStart(fireExpendTime));
    }

    public void FireSmallStop()
    {
        isSmallFire = false;

        foreach (GameObject sFire in smallFire)
        {
            sFire.SetActive(false);
        }
    }

    IEnumerator FireBigStart(float time)
    {
        yield return new WaitForSeconds(time);

        if (isSmallFire == true)
        {
            isBigFire = true;

            foreach (GameObject bFire in bigFire)
            {
                bFire.SetActive(true);
            }

            StartCoroutine(FirePropagate(Random.Range(firePropagMinTime, firePropagMaxTime)));

            TurnOnRedLight();
        }
    }

    public void FireBigStop()
    {
        isBigFire = false;

        foreach (GameObject bFire in bigFire)
        {
            bFire.SetActive(false);
        }

        FireSmallStop();

        TurnOffRedLight();
    }

    IEnumerator FirePropagate(float time)
    {
        yield return new WaitForSeconds(time);

        if (isBigFire)
        {
            foreach (GameObject room in sideRoom)
            {
                if(room.GetComponent<RoomBehavior>().isSmallFire == false && room.GetComponent<RoomBehavior>().isBigFire == false)
                    room.GetComponent<RoomBehavior>().FireSmallStart();
            }
        }
    }

    public void TurnOnRedLight()
    {
        redLight.gameObject.SetActive(true);
        isLightRotating = true;

        foreach (Light light in lights)
        {
            light.gameObject.SetActive(false);
        }
    }

    public void TurnOffRedLight()
    {
        redLight.gameObject.SetActive(false);
        isLightRotating = false;

        foreach (Light light in lights)
        {
            light.gameObject.SetActive(true);
        }
    }
}
