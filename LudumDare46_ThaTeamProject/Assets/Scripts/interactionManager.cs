using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InteractionType { Idle, Vent, Kill, Fire, KillFire, Autodestruct, Clone };

public class interactionManager : MonoBehaviour
{
    public List<GameObject> interactionPoints = new List<GameObject>();
    public GameObject babyPrefab;
    public HealthBar healthBar;

    private void Awake()
    {
        foreach (Transform child in transform)
        {
            interactionPoints.Add(child.gameObject);
        }
        print("there is " + interactionPoints.Count + " interactionPoints");
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
