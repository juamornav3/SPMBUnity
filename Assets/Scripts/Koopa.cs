using UnityEngine;

public class Koopa : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 2f; // Velocidad de movimiento

    [Header("Raycasts")]
    public float wallCheckDistance = 0.2f; // Distancia para detectar paredes
    public Transform wallCheckPoint; // Punto desde donde se realiza el Raycast
    public Transform groundCheckPoint; // Punto para verificar el suelo
    public float groundCheckDistance = 0.2f; // Distancia del raycast para el suelo

    [Header("Capas")]
    public LayerMask wallLayer; // Capas que serán consideradas como paredes
    public LayerMask groundLayer; // Capas que serán consideradas como suelo

    private Rigidbody2D rb;
    public bool movingRight = false; // Dirección inicial: a la izquierda

    private HitKoopa hitKoopa;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        hitKoopa = GetComponent<HitKoopa>();
    }

    void Update()
    {
        if (hitKoopa.canMoveAgain)
        {
            // Aplicar movimiento
            rb.velocity = new Vector2((movingRight ? 1f : -1f) * moveSpeed, rb.velocity.y);
        }
        // Dirección del Raycast dinámicamente según el movimiento
        Vector2 rayDirection = movingRight ? Vector2.right : Vector2.left;

        // Verificar colisión con pared o fin del suelo
        bool hittingWall = Physics2D.Raycast(wallCheckPoint.position, rayDirection, wallCheckDistance, wallLayer);
        bool groundAhead = Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckDistance, groundLayer);

        // Cambiar dirección si golpea una pared o si no hay suelo delante
        if (hittingWall)
        {
            movingRight = !movingRight; // Cambiar la dirección
            Flip();
        }

       
    }

    public void Flip()
    {
        // Cambiar la escala para reflejar la dirección
        transform.localScale = new Vector3(movingRight ? -1 : 1, 1, 1);

        // Ajustar el punto de raycasting dinámicamente cada vez
        UpdateRaycastPositions();
    }

    void UpdateRaycastPositions()
    {
        Vector3 wallCheckLocalPos = wallCheckPoint.localPosition;


        // Ajustar la posición en el eje X para reflejar la nueva dirección
        wallCheckLocalPos.x = Mathf.Abs(wallCheckLocalPos.x) * (movingRight ? 1 : -1);


        // Actualizar las posiciones locales
        wallCheckPoint.localPosition = wallCheckLocalPos;


    }

    private void OnDrawGizmos()
    {
        if (wallCheckPoint != null && groundCheckPoint != null)
        {
            Gizmos.color = Color.red;

            // Raycast para verificar paredes
            Vector2 rayDirection = movingRight ? Vector2.right : Vector2.left;
            Gizmos.DrawLine(wallCheckPoint.position, wallCheckPoint.position + (Vector3)rayDirection * wallCheckDistance);

            // Raycast para verificar el suelo
            Gizmos.color = Color.green;
            Gizmos.DrawLine(groundCheckPoint.position, groundCheckPoint.position + Vector3.down * groundCheckDistance);
        }
    }
}
