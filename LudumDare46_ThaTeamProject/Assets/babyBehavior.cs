using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class babyBehavior : MonoBehaviour
{

    public interactionManager intManager;
    GameObject target;

    enum BabyState { Idle, Walking, Interacting, Dead };

    BabyState babyState;

    void Start()
    {
        target = intManager.interactionPoints[Random.Range(0, intManager.interactionPoints.Length)];
        GetComponent<AIDestinationSetter>().target = target.transform;
        babyState = BabyState.Walking;
    }

    void Update()
    {
        //Quand le bébé arrive sur sa target
        //print(Vector3.Distance(GetComponent<AIDestinationSetter>().target.position, transform.position));
        if (GetComponent<AIDestinationSetter>().target != null)
        {
            if (Vector3.Distance(GetComponent<AIDestinationSetter>().target.position, transform.position) < 0.1f)
            {
                if (target.GetComponent<InteractionBehavior>().interactionType == InteractionType.Idle ||
                    target.GetComponent<InteractionBehavior>().interactionType == InteractionType.Clone )
                {
                    babyState = BabyState.Idle;
                }
                else if (target.GetComponent<InteractionBehavior>().interactionType == InteractionType.Kill ||
                         target.GetComponent<InteractionBehavior>().interactionType == InteractionType.Fire ||
                         target.GetComponent<InteractionBehavior>().interactionType == InteractionType.KillFire ||
                         target.GetComponent<InteractionBehavior>().interactionType == InteractionType.Vent )
                {
                    babyState = BabyState.Interacting;
                }

                //print("target reached");
                StartCoroutine( ExecuteInteraction( target.GetComponent<InteractionBehavior>().interactionTime ));

                //désactive la target de déplacement
                GetComponent<AIDestinationSetter>().target = null;

            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (GetComponent<PolygonCollider2D>().OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
            {
                if (babyState == BabyState.Interacting)
                {
                    print("Arrête ça " + this.name + " !");

                    FindNewTarget();
                }
                    
                    
            }
        }
    }

    IEnumerator ExecuteInteraction(float time)
    {
        yield return new WaitForSeconds(time);

        if (babyState == BabyState.Interacting)
        {
            Destroy(gameObject);
            print("interaction " + target.GetComponent<InteractionBehavior>().interactionType + " terminée");
        }
        
        //si bébé n'est pas déjà reparti en vadrouille (interaction interupt)
        if (babyState != BabyState.Walking)
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
