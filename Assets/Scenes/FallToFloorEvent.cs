using UnityEngine;

public class FallToFloorEvent : MonoBehaviour
{
    [Header("想变色的物体(粒子或普通模型)")]
    public GameObject targetObject; 
    public Color colorOnImpact = Color.red;

    private void OnCollisionEnter(Collision collision)
    {
        // 打印一条消息到 Console，确认碰撞发生了
        Debug.Log("碰到了: " + collision.gameObject.name);

        if (targetObject != null)
        {
            // 尝试改粒子颜色
            var ps = targetObject.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                var main = ps.main;
                main.startColor = colorOnImpact;
            }

            // 尝试改物体表面材质颜色
            var renderer = targetObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = colorOnImpact;
            }
        }
    }
}