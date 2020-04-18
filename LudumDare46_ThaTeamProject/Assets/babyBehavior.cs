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

    Vector3 flyingDirection;

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

    }

    void Update()
    {
        if (babyState != BabyState.Dead)
        {
            if (babyState == BabyState.FLying)
            {
                transform.Translate(flyingDirection * Time.deltaTime * 1.5f, Space.World);
                transform.Rotate(0, 0, Time.deltaTime * 250f, Space.World);

                print(Vector3.Distance(Vector3.zero, transform.position));
                if (Vector3.Distance(Vector3.zero, transform.position) > 10f)
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
                                 target.GetComponent<InteractionBehavior>().interactionType == InteractionType.Vent ||
                                 target.GetComponent<InteractionBehavior>().interactionType == InteractionType.Clone)
                        {
                            babyState = BabyState.Interacting;
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
                //kill tous les babies de la pièce
                GameObject[] babies = GameObject.FindGameObjectsWithTag("Baby");
                
                foreach (GameObject baby in babies)
                {
                    if (baby.GetComponent<babyBehavior>().currentRoom == currentRoom)
                    {
                        //Destroy(baby);

                        babyState = BabyState.FLying;

                        flyingDirection = Vector3.Normalize(target.transform.position - transform.position);
                        target = null;
                        GetComponent<AIDestinationSetter>().target = null;

                        GetComponent<AIPath>().maxSpeed = 0f;
                    }
                }
            }
            else if (target.GetComponent<InteractionBehavior>().interactionType == InteractionType.Fire)
            {
                //Room on fire
                babyState = BabyState.Idle;
            }
            else if (target.GetComponent<InteractionBehavior>().interactionType == InteractionType.KillFire)
            {
                Destroy(gameObject);
                //Room on fire
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
