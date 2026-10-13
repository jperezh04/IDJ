using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drive : MonoBehaviour {
    [Header("Movement")]
    public float speed = 10.0f;
    public float rotationSpeed = 100.0f;

    [Header("Gun Aiming & Shooting")]
    public Transform transGun;
    public Transform gun;
    public GameObject bulletObj;

    void Update() {
        // Translation along object's Z-axis (forward/backward: W/S or Up/Down arrows)
        float translation = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        // Rotation around Y-axis (steering: A/D or Left/Right arrows)
        float rotation = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;

        transform.Translate(0, 0, translation);
        transform.Rotate(0, rotation, 0);

        if (transGun != null) {
            if (Input.GetKey(KeyCode.T)) {
                transGun.RotateAround(transGun.position, transGun.right, -2.0f);
            } else if (Input.GetKey(KeyCode.G)) {
                transGun.RotateAround(transGun.position, transGun.right, 2.0f);
            }
        }

        if (Input.GetKeyDown(KeyCode.B) && gun != null && bulletObj != null) {
            Instantiate(bulletObj, gun.position, gun.rotation);
        }
    }
}
