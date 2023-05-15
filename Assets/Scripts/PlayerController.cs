using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D pLayerRb;

    [SerializeField] private float maxSpeed = 5f; // Максимальная скорость
    [SerializeField] private float acceleration = 2f; // Ускорение
    [SerializeField] private float deceleration = 1f; // Замедление
    [SerializeField] private float rotationSpeed = 1f; // Скорость поворота

    private float currentSpeed = 0f; // Текущая скорость
    private float targetRotation = 0f; // Целевой угол поворота
    private float rotationVelocity = 0f; // Скорость вращения

    [SerializeField] private ParticleSystem particleSystem; // ссылка на компонент ParticleSystem

    [SerializeField] private Transform cannonAreaRight; // пушки справа
    [SerializeField] private Transform cannonAreaLeft; // пушки слева
    [SerializeField] private Bullet bulletPrefab; // префаб пули
    [SerializeField] private float bulletSpeed = 10f; // скорость пули
    [SerializeField] private float recoilSpeed = 1f; // скорость отдачи
    [SerializeField] public float reloading = 1f; // общий таймер перезатядки
    private float reloadingTimerLeft = 1f; // таймер перезатядки для пушек слева
    private float reloadingTimerRight = 1f; // таймер перезатядки для пушек справа

    private void Start()
    {
        pLayerRb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Получаем ввод пользователя
        float horizontalInput = -Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Изменяем целевой угол поворота
        float targetRotationSpeed = horizontalInput * rotationSpeed * currentSpeed;
        rotationVelocity = Mathf.Lerp(rotationVelocity, targetRotationSpeed,
            Time.deltaTime * 5f); // использование метода Lerp для изменения скорости поворота
        targetRotation +=
            rotationVelocity * Time.deltaTime; // изменение целевого угла поворота на основе скорости поворота

        // Вычисляем скорость
        currentSpeed += verticalInput * acceleration * Time.deltaTime;

        // Ограничиваем скорость
        currentSpeed = Mathf.Clamp(currentSpeed, 0f, maxSpeed);

        // Двигаем корабль по направлению целевого угла поворота
        transform.rotation = Quaternion.Euler(0f, 0f, targetRotation);
        transform.position += transform.up * currentSpeed * Time.deltaTime;

        // Замедляем корабль, если пользователь не движется
        if (Mathf.Approximately(verticalInput, 0f))
        {
            if (currentSpeed > 0f)
            {
                currentSpeed -= deceleration * Time.deltaTime;
                currentSpeed = Mathf.Max(currentSpeed, 0f);
            }
        }

        // Запускаем или останавливаем систему партиклов
        if (currentSpeed > 0f)
        {
            particleSystem.Play();
        }
        else
        {
            if (particleSystem.isPlaying)
            {
                particleSystem.Stop();
            }
        }

        // Создаем частиц
        ParticleSystem.ShapeModule shape = particleSystem.shape;
        shape.length = currentSpeed * 2;

        // обработка выстрелов
        Shooting();

    }

    private void Shooting()
    {
        if (reloadingTimerRight >= reloading)
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
                StartCoroutine(ShootBullets(cannonAreaRight, 1));
                reloadingTimerRight = 0;
            }
        }
        else
        {
            reloadingTimerRight += Time.deltaTime;
        }

        if (reloadingTimerLeft >= reloading)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                StartCoroutine(ShootBullets(cannonAreaLeft, -1));
                reloadingTimerLeft = 0;
            }
        }
        else
        {
            reloadingTimerLeft += Time.deltaTime;
        }
    }

    private IEnumerator ShootBullets(Transform area,  int directionMod)
    {
        List<Transform> cannons = new List<Transform>();
        for (int i = 0; i < area.childCount; i++)
        {
            Transform cannon = area.GetChild(i);
            cannons.Add(cannon);
        }

        // Перемешиваем список
        for (int i = 0; i < cannons.Count; i++)
        {
            Transform temp = cannons[i];
            int randomIndex = Random.Range(i, cannons.Count);
            cannons[i] = cannons[randomIndex];
            cannons[randomIndex] = temp;
        }

        for (int i = 0; i < area.childCount; i++)
        {
            Transform cannon = cannons[i];
            Bullet bullet = Instantiate(bulletPrefab, cannon.position, Quaternion.identity);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.AddForce(cannon.right * bulletSpeed * directionMod, ForceMode2D.Impulse);

            float delay = Random.Range(0.02f, 0.2f);
            yield return new WaitForSeconds(delay);
        }

        var dir = pLayerRb.transform.right;
        pLayerRb.AddForce(-dir * recoilSpeed * directionMod, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.5f); // задержка для отдачи

        pLayerRb.AddForce(dir * recoilSpeed * directionMod, ForceMode2D.Impulse);
    }
}