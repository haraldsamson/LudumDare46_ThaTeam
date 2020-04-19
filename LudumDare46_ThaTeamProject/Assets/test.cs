using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    // Start is called before the first frame update

    void Start()
    {
        //StartCoroutine(ExecuteAfter(2f));
    }

    // Update is called once per frame
    void Update()
    {
        var x = (Mathf.Sin(Time.time*5f) + 1) / 2;
        print(x);

    }

    IEnumerator ExecuteAfter(float time)
    {
        yield return new WaitForSeconds(time);

        Destroy(this.gameObject);
    }
}
