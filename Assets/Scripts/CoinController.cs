using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    private float rotationSpeed = 100;
    private void Start()
    {
        rotationSpeed += Random.Range(0, rotationSpeed / 4.0f);
    }
    void Update()
    {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            transform.parent.gameObject.SetActive(false);
        }        
    }
}
