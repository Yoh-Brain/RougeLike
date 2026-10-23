using UnityEngine;

public class EffectSelfDestroy : MonoBehaviour
{
    // 这个方法将会被动画事件调用
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}