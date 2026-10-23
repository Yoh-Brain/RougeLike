using UnityEngine;

public abstract class BaseLoot : MonoBehaviour
{
    public int amount = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 调用子类的具体实现
            Collect();
            // 拾取后销毁自己
            Destroy(gameObject);
        }
    }

    // 抽象方法，强制子类必须实现
    protected abstract void Collect();
}