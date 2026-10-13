using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIShell : MonoBehaviour {

    public GameObject explosion;
    private Rigidbody rb;

    void Start() {
        rb = GetComponent<Rigidbody>();
    }

    void Update() {
        if (rb != null) {
#if UNITY_6000_0_OR_NEWER
            Vector3 vel = rb.linearVelocity;
#else
            Vector3 vel = rb.velocity;
#endif
            if (vel.sqrMagnitude > 0.01f) {
                this.transform.forward = vel;
            }
        }
    }

    void OnCollisionEnter(Collision col) {
        if (col.gameObject.CompareTag("tank")) {
            Debug.Log("Hit tank: " + col.gameObject.name);
            if (explosion != null) {
                GameObject exp = Instantiate(explosion, this.transform.position, Quaternion.identity);
                Destroy(exp, 0.5f);
            }
            Destroy(this.gameObject);
        }
    }
}
