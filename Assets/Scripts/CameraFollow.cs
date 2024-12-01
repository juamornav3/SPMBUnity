using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;  // El jugador (Mario)
    public float smoothSpeed = 0.125f;  // Velocidad de interpolaci�n para suavidad
    public Vector3 offset;  // Offset de la c�mara
    private float maxCameraX;  // L�mite m�ximo alcanzado por la c�mara en X

    void Start()
    {
        // Inicializar el l�mite en la posici�n inicial de la c�mara
        maxCameraX = transform.position.x;
    }

    void FixedUpdate()
    {
        if (target != null)
        {
            // Calcular la posici�n deseada
            Vector3 desiredPosition = new Vector3(target.position.x + offset.x, transform.position.y, transform.position.z);

            // Actualizar el l�mite m�ximo de la c�mara en X si Mario avanza
            if (desiredPosition.x > maxCameraX)
            {
                maxCameraX = desiredPosition.x;
            }

            // Limitar la c�mara a no retroceder m�s all� del l�mite
            desiredPosition.x = Mathf.Max(desiredPosition.x, maxCameraX);

            // Suavizar el movimiento
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

            // Actualizar la posici�n de la c�mara
            transform.position = smoothedPosition;
        }
    }
}
