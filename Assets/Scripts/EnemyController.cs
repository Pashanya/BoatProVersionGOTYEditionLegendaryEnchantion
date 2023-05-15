using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 2.0f;

    public int health = 5;

    private Rigidbody2D enemyRb;
    private Transform playerTransform; // ссылка на трансформ игрока
    [SerializeField] private float maxSpeed = 5f; // Максимальная скорость
    [SerializeField] private float acceleration = 2f; // Ускорение
    [SerializeField] private float deceleration = 1f; // Замедление
    [SerializeField] private float rotationSpeed = 1f; // Скорость поворота
    private float currentSpeed = 0f; // Текущая скорость
    private float targetRotation = 0f; // Целевой угол поворота
    private float rotationVelocity = 0f; // Скорость вращения

    void Start()
    {
        enemyRb = GetComponent<Rigidbody2D>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform; // поиск игрока по тегу
    }

    void Update()
    {
        // Вычисляем вектор направления к игроку
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        // Изменяем целевой угол поворота, чтобы враг был боком к игроку
        targetRotation = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg - 90f;

        // Изменяем текущую скорость
        currentSpeed += acceleration * Time.deltaTime;

        // Ограничиваем скорость
        currentSpeed = Mathf.Clamp(currentSpeed, 0f, maxSpeed);

        // Поворачиваем врага к целевому углу поворота
        float rotationDifference = targetRotation - transform.eulerAngles.z;
        if (rotationDifference > 180f)
        {
            rotationDifference -= 360f;
        }
        else if (rotationDifference < -180f)
        {
            rotationDifference += 360f;
        }

        float rotationDirection = Mathf.Sign(rotationDifference);
        float rotationAmount = rotationSpeed * rotationDirection * Time.deltaTime;
        if (Mathf.Abs(rotationDifference) < Mathf.Abs(rotationAmount))
        {
            rotationAmount = rotationDifference;
        }

        transform.Rotate(0f, 0f, rotationAmount);

        // Двигаем врага по направлению целевого угла поворота
        enemyRb.velocity = transform.up * currentSpeed;

        // Замедляем врага, если он не движется к игроку
        if (Vector3.Dot(transform.up, directionToPlayer) < 0.5f)
        {
            if (currentSpeed > 0f)
            {
                currentSpeed -= deceleration * Time.deltaTime;
                currentSpeed = Mathf.Max(currentSpeed, 0f);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}