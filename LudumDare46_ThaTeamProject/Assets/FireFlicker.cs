using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireFlicker : MonoBehaviour
{

    SpriteRenderer spRend;
    float r;

    // Start is called before the first frame update
    void Start()
    {
        spRend = GetComponent<SpriteRenderer>();
        r = Random.Range(0f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        var x = (Mathf.Sin((Time.time + r) * 5f) + 1) / 2;
        spRend.color = new Color(1f, (110f + x * 80f)/255f, 0f);
    }
}
