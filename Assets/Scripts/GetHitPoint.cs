using UnityEngine;

public class GetHitPoint : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Arrow"))
        {
            // Get the point of impact
            Vector3 hitPoint = collision.contacts[0].point;
            Debug.Log("Hit Point: " + hitPoint);
        }
    }
}
