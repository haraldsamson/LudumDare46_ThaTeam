using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    Transform bar;
    public Transform ship;
    float hp;
    public float shakeDuration = 0.7f;
    public float shakeScale = 1f;
    public GameObject explosionPrefab;
    public SpriteRenderer barSprite;

    public GameObject gameOverMenuUI;

    // Start is called before the first frame update
    void Start()
    {
        bar = transform.Find("Bar");
        hp = 100f;
        bar.localScale = new Vector3(hp / 100f, 1f);
    }

    public void HPChange(float hpVar,Bounds roomFloor)
    {
        hp = Mathf.Clamp(hp+hpVar, 0f,100f);
        bar.localScale = new Vector3(hp/100f, 1f);

        StartCoroutine("HpBarFlicker");
        StartCoroutine("ShakeCamera");
        Explosion(roomFloor);

        if (hp <= 0f)
        {
            print("GAMEOVER");
            StartCoroutine("GameOverShakeCamera");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator HpBarFlicker()
    {
        float delta = 0.1f;
        float deltaSum = 0.1f;
        bool isWhite = false;

        for (float t = 0.0f; t < shakeDuration; t += Time.deltaTime)
        {
            if (t > deltaSum)
            {
                if (isWhite)
                {
                    barSprite.color = new Color(1f, 0f, 0f);
                    isWhite = false;
                }
                else
                {
                    barSprite.color = new Color(1f, 1f, 1f);
                    isWhite = true;
                }
                deltaSum += delta;
            }
            
            yield return null;
        }

        // Return back to the original color.
        barSprite.color = new Color(1f, 0f, 0f);
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

        InvokeRepeating("ShipExplosion", 0f, 0.3f);

        for (float t = 0.0f; t < 4f; t += Time.deltaTime)
        {
            intensity = Mathf.Clamp(intensity + Time.deltaTime * 3f, 0f, 1f); 

            // Create a temporary vector2 with the ship's original position modified by a random distance from the origin.
            origPos -= new Vector3(Time.deltaTime * 8f, 0f, 0f);
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
            origPos -= new Vector3(Time.deltaTime * 8f, 0f, 0f);
            Vector3 tempVec = origPos + Random.insideUnitSphere * shakeScale * intensity;

            ship.position = tempVec;

            ship.Rotate(0f,0f,Time.deltaTime * 3f, Space.World);

            // Yield until next frame.
            yield return null;
        }

        CancelInvoke();
        // display final GAMEOVER screenn
        gameOverMenuUI.SetActive(true);

        foreach (Transform chil in transform)
        {
            chil.gameObject.SetActive(false);
        }

        Time.timeScale = 0f;

    }

    void ShipExplosion()
    {
        //1 chance sur 3 pour casser le rythme
        if (Random.Range(0, 4) == 0)
        {
            Explosion(ship.gameObject.GetComponent<PolygonCollider2D>().bounds);
        }
    }

    void Explosion(Bounds bounds)
    {
        GameObject explo;

        Vector3 randPos = new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z));

        explo = Instantiate(explosionPrefab, randPos, Quaternion.identity);
        Destroy(explo, 0.8f);
} 

}
