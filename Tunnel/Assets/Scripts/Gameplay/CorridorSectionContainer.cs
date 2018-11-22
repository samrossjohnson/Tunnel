using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorridorSectionContainer : MonoBehaviour
{

    void Update()
    {
        if (transform.position.y < Camera.main.transform.position.y - Camera.main.orthographicSize - 8f)
            Destroy(gameObject);
    }
}
