using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class Seek : MonoBehaviour
{

    public Transform target;
    public float maxSpeed;

    private Vector3 Velocity_V3;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb=this.gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (target != null)
        {
            Velocity_V3 = Vector3.Normalize(target.position- transform.position);
            rb.velocity = Velocity_V3*maxSpeed;
        }

    }


}
