using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Bullet : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 direction;
    public float speed;
    private float distanceToDestroy = 20.0f;
    public int damage = 1;
    public GameObject canonArea;
    public string enemyTag = "Enemy";

    public void SetDirection(Vector2 dir, float spd)
    {
        direction = dir;
        speed = spd;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Задаем скорость пули
        // rb.velocity = direction * speed;
    }

    void Update()
    {
        // Уничтожаем пулю, если она улетела далеко
        if (transform.position.magnitude > distanceToDestroy)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject collidedObject = collision.collider.gameObject;
        if (collidedObject != canonArea)
        {
            EnemyController enemy = collidedObject.GetComponent<EnemyController>();
            Debug.Log(enemy.name);
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }
}