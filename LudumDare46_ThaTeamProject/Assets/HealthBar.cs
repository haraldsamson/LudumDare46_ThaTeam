using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    Transform bar;
    public Transform ship;
    float hp;
    public float shakeDuration = 0.5f;
    public float shakeScale = 0.3f;
    public GameObject gameOverScreen;
    public GameObject explosionPrefab;
    Bounds shipBounds;

    // Start is called before the first frame update
    void Start()
    {
        bar = transform.Find("Bar");
        hp = 100f;
        bar.localScale = new Vector3(hp / 100f, 1f);

        shipBounds = ship.GetComponent<PolygonCollider2D>().bounds;
    }

    public void HPChange(float hpVar)
    {
        hp = Mathf.Clamp(hp+hpVar, 0f,100f);
        bar.localScale = new Vector3(hp/100f, 1f);

        if (hp <= 0f)
        {
            print("GAMEOVER");
            StartCoroutine("GameOverShakeCamera");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine("ShakeCamera");
        }

    }

    IEnumerator ShakeCamera()
    {
        // Store the original position of the ship.
        Vector3 origPos = ship.position;
        for (float t = 0.0f; t < shakeDuration; t += Time.deltaTime)
        {
            // Create a temporary vector2 with the ship's original position modified by a random distance from the origin.
            Vector3 tempVec = origPos + Random.insideUnitSphere * shakeScale;

            // Apply the temporary vector.
            ship.position = tempVec;

            // Yield until next frame.
            yield return null;
        }

        // Return back to the original position.
        ship.position = origPos;
    }

    IEnumerator GameOverShakeCamera()
    {
        // Store the original position of the ship.
        Vector3 origPos = ship.position;
        float intensity = 0f;

        InvokeRepeating("Explosion", 0f, 0.3f);

        for (float t = 0.0f; t < 3f; t += Time.deltaTime)
        {
            intensity = Mathf.Clamp(intensity + Time.deltaTime * 3f, 0f, 1f); 

            // Create a temporary vector2 with the ship's original position modified by a random distance from the origin.
            origPos -= new Vector3(Time.deltaTime * 2.5f, 0f, 0f);
            Vector3 tempVec = origPos + Random.insideUnitSphere * shakeScale * intensity;

            // Apply the temporary vector.
            ship.position = tempVec;

            // Yield until next frame.
            yield return null;
        }

        //all lights off

        for (float t = 0.0f; t < 6f; t += Time.deltaTime)
        {
            intensity = Mathf.Clamp(intensity - Time.deltaTime * 0.6f, 0f, 1f);

            // Apply the temporary vector.
            origPos -= new Vector3(Time.deltaTime * 2.5f, 0f, 0f);
            Vector3 tempVec = origPos + Random.insideUnitSphere * shakeScale * intensity;

            ship.position = tempVec;

            ship.Rotate(0f,0f,Time.deltaTime * 3f, Space.World);

            // Yield until next frame.
            yield return null;
        }

        CancelInvoke();

        // display final GAMEOVER screenn
        gameOverScreen.SetActive(true);

    }

    void Explosion()
    {
        shipBounds = ship.GetComponent<PolygonCollider2D>().bounds;
        //1 chance sur 3 pour casser le rythme
        if (Random.Range(0,3) == 0)
        {
            Vector3 randPos = new Vector3(
                Random.Range(shipBounds.min.x, shipBounds.max.x),
                Random.Range(shipBounds.min.y, shipBounds.max.y),
                Random.Range(shipBounds.min.z, shipBounds.max.z));

            Instantiate(explosionPrefab, randPos, Quaternion.identity);
        }
    }
}
