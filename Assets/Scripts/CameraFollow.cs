using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;  // El jugador (Mario)
    public float smoothSpeed = 0.125f;  // Velocidad de interpolación para suavidad
    public Vector3 offset;  // Offset de la cámara
    private float maxCameraX;  // Límite máximo alcanzado por la cámara en X

    void Start()
    {
        // Inicializar el límite en la posición inicial de la cámara
        maxCameraX = transform.position.x;
    }

    void FixedUpdate()
    {
        if (target != null)
        {
            // Calcular la posición deseada
            Vector3 desiredPosition = new Vector3(target.position.x + offset.x, transform.position.y, transform.position.z);

            // Actualizar el límite máximo de la cámara en X si Mario avanza
            if (desiredPosition.x > maxCameraX)
            {
                maxCameraX = desiredPosition.x;
            }

            // Limitar la cámara a no retroceder más allá del límite
            desiredPosition.x = Mathf.Max(desiredPosition.x, maxCameraX);

            // Suavizar el movimiento
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

            // Actualizar la posición de la cámara
            transform.position = smoothedPosition;
        }
    }
}
