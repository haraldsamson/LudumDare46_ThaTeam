using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class babyBehavior : MonoBehaviour
{

    interactionManager intManager;
    GameObject target;

    enum BabyState { Idle, Walking, Interacting, Dead, FLying };

    BabyState babyState;

    GameObject currentRoom;

    Transform window;
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
        intManager = GameObject.Find("InteractionManager").GetComponent<interactionManager>();
        target = intManager.interactionPoints[Random.Range(0, intManager.interactionPoints.Length)];
        GetComponent<AIDestinationSetter>().target = target.transform;
        babyState = BabyState.Walking;

        //update "opti" tous les 0.1s
        InvokeRepeating("Update01", Random.Range(0f,0.1f), 0.1f);
    }

    void Update01()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, 1, LayerMask.GetMask("RoomFloor"));

        if (hit.collider != null)
        {
            if (hit.transform.parent.gameObject != currentRoom)
            {
                currentRoom = hit.transform.parent.gameObject;
                //print(hit.transform.parent.gameObject.name);
            }
        }
        else
        {
            currentRoom = null;
        }

        if (currentRoom != null)
        {
            if (currentRoom.GetComponent<RoomBehavior>() != null)
            {
                if (currentRoom.GetComponent<RoomBehavior>().isBigFire)
                    Destroy(this.gameObject);
            }
        }

        //print("currentRoom is " + currentRoom);

    }

    void Update()
    {
        if (babyState != BabyState.Dead)
        {
            if (babyState == BabyState.FLying)
            {
                if (isAccelerating)
                {
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
                    currentRoom = null;
                }

                if (Vector3.Distance(transform.position, Vector3.zero) > 10f)
                {
                    Destroy(this.gameObject);
                }
            }
            else
            {
                //Quand le bébé arrive sur sa target
                //print(Vector3.Distance(GetComponent<AIDestinationSetter>().target.position, transform.position));
                if (GetComponent<AIDestinationSetter>().target != null)
                {
                    if (Vector3.Distance(GetComponent<AIDestinationSetter>().target.position, transform.position) < 0.1f)
                    {
                        if (target.GetComponent<InteractionBehavior>().interactionType == InteractionType.Idle)
                        {
                            babyState = BabyState.Idle;
                        }
                        else if (target.GetComponent<InteractionBehavior>().interactionType == InteractionType.Kill ||
                                 target.GetComponent<InteractionBehavior>().interactionType == InteractionType.Fire ||
                                 target.GetComponent<InteractionBehavior>().interactionType == InteractionType.KillFire ||
                                 target.GetComponent<InteractionBehavior>().interactionType == InteractionType.Clone)
                        {
                            babyState = BabyState.Interacting;
                        }
                        else if (target.GetComponent<InteractionBehavior>().interactionType == InteractionType.Vent)
                        {
                            babyState = BabyState.Interacting;
                            target.GetComponent<InteractionBehavior>().room.GetComponent<RoomBehavior>().TurnOnRedLight();
                        }

                        //print("target reached");
                        StartCoroutine(ExecuteInteraction(target.GetComponent<InteractionBehavior>().interactionTime));

                        //désactive la target de déplacement
                        GetComponent<AIDestinationSetter>().target = null;

                    }
                }

                if (Input.GetMouseButtonDown(0))
                {
                    if (GetComponent<PolygonCollider2D>().OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
                    {
                        //stop if interacting except clone
                        if (babyState == BabyState.Interacting && target.GetComponent<InteractionBehavior>().interactionType != InteractionType.Clone)
                        {
                            print("Arrête ça " + this.name + " !");

                            if (target.GetComponent<InteractionBehavior>().interactionType == InteractionType.Vent)
                            {
                                target.GetComponent<InteractionBehavior>().room.GetComponent<RoomBehavior>().TurnOffRedLight();
                            }

                            FindNewTarget();
                        }


                    }
                }
            }
        }
    }

    IEnumerator ExecuteInteraction(float time)
    {
        yield return new WaitForSeconds(time);

        if (babyState == BabyState.Interacting)
        {
            if (target.GetComponent<InteractionBehavior>().interactionType == InteractionType.Clone)
            {
                //print("new BABYYYYYYYYY");
                Instantiate(intManager.babyPrefab, transform.position, transform.rotation);
                babyState = BabyState.Idle;
            }
            else if (target.GetComponent<InteractionBehavior>().interactionType == InteractionType.Kill)
            {
                Destroy(gameObject);
            }
            else if (target.GetComponent<InteractionBehavior>().interactionType == InteractionType.Vent)
            {
                window = target.transform.Find("Window").transform;

                target.GetComponent<InteractionBehavior>().room.GetComponent<RoomBehavior>().TurnOffRedLight();

                //kill tous les babies de la pièce
                GameObject[] babies = GameObject.FindGameObjectsWithTag("Baby");

                foreach (GameObject baby in babies)
                {
                    var babyBehav = baby.GetComponent<babyBehavior>();
                    if (babyBehav.currentRoom == currentRoom)
                    {
                        //start flying in space
                        babyBehav.babyState = BabyState.FLying;
                        babyBehav.window = window;
                        babyBehav.flyDirection = Vector3.Normalize(window.position - babyBehav.transform.position);
                        babyBehav.isAccelerating = true;

                        //stop pathfinding
                        babyBehav.target = null;
                        baby.GetComponent<AIDestinationSetter>().target = null;
                        baby.GetComponent<AIPath>().maxSpeed = 0f;
                    }
                }
            }
            else if (target.GetComponent<InteractionBehavior>().interactionType == InteractionType.Fire)
            {
                currentRoom.GetComponent<RoomBehavior>().FireSmallStart();
                babyState = BabyState.Idle;
            }
            else if (target.GetComponent<InteractionBehavior>().interactionType == InteractionType.KillFire)
            {
                currentRoom.GetComponent<RoomBehavior>().FireSmallStart();
                Destroy(this.gameObject);
            }
            //print("interaction " + target.GetComponent<InteractionBehavior>().interactionType + " terminée");
        }

        
        //si bébé n'est pas déjà reparti en vadrouille (interaction interupt)
        if (babyState == BabyState.Idle)
        {
            FindNewTarget();
        }
    }

    void FindNewTarget()
    {
        //tirer une nouvelle target random
        target = intManager.interactionPoints[Random.Range(0, intManager.interactionPoints.Length)];
        GetComponent<AIDestinationSetter>().target = target.transform;

        babyState = BabyState.Walking;

        //tirer une target random (autre que celle ou le bébé est déjà)
        while (Vector3.Distance(GetComponent<AIDestinationSetter>().target.position, transform.position) < 0.15f)
        {
            target = intManager.interactionPoints[Random.Range(0, intManager.interactionPoints.Length)];
            GetComponent<AIDestinationSetter>().target = target.transform;
        }
    }

    /*
    public virtual void AIPath.OnTargetReached() //public virtual void 
    {
        print("target reached");
        //GetComponent<AIDestinationSetter>().target = intManager.interactionPoints[Random.Range(0, intManager.interactionPoints.Length)];
    }
    */

}
