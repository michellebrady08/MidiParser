using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float velocidadCamara = 100.0f; // Velocidad de movimiento de la cámara.
    public GameObject Staff;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.right * velocidadCamara * Time.deltaTime);
        Staff.transform.Translate(Vector3.right * velocidadCamara * Time.deltaTime);
    }
}
