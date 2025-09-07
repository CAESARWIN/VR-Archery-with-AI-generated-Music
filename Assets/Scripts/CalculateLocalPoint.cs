using UnityEngine;

public class CalculateLocalPoint : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public Vector3 GetLocalPoint(Vector3 worldPoint)
    {
        // Convert the world point to local space relative to this GameObject
        return transform.InverseTransformPoint(worldPoint);
    }
}
