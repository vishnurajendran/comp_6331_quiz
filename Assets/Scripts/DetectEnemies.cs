using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectEnemies : MonoBehaviour
{
    private IndividualAI individualAI;

    // Start is called before the first frame update
    void Start()
    {
        individualAI = this.gameObject.transform.GetComponentInParent<IndividualAI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Floor"))
        {
            return;
        }

        if (other != null){
                   
            if (this.gameObject.transform.parent.CompareTag("Rock"))
            {
                if (other.CompareTag("Paper")){
                    individualAI.closestEnemy = other.gameObject.transform;
                }
            }

            if (this.gameObject.transform.parent.CompareTag("Paper"))
            {
                if (other.CompareTag("Scissors"))
                {
                    individualAI.closestEnemy = other.gameObject.transform;
                }
            }
            if (this.gameObject.transform.parent.CompareTag("Scissors"))
            {
                if (other.CompareTag("Rock"))
                {
                    individualAI.closestEnemy = other.gameObject.transform;
                }
            }
        }
    }
}
