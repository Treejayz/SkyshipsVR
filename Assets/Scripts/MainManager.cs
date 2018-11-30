using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : MonoBehaviour {

    public static Vector3 minBounds = new Vector3(-280f, -10f, -280f);
    public static Vector3 maxBounds = new Vector3(280f, 140f, 280f);
    public static Vector3 wind = new Vector3(1f, 0f, 1f);

    public AudioSource windSound;
    public CloudManager clouds;
    public ShipControls ship;
    public GameObject windzone;

    float max;

    private void Start()
    {
        max = wind.magnitude;
        windSound.volume = .15f;
        StartCoroutine("ChangeWind");
    }

    IEnumerator ChangeWind()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(30f, 120f));
            Random.InitState(System.DateTime.Now.Millisecond);
            Vector3 newWind = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
            print(newWind);
            while (Vector3.Distance(wind, newWind) > .05f)
            {
                wind = Vector3.MoveTowards(wind, newWind, Time.deltaTime * 0.2f);
                clouds.wind = wind * 1.5f;
                ship.windDirection = wind;
                ship.windForce = wind.magnitude * .3f;
                windzone.GetComponent<WindZone>().windMain = wind.magnitude;
                windzone.transform.rotation = Quaternion.LookRotation(wind);
                windSound.volume = .25f * (wind.magnitude / max);
                yield return new WaitForEndOfFrame();
            }
            wind = newWind;
            clouds.wind = wind * 1.5f;
            ship.windDirection = wind;
            ship.windForce = wind.magnitude * .3f;
            windzone.GetComponent<WindZone>().windMain = wind.magnitude;
            windzone.transform.rotation = Quaternion.LookRotation(wind);
            windSound.volume = .25f * (wind.magnitude / max);
        }
    }

}
