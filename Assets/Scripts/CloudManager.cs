using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudManager : MonoBehaviour {

    public Vector3 wind;

    public int numClouds;

    public Vector3 minPos;
    public Vector3 maxPos;

    public GameObject[] cloudPrefabs;

    List<GameObject> Clouds = new List<GameObject>();

	// Use this for initialization
	void Awake () {

        float xRange, zRange, yRange;
        xRange = maxPos.x - minPos.x;
        zRange = maxPos.z - minPos.z;
        yRange = maxPos.y - minPos.y;

        float temp = Mathf.Sqrt((float)(numClouds));
        int rows = Mathf.FloorToInt(temp);
        int columns = rows;
        

        float xSection, zSection;
        xSection = xRange / (float)rows;
        if (numClouds % rows != 0)
        {
            xSection = xRange / (float)(rows + 1);
        }
        zSection = zRange / (float)columns;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                float x = minPos.x + xSection * i;
                x += Random.Range(0f, 1f) * xSection;
                float z = minPos.z + zSection * j;
                z += Random.Range(0f, 1f) * zSection;
                float y = minPos.y +Random.Range(0f, 1f) * yRange;
                Vector3 pos = new Vector3(x, y, z);
                GameObject cloud = Instantiate(cloudPrefabs[Random.Range(0, cloudPrefabs.Length)], pos, Random.rotation);
                Clouds.Add(cloud);
            }
        }

        if (numClouds % rows != 0)
        {
            for (int i = 0; i < numClouds % rows; i++)
            {
                float x = minPos.x + xSection * rows;
                x += Random.Range(0f, 1f) * xSection;
                float z = minPos.z + zSection * i;
                z += Random.Range(0f, 1f) * zSection;
                float y = minPos.y + Random.Range(0f, 1f) * yRange;
                Vector3 pos = new Vector3(x, y, z);
                GameObject cloud = Instantiate(cloudPrefabs[Random.Range(0, cloudPrefabs.Length)], pos, Random.rotation);
                Clouds.Add(cloud);
            }
        }

    }
	
	// Update is called once per frame
	void Update () {
		foreach (GameObject cloud in Clouds)
        {
            cloud.transform.position += (wind * Time.deltaTime);
            if (cloud.transform.position.x > maxPos.x)
            {
                cloud.transform.position = new Vector3(minPos.x, cloud.transform.position.y, cloud.transform.position.z);
            }
            else if (cloud.transform.position.x < minPos.x)
            {
                cloud.transform.position = new Vector3(maxPos.x, cloud.transform.position.y, cloud.transform.position.z);
            }
            if (cloud.transform.position.z > maxPos.z)
            {
                cloud.transform.position = new Vector3(cloud.transform.position.x, cloud.transform.position.y, minPos.z);
            }
            else if (cloud.transform.position.z < minPos.z)
            {
                cloud.transform.position = new Vector3(cloud.transform.position.x, cloud.transform.position.y, maxPos.z);
            }
        }
	}
}
