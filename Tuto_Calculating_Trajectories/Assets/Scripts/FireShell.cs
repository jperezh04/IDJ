using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireShell : MonoBehaviour {

    [Header("References")]
    public GameObject bullet;
    public GameObject turret;
    public GameObject enemy;
    public Transform turretBase;

    [Header("Trajectory & Firing Settings")]
    [Tooltip("Initial speed of the projectile")]
    public float speed = 15.0f;
    [Tooltip("Rotation speed of the tank towards enemy")]
    public float rotSpeed = 5.0f;
    [Tooltip("Movement speed when pursuing enemy out of range")]
    public float moveSpeed = 1.0f;
    [Tooltip("Delay between shots in seconds")]
    public float delayReset = 0.5f;
    [Tooltip("Use low trajectory arc if true, or high trajectory arc if false")]
    public bool useLowAngle = false;
    [Tooltip("Distance adjustment fudge factor to improve hit accuracy")]
    public float distanceFudge = 1.0f;

    private float delay = 0.0f;

    void CreateBullet() {
        if (bullet == null || turret == null || turretBase == null) return;

        GameObject shell = Instantiate(bullet, turret.transform.position, turret.transform.rotation);
        Rigidbody rb = shell.GetComponent<Rigidbody>();
        if (rb != null) {
#if UNITY_6000_0_OR_NEWER
            rb.linearVelocity = speed * turretBase.forward;
#else
            rb.velocity = speed * turretBase.forward;
#endif
        }
    }

    float? RotateTurret() {
        if (turretBase == null) return null;

        float? angle = CalculateAngle(useLowAngle);

        if (angle != null) {
            turretBase.localEulerAngles = new Vector3(360.0f - (float)angle, 0.0f, 0.0f);
        }
        return angle;
    }

    float? CalculateAngle(bool low) {
        if (enemy == null) return null;

        Vector3 targetDir = enemy.transform.position - this.transform.position;
        float y = targetDir.y;
        targetDir.y = 0.0f;
        float x = Mathf.Max(0.01f, targetDir.magnitude - distanceFudge);
        float gravity = 9.8f;
        float sSqr = speed * speed;
        float underTheSqrRoot = (sSqr * sSqr) - gravity * (gravity * x * x + 2 * y * sSqr);

        if (underTheSqrRoot >= 0.0f) {
            float root = Mathf.Sqrt(underTheSqrRoot);
            float highAngle = sSqr + root;
            float lowAngle = sSqr - root;

            if (low) return (Mathf.Atan2(lowAngle, gravity * x) * Mathf.Rad2Deg);
            else return (Mathf.Atan2(highAngle, gravity * x) * Mathf.Rad2Deg);
        } else {
            return null;
        }
    }

    void Update() {
        if (enemy == null) return;

        delay -= Time.deltaTime;
        Vector3 direction = (enemy.transform.position - this.transform.position).normalized;
        if (direction.sqrMagnitude > 0.001f) {
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0.0f, direction.z));
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, lookRotation, Time.deltaTime * rotSpeed);
        }

        float? angle = RotateTurret();

        if (angle != null && delay <= 0.0f) {
            CreateBullet();
            delay = delayReset;
        } else if (angle == null) {
            this.transform.Translate(0.0f, 0.0f, Time.deltaTime * moveSpeed);
        }
    }

    //Vector3 CalculateTrajectory() {

    //    Vector3 p = enemy.transform.position - this.transform.position;
    //    Vector3 v = enemy.transform.forward * enemy.GetComponent<Drive>().speed;
    //    float s = bullet.GetComponent<MoveShell>().speed;

    //    float a = Vector3.Dot(v, v) - s * s;
    //    float b = Vector3.Dot(p, v);
    //    float c = Vector3.Dot(p, p);
    //    float d = b * b - a * c;

    //    if (d < 0.1f) return Vector3.zero;

    //    float sqrt = Mathf.Sqrt(d);
    //    float t1 = (-b - sqrt) / c;
    //    float t2 = (-b + sqrt) / c;

    //    float t = 0.0f;
    //    if (t1 < 0.0f && t2 < 0.0f) return Vector3.zero;
    //    else if (t1 < 0.0f) t = t2;
    //    else if (t2 < 0.0f) t = t1;
    //    else {

    //        t = Mathf.Max(new float[] { t1, t2 });
    //    }
    //    return t * p + v;
    //}
}
