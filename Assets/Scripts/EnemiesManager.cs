using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Wrld;
using Wrld.Space;
using Wrld.Transport;

public class EnemiesManager : MonoBehaviour
{
    public string POIURL;
    public GameObject EnemyPrefab;
    public GameObject RoadEnemy;

    private TransportPositioner m_transportPositioner;
    private readonly LatLongAltitude m_inputCoords = LatLongAltitude.FromDegrees(37.7850068, -122.400752, 10.0);

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
    void OnEnable()
    {
        RoadEnemy = Instantiate(EnemyPrefab) as GameObject;
        RoadEnemy.transform.localRotation = Quaternion.identity;

        var options = new TransportPositionerOptionsBuilder()
           .SetInputCoordinates(m_inputCoords.GetLatitude(), m_inputCoords.GetLongitude())
           .SetInputHeading(225.0f)
           .Build();

        m_transportPositioner = Api.Instance.TransportApi.CreatePositioner(options);
        m_transportPositioner.OnPointOnGraphChanged += OnPointOnGraphChanged;

        //Wait for the buildings to load so enemies can be swapwned on top
        Api.Instance.OnInitialStreamingComplete += GetEnemies;
    }

    public void GetEnemies()
    {
        StartCoroutine(GetEnemiesCo());
    }

    private void OnPointOnGraphChanged()
    {
        if (m_transportPositioner.IsMatched())
        {
            var pointOnGraph = m_transportPositioner.GetPointOnGraph();

            var outputLLA = LatLongAltitude.FromECEF(pointOnGraph.PointOnWay);
            const double verticalOffset = 1.0;
            outputLLA.SetAltitude(outputLLA.GetAltitude() + verticalOffset);
            var outputPosition = Api.Instance.SpacesApi.GeographicToWorldPoint(outputLLA);

            RoadEnemy.transform.position = outputPosition;
        }
    }

    IEnumerator GetEnemiesCo()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(POIURL))
        {
            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log("Error: " + request.error);
            }
            else
            {
                APIOutput output = JsonUtility.FromJson<APIOutput>("{\"EnemyPositions\":" + request.downloadHandler.text + "}");
                SpawnEnemies(output.EnemyPositions);
            }
        }
    }

    public void SpawnEnemies(EnemyPosition[] EnemyPositions)
    {
        foreach (EnemyPosition position in EnemyPositions)
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
