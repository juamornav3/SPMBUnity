using System.Collections;
using UnityEngine;

public class Mario : MonoBehaviour
{

    [Header("Movimiento")]
    public float moveSpeed = 10f; // Velocidad normal de movimiento horizontal
    public float runMultiplier = 2f; // Multiplicador de velocidad al correr
    public float acceleration = 3f; // Velocidad de aceleraci�n
    public float deceleration = 2f; // Velocidad de desaceleraci�n
    public float normalAnimatorSpeed;

    [Header("Salto")]
    public float jumpForce = 10f; // Fuerza base del salto
    public float jumpBoostForce = 5f; // Fuerza adicional mientras se mantiene presionado
    public float maxJumpTime = 0.5f; // Tiempo m�ximo que se puede mantener el bot�n para saltar alto
    public float fallMultiplier = 2f; // Multiplicador para acelerar la ca�da

    [Header("Rebote")]
    [SerializeField] private float velocidadRebote;

    [Header("Tiempo de Gracia (Coyote Time)")]
    public float coyoteTime = 0.2f; // Tiempo de gracia para saltar despu�s de dejar el suelo
    private float coyoteTimeCounter; // Contador para el coyote time

    [Header("Raycasts")]
    public float groundCheckDistance = 0.5f; // Distancia del Raycast hacia abajo
    public float wallCheckDistance = 0.2f;
    public Transform groundCheckCenter; // Punto central para el Raycast
    public Transform groundCheckLeft; // Punto izquierdo para el Raycast
    public Transform groundCheckRight; // Punto derecho para el Raycast
    public Transform wallCheckRight; // Puntos de raycast para detectar paredes
    public Transform wallCheckRightBottom;
    public Transform wallCheckRightHead;
    public Transform wallCheckLeft;

    [Header("L�mites de Pantalla")]
    public float leftScreenLimit = -10f; // L�mite inicial del borde izquierdo
    private Camera mainCamera;

    [Header("Capas")]
    public LayerMask groundLayer; // Capas que ser�n consideradas como suelo
    public LayerMask wallLayer;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isTouchingWall;
    private float moveInput;
    private float currentSpeed = 0f; // Velocidad horizontal actual

    private float jumpTimeCounter; // Contador para el tiempo de salto
    private bool isJumping; // Indica si el personaje est� en un salto
    private Animator animator;
    private int previousDirection = 0; // Direcci�n anterior del personaje

   

    void Start()
    {
        
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        mainCamera = Camera.main; // Obtener la c�mara principal
    }

    private void Update()
    {
        HandleOrientation();
        PlayerMovement();
        HandleJumpPhysics();

        // Calcular el borde izquierdo de la pantalla basado en la posici�n de la c�mara
        float cameraLeftEdge = mainCamera.transform.position.x - mainCamera.orthographicSize * mainCamera.aspect;
        leftScreenLimit = cameraLeftEdge; // Actualizar el l�mite izquierdo
    }

    

    public void Rebote()
    {
        rb.velocity = new Vector2(rb.velocity.x,velocidadRebote);
    }

   public void DeathMario()
    {
        // Activar la animaci�n de muerte
        animator.SetTrigger("Die");

        // Detener el movimiento de Mario
        rb.velocity = Vector2.zero;

        // Hacer que Mario deje de moverse con la f�sica
        rb.isKinematic = true;

        // Asegurarse de que la animaci�n de muerte se maneje sin interrupciones
        StartCoroutine(PlayDeathAnimation());
    }

    
    public void TakeDamage()
    {
       
        
    }

    private void HandleOrientation()
    {
        if (isGrounded)
        {
            // Cambiar la escala de Mario para reflejar su direcci�n
            if (currentSpeed < 0)
            {
                if (previousDirection > 0)
                {
                    animator.SetTrigger("change");
                }
                previousDirection = -1;
                transform.localScale = new Vector3(-1, 1, 1); // Mirar a la izquierda
            }
            else if (currentSpeed > 0)
            {
                if (previousDirection < 0)
                {
                    animator.SetTrigger("change");
                }
                previousDirection = 1;
                transform.localScale = new Vector3(1, 1, 1); // Mirar a la derecha
            }
        }
    }

    

    private void PlayerMovement()
    {
        // Detectar el input horizontal (izquierda/derecha)
        moveInput = Input.GetAxisRaw("Horizontal");

        // Detectar si se est� presionando Shift para correr
        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        // Determinar la velocidad m�xima (correr o caminar)
        float targetSpeed = isRunning ? moveSpeed * runMultiplier : moveSpeed;

        animator.SetFloat("walking", Mathf.Abs(currentSpeed));

        // Comprobar si el personaje est� tocando el suelo usando tres Raycasts
        isGrounded = Physics2D.Raycast(groundCheckCenter.position, Vector2.down, groundCheckDistance, groundLayer) ||
                     Physics2D.Raycast(groundCheckLeft.position, Vector2.down, groundCheckDistance, groundLayer) ||
                     Physics2D.Raycast(groundCheckRight.position, Vector2.down, groundCheckDistance, groundLayer);

        // Detectar si Mario est� tocando una pared
        isTouchingWall = Physics2D.Raycast(wallCheckRight.position, Vector2.right, wallCheckDistance, wallLayer) ||
                         Physics2D.Raycast(wallCheckRightBottom.position, Vector2.right, wallCheckDistance, wallLayer) ||
                         Physics2D.Raycast(wallCheckRightHead.position, Vector2.right, wallCheckDistance, wallLayer) ||
                         Physics2D.Raycast(wallCheckLeft.position, Vector2.left, wallCheckDistance, wallLayer);

        // L�gica de movimiento horizontal con restricci�n
        if (transform.position.x > leftScreenLimit || moveInput > 0)
        {
            // Permitir el movimiento hacia adelante o evitar retroceder m�s all� del l�mite
            if (moveInput != 0)
            {
                currentSpeed = Mathf.Lerp(currentSpeed, moveInput * targetSpeed, acceleration * Time.deltaTime);
            }
            else
            {
                currentSpeed = Mathf.Lerp(currentSpeed, 0f, deceleration * Time.deltaTime);
            }
        }
        else
        {
            // Si est� en el borde izquierdo, detener el movimiento hacia la izquierda
            currentSpeed = Mathf.Max(0, currentSpeed);
        }

        // Movimiento horizontal
        if (isTouchingWall && !isGrounded)
        {
            rb.velocity = new Vector2(0, rb.velocity.y); // Detener movimiento horizontal si est� en la pared
        }
        else
        {
            rb.velocity = new Vector2(currentSpeed, rb.velocity.y);
        }

        animator.SetBool("onGround", isGrounded);

        // Iniciar un salto si est� en el suelo o dentro del coyote time
        if ((isGrounded || coyoteTimeCounter > 0f) && Input.GetButtonDown("Jump"))
        {
            isJumping = true;
            jumpTimeCounter = maxJumpTime; // Reinicia el contador del salto
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    private void HandleJumpPhysics()
    {
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float jumpBoostRunning = isRunning ? 1.2f : 1f;

        if (isJumping && Input.GetButton("Jump"))
        {
            if (jumpTimeCounter > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, (jumpForce + jumpBoostForce) * jumpBoostRunning);
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }

        if (Input.GetButtonUp("Jump"))
        {
            isJumping = false;
        }

        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime; // Reinicia el contador
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawLine(groundCheckCenter.position, groundCheckCenter.position + Vector3.down * groundCheckDistance);
        Gizmos.DrawLine(groundCheckLeft.position, groundCheckLeft.position + Vector3.down * groundCheckDistance);
        Gizmos.DrawLine(groundCheckRight.position, groundCheckRight.position + Vector3.down * groundCheckDistance);

        float rayDirection = transform.localScale.x > 0 ? 1f : -1f;
        Gizmos.DrawLine(wallCheckRight.position, wallCheckRight.position + Vector3.right * rayDirection * wallCheckDistance);
        Gizmos.DrawLine(wallCheckRightBottom.position, wallCheckRightBottom.position + Vector3.right * rayDirection * wallCheckDistance);
        Gizmos.DrawLine(wallCheckRightHead.position, wallCheckRightHead.position + Vector3.right * rayDirection * wallCheckDistance);
        Gizmos.DrawLine(wallCheckLeft.position, wallCheckLeft.position + Vector3.left * rayDirection * wallCheckDistance);

        // Visualizar el l�mite izquierdo
        Gizmos.DrawLine(new Vector3(leftScreenLimit, -10f, 0f), new Vector3(leftScreenLimit, 10f, 0f));
    }

    // Corutina para reproducir la animaci�n de muerte sin ser afectada por Time.timeScale
    IEnumerator PlayDeathAnimation()
    {
        float deathAnimDuration = animator.GetCurrentAnimatorStateInfo(0).length; // Duraci�n de la animaci�n de muerte

        // Esperar hasta que termine la animaci�n
        float timer = 0f;
        while (timer < deathAnimDuration)
        {
            timer += Time.unscaledDeltaTime; // Usar el tiempo no escalado
            yield return null;
        }

        // Aqu� puedes poner la l�gica para lo que suceda despu�s de que Mario muera
        Debug.Log("Mario ha muerto. Fin de la animaci�n.");
        Destroy(gameObject);
        // Puedes cargar una escena de Game Over o cualquier otro comportamiento aqu�.
    }
}
