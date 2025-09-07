using UnityEngine;
using BNG;

public class ArrowPickupRelay : MonoBehaviour
{
    public ArrowManager arrowManager;

    private Grabbable grabbable;
    private bool hasStartedThisHold;

    private void Awake()
    {

        Debug.Log($"[ArrowPickupRelay] Awake on: {gameObject.name}");

        if (arrowManager == null)
        {
            arrowManager = FindObjectOfType<ArrowManager>();
        }

        grabbable = GetComponent<Grabbable>();

        if (grabbable == null)
        {
            Debug.LogWarning("[ArrowPickupRelay] No Grabbable found on Arrow2.");
        }
    }

    private void Update()
    {
        if (grabbable == null) return;

        if (grabbable.BeingHeld && !hasStartedThisHold)
        {
            hasStartedThisHold = true;

            arrowManager?.MarkPickup();
            Debug.Log("[ArrowPickupRelay] Arrow grabbed → timer started");
        }
        else if (!grabbable.BeingHeld && hasStartedThisHold)
        {
            hasStartedThisHold = false;
        }
    }
}
