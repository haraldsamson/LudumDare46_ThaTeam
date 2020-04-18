using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class babyBehavior : MonoBehaviour
{

    public interactionManager intManager;

    void Start()
    {
        GetComponent<AIDestinationSetter>().target = intManager.interactionPoints[Random.Range(0, intManager.interactionPoints.Length)];
    }

    void Update()
    {
        //print(Vector3.Distance(GetComponent<AIDestinationSetter>().target.position, transform.position));
        if (GetComponent<AIDestinationSetter>().target != null)
        {
            if (Vector3.Distance(GetComponent<AIDestinationSetter>().target.position, transform.position) < 0.1f)
            {
                print("target reached");
                GetComponent<AIDestinationSetter>().target = null;
                StartCoroutine(ExecuteAfterTime(2f));

            }
        }
    }

    IEnumerator ExecuteAfterTime(float time)
    {
        yield return new WaitForSeconds(time);

        GetComponent<AIDestinationSetter>().target = intManager.interactionPoints[Random.Range(0, intManager.interactionPoints.Length)];

        while (Vector3.Distance(GetComponent<AIDestinationSetter>().target.position, transform.position) < 0.15f)
        {
            GetComponent<AIDestinationSetter>().target = intManager.interactionPoints[Random.Range(0, intManager.interactionPoints.Length)];
            yield return null;
        }
        
    }

    private IEnumerator Countdown()
    {
        float duration = 2f;

        float normalizedTime = 0;
        while (normalizedTime <= 1f)
        {
            normalizedTime += Time.deltaTime / duration;
            yield return null;
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
