using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mario : MonoBehaviour
{

        public float moveSpeed = 5f; // Velocidad de movimiento horizontal
        public float jumpForce = 10f; // Fuerza del salto
        public LayerMask groundLayer; // Capas que serán consideradas como suelo

        private Rigidbody2D rb;
        private bool isGrounded;
        private float moveInput;

        // Variables de Coyote Time (tiempo de gracia para saltar)
        public float coyoteTime = 0.2f; // Tiempo de gracia para saltar después de dejar el suelo
        private float coyoteTimeCounter; // Contador para el coyote time

        // Parámetros para el Raycast
        public float groundCheckDistance = 0.5f; // Distancia del Raycast hacia abajo
        public Transform groundCheck; // Punto desde donde se lanza el Raycast

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.freezeRotation = true;
        }

        void Update()
        {
            // Detectar el input horizontal (izquierda/derecha)
            moveInput = Input.GetAxisRaw("Horizontal");

            // Movimiento horizontal
            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

            // Comprobar si el personaje está tocando el suelo usando un Raycast
            isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);

            // Manejar el Coyote Time (tiempo de gracia)
            if (isGrounded)
            {
            
                coyoteTimeCounter = coyoteTime; // Reinicia el contador si está en el suelo
            }
            else
            {
                coyoteTimeCounter -= Time.deltaTime; // Disminuye el tiempo si no está en el suelo
            }

            // Saltar si el jugador está en el suelo o dentro del Coyote Time
            if ((isGrounded || coyoteTimeCounter > 0f) && Input.GetButtonDown("Jump"))
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
        }

        // Para visualizar el Raycast en el editor
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckDistance);
        }
    
}
