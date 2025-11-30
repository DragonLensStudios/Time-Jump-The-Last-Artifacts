using System;
using UnityEngine;

public class SpawnOnDestroyObject : MonoBehaviour
{
    [field: SerializeField] public GameObject Prefab { get; set; }
    [field: SerializeField] public Vector3 Offset { get; set; }
    [field: SerializeField] public Quaternion Rotation { get; set; } = Quaternion.identity;
    [field: SerializeField] public Vector3 Scale { get; set; } = Vector3.one;
    [field: SerializeField] public double Delay { get; set; } = 3.0;
    
    // public void Start()
    // {
    //     var obj = Instantiate(Prefab, transform.position + Offset, Rotation);
    //     Destroy(obj, (float)Delay);
    // }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        var obj = Instantiate(Prefab, transform.position + Offset, Rotation);
        obj.transform.localScale = Scale;
        Destroy(obj, (float)Delay);
    }
}
