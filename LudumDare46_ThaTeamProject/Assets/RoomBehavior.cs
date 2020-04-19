using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBehavior : MonoBehaviour
{

    public Light[] lights;
    public Light redLight;

    bool isLightRotating = false;
    float rotationSpeed = 370f;

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
