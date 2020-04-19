using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireExctinguisher : MonoBehaviour
{
    public GameObject smokeChild;
    GameObject SmokingRoom;

    bool isSmoking = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, -1f);

        if (Input.GetKey(KeyCode.Mouse0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), -Vector2.up, 1, LayerMask.GetMask("RoomFloor"));

            if (hit.collider != null)
            {
                if ((hit.transform.GetComponentInParent<RoomBehavior>().isBigFire || hit.transform.GetComponentInParent<RoomBehavior>().isSmallFire) && !isSmoking)
                {
                    isSmoking = true;
                    smokeChild.GetComponent<ParticleSystem>().Play();
                    SmokingRoom = hit.transform.parent.gameObject;
                }
            }
            else
            {
                isSmoking = false;
                smokeChild.GetComponent<ParticleSystem>().Stop();
                SmokingRoom = null;
            }
        }
        else
        {
            isSmoking = false;
            SmokingRoom = null;
            smokeChild.GetComponent<ParticleSystem>().Stop();
        }

        if (isSmoking)
        {
            transform.position = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, -1f);
            SmokingRoom.GetComponent<RoomBehavior>().flameLevel -= Time.deltaTime * 1f;
            print("extinguishing " + SmokingRoom.name);
        }


    }
}
