using UnityEngine;

public class EnemyArcher : MonoBehaviour
{
    public float speed = 0.1f;
    public GameObject BulletPrefab;
    public float BulletSpeed;
    Rigidbody2D rigid; 
    GameObject target;

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        GameObject Bullet = Instantiate(BulletPrefab);
       // Bullet.transform.position = target.position.x;

       // Bullet.rigid.linearVelocity = new Vector2()
    }
}
