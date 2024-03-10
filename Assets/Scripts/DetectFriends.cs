using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectFriends : MonoBehaviour
{
    private IndividualAI individualAI;
    void Start()
    {
        individualAI = this.gameObject.transform.GetComponentInParent<IndividualAI>();
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
                if (other.CompareTag("Rock")){
                   Add(other.transform);
                }
            }

            if (this.gameObject.transform.parent.CompareTag("Paper"))
            {
                if (other.CompareTag("Paper")){
                    Add(other.transform);
                }
            }
            if (this.gameObject.transform.parent.CompareTag("Scissors"))
            {
                if (other.CompareTag("Scissors")){
                    Add(other.transform);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Floor"))
        {
            return;
        }

        if (other != null){
                   
            if (this.gameObject.transform.parent.CompareTag("Rock"))
            {
                if (other.CompareTag("Rock")){
                    Remove(other.transform);
                }
            }

            if (this.gameObject.transform.parent.CompareTag("Paper"))
            {
                if (other.CompareTag("Paper")){
                    Remove(other.transform);
                }
            }
            if (this.gameObject.transform.parent.CompareTag("Scissors"))
            {
                if (other.CompareTag("Scissors")){
                    Remove(other.transform);
                }
            }
        }
    }

    private void Add(Transform other)
    {
        if (!individualAI.nearbyFriends.Contains(other))
        {
            individualAI.nearbyFriends.Add(other);
        }
    }
    
    private void Remove(Transform other)
    {
        if (individualAI.nearbyFriends.Contains(other))
        {
            individualAI.nearbyFriends.Remove(other);
        }
    }
}
