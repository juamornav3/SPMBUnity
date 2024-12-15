using System.Collections;
using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
    // Referencia a los objetos con el componente Animator y Rigidbody2D
    private Animator objectAnimator;
    private Rigidbody2D objectRigidbody;

   

    // Este m�todo se llama cuando un objeto con el tag "Player" o "Enemy" entra en contacto con el collider del precipicio
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Enemy"))
        {
            // Obtener el Animator y Rigidbody2D del objeto que cay�
            objectAnimator = collision.GetComponent<Animator>();
            objectRigidbody = collision.GetComponent<Rigidbody2D>();

            if (objectAnimator != null && objectRigidbody != null)
            {
                TriggerDeath(collision.tag);
            }
        }
    }

    private void TriggerDeath(string tag)
    {
        // Pausar el resto del juego
        if (tag=="Player")
        {
            Time.timeScale = 0f; // Pausar el juego, pero el personaje sigue con su animaci�n
        }
        

        // Detener el movimiento del personaje (tanto para Mario como para enemigos)
        objectRigidbody.velocity = Vector2.zero;
        objectRigidbody.isKinematic = true; // Detener la f�sica del objeto

        // Activar la animaci�n de muerte
        objectAnimator.SetTrigger("Die");

        // Llamar al script de pausa para manejar la muerte
        //pauseScript.OnPlayerDeath();  // Esto se puede personalizar si tienes diferentes comportamientos para enemigos

        // Iniciar la animaci�n de muerte y esperar a que termine
        StartCoroutine(PlayDeathAnimation());
    }

    // Corutina para reproducir la animaci�n de muerte sin ser afectada por Time.timeScale
    private IEnumerator PlayDeathAnimation()
    {
        // Duraci�n de la animaci�n de muerte
        float deathAnimDuration = objectAnimator.GetCurrentAnimatorStateInfo(0).length;

        // Esperar a que termine la animaci�n
        float timer = 0f;
        while (timer < deathAnimDuration)
        {
            timer += Time.unscaledDeltaTime; // Usar el tiempo no escalado
            yield return null;
        }

        // Aqu� puedes poner la l�gica para lo que suceda despu�s de la muerte del personaje
        Debug.Log("El objeto ha muerto por caer en el precipicio.");
        // Aqu� podr�as cargar una escena de Game Over o reiniciar el nivel.
    }
}
