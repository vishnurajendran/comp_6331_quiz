using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class IndividualAI : MonoBehaviour
{
    [SerializeField] private GroupAI.AIof myType;
    public Transform target;
    public Transform closestEnemy;
    [FormerlySerializedAs("friends")] public List<Transform> nearbyFriends;

    [SerializeField] private float maxRadius = 75;
    private float agresiveness;

    private Vector3 Velocity_V3;
    private Rigidbody rb;
    public float safeDistance;

    private SphereCollider _sphereCollider;
    private bool isBoosting = false;

    private Coroutine boostActiveRoutine;
    private float boostChargeTimeLeft = 0;


    public float Agresiveness { get => agresiveness; set => agresiveness = value; }

    private void Awake()
    {
        nearbyFriends = new List<Transform>();
        GameManager.OnGameStateChanged += GameManagerOnGameStateChanged;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManagerOnGameStateChanged;
    }

    private void GameManagerOnGameStateChanged(GameState state)
    {
        return;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = this.gameObject.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        var centre = new Vector3(0, transform.position.y, 0);
        var distFromCentre = Vector3.Distance(transform.position, centre);
        if (distFromCentre > maxRadius)
        {
            var ranPos = Random.insideUnitCircle;
            rb.position = new Vector3(ranPos.x, transform.position.y, ranPos.y);
            Debug.Log($"{name} Teleported to {rb.position.ToString()}");
        }
        
        if (boostChargeTimeLeft > 0)
        {
            boostChargeTimeLeft -= Time.fixedDeltaTime;
            if (boostChargeTimeLeft < 0)
                boostChargeTimeLeft = 0;
        }
        
        if (target == null || !target.gameObject.activeSelf)
        {
            target = GroupAI.GetGroupAIForType(myType)?.GetClosestTarget() ?? null;
        }
        Vector3 speedVector = new Vector3();
        
        if (closestEnemy != null)
        {
            if (!isBoosting && boostActiveRoutine == null && boostChargeTimeLeft <= 0)
            {
                boostActiveRoutine = StartCoroutine(BoostCoroutine());
            }
            
            Velocity_V3 = Vector3.Normalize(transform.position-closestEnemy.position);
            if (Vector3.Distance(closestEnemy.position, this.transform.position) > safeDistance || !closestEnemy.gameObject.activeSelf)
            {
                closestEnemy = null;
            }
        }
        else if(target != null)
        {
            Velocity_V3 = Vector3.Normalize(target.position- transform.position);
            if ( Vector3.Distance(target.position, transform.position) >= 7.5f)
            {
                target = null;
            }
        }

        var friendAvoidance = Vector3.zero;
        foreach (var friend in nearbyFriends)
        {
            if (!friend)
                continue;

            var dist = Vector3.Distance(transform.position, friend.position);
            if (dist <= 3.5f)
            {
                var dir = (transform.position - friend.position).normalized;
                friendAvoidance += dir;
            }
        }
        
        friendAvoidance = friendAvoidance.normalized;
        Velocity_V3 = (Velocity_V3 + (1 * friendAvoidance).normalized).normalized;
         speedVector = Velocity_V3 * (agresiveness + (isBoosting?2:0));
        rb.velocity = speedVector;
    }

    IEnumerator BoostCoroutine()
    {
        target = null; // no more target, its to get a new one later
        isBoosting = true;
        yield return new WaitForSeconds(2);
        boostChargeTimeLeft = 3;
        isBoosting = false;
        boostActiveRoutine = null;
    }
    
    private void OnDrawGizmos()
    {
        var centre = new Vector3(0, transform.position.y, 0);
#if UNITY_EDITOR
        var color = Color.gray;
        color.a = 0.015f;
        Handles.color = color;
        Handles.DrawSolidDisc(centre, Vector3.up, maxRadius);
        
        if (!Application.isPlaying)
        {
           
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, centre);
            Handles.Label(transform.position, $"Dist To Centre: {Vector3.Distance(transform.position, centre)}");
            return;
        }
            
        
        if(_sphereCollider == null)
            _sphereCollider = GetComponent<SphereCollider>();
        var col = Color.white;
        if (tag.Equals("Scissors"))
        {
            col = Color.red;
        }else if (tag.Equals("Paper"))
        {
            col = Color.green;
        }
        else if (tag.Equals("Rock"))
        {
            col = Color.blue;
        }

        col.a = 0.25f;
        Gizmos.color = col;
        Gizmos.DrawSphere(transform.position, _sphereCollider.radius);

        if (target != null && target.gameObject.activeSelf)
        {
            Gizmos.DrawLine(transform.position, target.position);
        }

        if (closestEnemy != null && closestEnemy.gameObject.activeSelf)
        {
            Gizmos.DrawLine(transform.position, closestEnemy.position);
        }
#endif
    }
}
