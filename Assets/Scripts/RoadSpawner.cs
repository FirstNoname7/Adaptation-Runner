using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadSpawner : MonoBehaviour
{
    static public RoadSpawner instance;

    [SerializeField] private GameObject roadPrefab;
    private List<GameObject> roads = new List<GameObject>();
    private float maxSpeed = 10.0f;
    public float currentSpeed = 0.0f;
    private int maxRoadCount = 5;
    [SerializeField] private Rigidbody rigidbody;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        ResetLevel();
    }

    public void StartLevel()
    {
        currentSpeed = maxSpeed;
        SwipeManager.instance.enabled = true; //включение свайпа
    }

    private void Update()
    {
        if (currentSpeed == 0)
        {
            return;
        }
        
        //Vector3 trans = new Vector3(0, 0, currentSpeed * Time.deltaTime);
        foreach (GameObject road in roads)
        {
            road.transform.position -= new Vector3(0, 0, currentSpeed * Time.deltaTime);
            //rigidbody.MovePosition(trans);
        }

        if (roads[0].transform.position.z < -15)
        {
            Destroy(roads[0]);
            roads.RemoveAt(0);
            CreateNextRoad();
        }
    }

    private void CreateNextRoad()
    {
        Vector3 pos = Vector3.zero;
        if (roads.Count > 0)
        {
            pos = roads[roads.Count - 1].transform.position + new Vector3(0, 0, 15); //15 - ширина плоскости, т.е. по оси Z
        }
        GameObject go = Instantiate(roadPrefab, pos, Quaternion.identity);
        go.transform.SetParent(transform);
        roads.Add(go);
    }

    public void ResetLevel()
    {
        currentSpeed = 0;
        while (roads.Count > 0)
        {
            Destroy(roads[0]);
            roads.RemoveAt(0);
        }
        for(int i = 0; i < maxRoadCount; i++)
        {
            CreateNextRoad();
        }
        SwipeManager.instance.enabled = true; //сброс свайпа
        MapSpawn.instance.ResetMaps();
    }

}
