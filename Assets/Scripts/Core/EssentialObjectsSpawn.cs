using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EssentialObjectsSpawn : MonoBehaviour
{
    [SerializeField] GameObject essentialObjectsPrefab;

    private void Awake()
    {
        var existingObjects = FindObjectsOfType<EssentialObjects>();

        if (existingObjects.Length == 0)
            Instantiate(essentialObjectsPrefab, new Vector3(0, 0, 0), Quaternion.identity);
    }
}
