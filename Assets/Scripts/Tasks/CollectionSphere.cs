using UnityEngine;

public class CollectionSphere : MonoBehaviour
{
    public float radius = 0.5f;
    public LayerMask itemLayer;

    void Update()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, radius, itemLayer);
        foreach (var hit in hits)
        {
            var item = hit.GetComponent<ItemCollectable>();
            if (item != null)
                item.TryCollect();
        }
    }
}