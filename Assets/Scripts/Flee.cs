using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flee : MonoBehaviour
{

    public Transform target;
    public float maxSpeed;

    private Vector3 Velocity_V3;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.gameObject.GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void FixedUpdate()
    {
        if (target != null)
        {
            Vector3 speedVector = new Vector3();
            Velocity_V3 = Vector3.Normalize(transform.position - target.position);
            speedVector = Velocity_V3 * maxSpeed;
            rb.velocity = speedVector;
        }

    }
}
