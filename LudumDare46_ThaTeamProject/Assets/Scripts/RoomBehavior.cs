using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBehavior : MonoBehaviour
{

    public Light[] lights;
    public Light redLight;

    interactionManager intManager;

    GameObject floor;

    bool isLightRotating = false;
    float rotationSpeed = 370f;

    public bool isSmallFire = false;
    public bool isBigFire = false;

    public GameObject[] smallFire;
    public GameObject[] bigFire;

    public float flameLevel = 0f;

    float fireExpendTime = 5f;
    float firePropagTime = 3f;

    public GameObject[] sideRoom;

    // Start is called before the first frame update
    void Start()
    {
        intManager = GameObject.Find("InteractionManager").GetComponent<interactionManager>();
        foreach (Transform eachChild in transform)
        {
            if (eachChild.name == "Floor")
            {
                floor = eachChild.gameObject;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isLightRotating)
        {
            redLight.gameObject.transform.Rotate(Vector3.right, rotationSpeed * Time.deltaTime, Space.World);
        }

        if (isSmallFire)
        {
            flameLevel += Time.deltaTime / fireExpendTime;

            if (flameLevel >= 1f)
            {
                FireBigStart();
                isSmallFire = false;

                //perd hp
                intManager.healthBar.HPChange(-5f, transform.Find("Floor").GetComponent<BoxCollider2D>().bounds);
            }
            else if (flameLevel <= 0f)
            {
                FireSmallStop();
            }
        }

        if (isBigFire)
        {
            flameLevel = Mathf.Min(flameLevel + Time.deltaTime / firePropagTime, 2f);

            if (flameLevel >= 2f)
            {
                FirePropagate();
                flameLevel = 1f;

                //perd hp
                intManager.healthBar.HPChange(-5f, transform.Find("Floor").GetComponent<BoxCollider2D>().bounds);
            }
            else if (flameLevel <= 0f)
            {
                FireBigStop();
            }
        }


    }

    public void FireSmallStart()
    {
        isSmallFire = true;

        foreach (GameObject sFire in smallFire)
        {
            sFire.SetActive(true);
        }
    }

    public void FireSmallStop()
    {
        isSmallFire = false;
        flameLevel = 0;

        foreach (GameObject sFire in smallFire)
        {
            sFire.SetActive(false);
        }
    }

    void FireBigStart()
    {
        isBigFire = true;

        foreach (GameObject bFire in bigFire)
        {
            bFire.SetActive(true);
        }

        TurnOnRedLight();
    }

    public void FireBigStop()
    {
        isBigFire = false;
        flameLevel = 0;

        foreach (GameObject bFire in bigFire)
        {
            bFire.SetActive(false);
        }

        FireSmallStop();

        TurnOffRedLight();
    }

    void FirePropagate()
    {
        foreach (GameObject room in sideRoom)
        {
            if(room.GetComponent<RoomBehavior>().isSmallFire == false && room.GetComponent<RoomBehavior>().isBigFire == false)
                room.GetComponent<RoomBehavior>().FireSmallStart();
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
