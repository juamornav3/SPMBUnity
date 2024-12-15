using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitKoopa : MonoBehaviour
{
    private Animator animator;
    public bool isShell = false; // Indica si el Koopa está en el caparazón
    public bool canMoveAgain = true; // Indica si el Koopa puede volver a moverse
    private float shellTimer = 4f; // Tiempo antes de que el Koopa salga del caparazón
    private float timer = 0f; // Temporizador interno
    private Koopa koopa;

    private void Start()
    {
        animator = GetComponent<Animator>();
        koopa = GetComponent<Koopa>();
    }

    private void Update()
    {
        // Si el Koopa está en el caparazón, comienza el temporizador
        if (isShell && !canMoveAgain)
        {
            timer += Time.deltaTime;
            if (timer >= shellTimer)
            {
                ExitShell();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            foreach (ContactPoint2D point in other.contacts)
            {
                if (!isShell) // Koopa no está en el caparazón
                {
                    if (point.normal.y <= -0.9f) // Golpe desde arriba
                    {
                        EnterShell(); // El Koopa entra en el caparazón
                        other.gameObject.GetComponent<Mario>().Rebote();
                    }
                    else // Golpe desde otro ángulo
                    {
                        Time.timeScale = 0f; // Pausa el juego
                        other.gameObject.GetComponent<Mario>().DeathMario(); // Mario muere
                    }
                }
                else // Koopa ya está en el caparazón
                {
                    
                    koopa.moveSpeed = 5f;
                    if (point.normal.x <= -0.9f && canMoveAgain ==false) // Golpe desde la derecha
                    {
                        // Mover caparazón hacia la izquierda
                        koopa.movingRight = false;
                        koopa.Flip();
                        other.gameObject.GetComponent<Mario>().TriggerInvulnerability();
                        GetComponent<Rigidbody2D>().velocity = new Vector2(-koopa.moveSpeed, 0);
                        canMoveAgain = true;
                        timer = 0f;
                    }
                    else if (point.normal.x >= 0.9f && canMoveAgain == false) // Golpe desde la izquierda
                    {
                        // Mover caparazón hacia la derecha
                        koopa.movingRight = true;
                        koopa.Flip();
                        GetComponent<Rigidbody2D>().velocity = new Vector2(koopa.moveSpeed, 0);
                        other.gameObject.GetComponent<Mario>().TriggerInvulnerability();
                        canMoveAgain = true;
                        timer = 0f;
                    }
                    else if (point.normal.y <= -0.9f && !other.gameObject.GetComponent<Mario>().isInvulnerable)
                    {
                        timer = 0f;
                        other.gameObject.GetComponent<Mario>().Rebote();
                        if (canMoveAgain==true)
                        {
                            
                            other.gameObject.GetComponent<Mario>().TriggerInvulnerability();
                            GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                        }
                        else
                        {
                          
                            GetComponent<Rigidbody2D>().velocity = new Vector2(koopa.moveSpeed, 0);
                            other.gameObject.GetComponent<Mario>().TriggerInvulnerability();
                            
                        }
                        canMoveAgain = !canMoveAgain;

                    }
                    else 
                    {
                        if (!other.gameObject.GetComponent<Mario>().isInvulnerable)
                        {
                            Time.timeScale = 0f; // Pausa el juego
                            other.gameObject.GetComponent<Mario>().DeathMario(); // Mario muere
                        }
                       
                    }

                }
            }
        }
    }

    private void EnterShell()
    {
        // Activa la animación de entrar en el caparazón
        animator.SetTrigger("Shell");
        isShell = true;
        
        timer = 0f; // Reinicia el temporizador
    }

    private void ExitShell()
    {
        // El Koopa sale del caparazón
        animator.SetTrigger("WalkAgain"); // Asegúrate de tener una animación para esto
        isShell = false;
        canMoveAgain = true;
        koopa.moveSpeed = 2f;// Permite al Koopa caminar nuevamente
        timer = 0f; // Reinicia el temporizador
    }

    public void WaitToDie()
    {
        Destroy(gameObject);
    }
}
