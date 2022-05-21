using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Wrld;
using Wrld.Space;

public class EnemiesManager : MonoBehaviour
{
    public string POIURL;
    public GameObject EnemyPrefab;

    [Serializable]
    public class APIOutput
    {
        public EnemyPosition[] EnemyPositions;
    }

    [Serializable]
    public class EnemyPosition
    {
        public float lat;
        public float lon;
    }


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetEnemies());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator GetEnemies()
    {
        yield return new WaitForSeconds(4f);

        using (UnityWebRequest request = UnityWebRequest.Get(POIURL))
        {
            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log("Error: " + request.error);
            }
            else
            {
                Debug.Log(request.downloadHandler.text);

                APIOutput output = JsonUtility.FromJson<APIOutput>("{\"EnemyPositions\":" + request.downloadHandler.text + "}");
                SpawnEnemies(output.EnemyPositions);
            }
        }
    }

    public void SpawnEnemies(EnemyPosition[] EnemyPositions)
    {
        foreach(EnemyPosition position in EnemyPositions)
        {
            MakeEnemy(LatLong.FromDegrees(position.lat, position.lon));
        }
    }

    void MakeEnemy(LatLong latLong)
    {
        var ray = Api.Instance.SpacesApi.LatLongToVerticallyDownRay(latLong);
        LatLongAltitude buildingIntersectionPoint;
        var didIntersectBuilding = Api.Instance.BuildingsApi.TryFindIntersectionWithBuilding(ray, out buildingIntersectionPoint);
        if (didIntersectBuilding)
        {
            var Enemy = Instantiate(EnemyPrefab) as GameObject;
            Enemy.GetComponent<GeographicTransform>().SetPosition(buildingIntersectionPoint.GetLatLong());
            Enemy.transform.localPosition = Vector3.up * (float)buildingIntersectionPoint.GetAltitude();
        }
    }
}
