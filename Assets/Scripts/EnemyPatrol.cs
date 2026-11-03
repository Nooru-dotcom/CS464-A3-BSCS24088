using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyPatrol : MonoBehaviour
{
    public enum GuardState { Patrol, Chase, Return }

    [Header("Patrol Waypoints")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private bool pingPong = false;
    [SerializeField] private float arriveDistance = 0.3f;

    [Header("Movement Speeds")]
    [SerializeField] private float patrolSpeed = 2.5f;
    [SerializeField] private float chaseSpeed = 4.0f; // Slower than player speed (7 m/s)

    [Header("Detection Ranges")]
    [SerializeField] private float chaseRange = 5.0f;
    [SerializeField] private float giveUpRange = 8.0f; // Longer than chaseRange to prevent edge flickering

    private GuardState currentState = GuardState.Patrol;
    private int currentWaypointIndex = 0;
    private int pingPongDirection = 1;
    private Transform player;
    private Rigidbody rb;

    public GuardState CurrentState => currentState;
    public float PatrolSpeed => patrolSpeed;
    public float ChaseSpeed => chaseSpeed;
    public float ChaseRange => chaseRange;
    public float GiveUpRange => giveUpRange;

    public void SetWaypoints(Transform[] newWaypoints, bool isPingPong = false)
    {
        waypoints = newWaypoints;
        pingPong = isPingPong;
        currentWaypointIndex = 0;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    private void Start()
    {
        // Player found in Start by FindAnyObjectByType, so one prefab fits all levels
        PlayerController pc = Object.FindAnyObjectByType<PlayerController>();
        if (pc != null)
        {
            player = pc.transform;
        }
    }

    private void Update()
    {
        if (player == null)
        {
            PlayerController pc = Object.FindAnyObjectByType<PlayerController>();
            if (pc != null) player = pc.transform;
            if (player == null) return;
        }

        switch (currentState)
        {
            case GuardState.Patrol:
                UpdatePatrol();
                break;
            case GuardState.Chase:
                UpdateChase();
                break;
            case GuardState.Return:
                UpdateReturn();
                break;
        }
    }

    private void UpdatePatrol()
    {
        // Check if player entered detection zone
        if (InRange(chaseRange))
        {
            currentState = GuardState.Chase;
            return;
        }

        if (waypoints == null || waypoints.Length == 0) return;

        Vector3 targetPos = GetWaypointPosition(currentWaypointIndex);
        MoveTowardsTarget(targetPos, patrolSpeed);

        // Arrival by distance, not ==
        if (Arrived(targetPos))
        {
            AdvanceWaypoint();
        }
    }

    private void UpdateChase()
    {
        // Only outside the longer giveUpRange does it give up and walk back
        if (!InRange(giveUpRange))
        {
            currentState = GuardState.Return;
            return;
        }

        Vector3 targetPos = new Vector3(player.position.x, transform.position.y, player.position.z);
        MoveTowardsTarget(targetPos, chaseSpeed);
    }

    private void UpdateReturn()
    {
        // If player steps back into chaseRange while returning, re-engage
        if (InRange(chaseRange))
        {
            currentState = GuardState.Chase;
            return;
        }

        if (waypoints == null || waypoints.Length == 0)
        {
            currentState = GuardState.Patrol;
            return;
        }

        Vector3 targetPos = GetWaypointPosition(currentWaypointIndex);
        MoveTowardsTarget(targetPos, patrolSpeed);

        // Arrived back at patrol route
        if (Arrived(targetPos))
        {
            currentState = GuardState.Patrol;
        }
    }

    private void MoveTowardsTarget(Vector3 target, float speed)
    {
        Vector3 newPos = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        newPos.y = transform.position.y; // Keep guard on horizontal plane
        transform.position = newPos;

        Vector3 dir = (target - transform.position);
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 10f * Time.deltaTime);
        }
    }

    private Vector3 GetWaypointPosition(int index)
    {
        if (waypoints == null || index < 0 || index >= waypoints.Length || waypoints[index] == null)
        {
            return transform.position;
        }
        return new Vector3(waypoints[index].position.x, transform.position.y, waypoints[index].position.z);
    }

    private void AdvanceWaypoint()
    {
        if (waypoints == null || waypoints.Length <= 1) return;

        if (pingPong)
        {
            currentWaypointIndex += pingPongDirection;
            if (currentWaypointIndex >= waypoints.Length)
            {
                currentWaypointIndex = waypoints.Length - 2;
                pingPongDirection = -1;
            }
            else if (currentWaypointIndex < 0)
            {
                currentWaypointIndex = 1;
                pingPongDirection = 1;
            }
        }
        else
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }

    // Arrival by distance, not == (prompt slide 10)
    private bool Arrived(Vector3 target) =>
        Vector3.Distance(new Vector3(transform.position.x, 0f, transform.position.z),
                         new Vector3(target.x, 0f, target.z)) < arriveDistance;

    // Seen inside one range, lost outside a longer one (prompt slide 13)
    private bool InRange(float range) =>
        Vector3.Distance(new Vector3(transform.position.x, 0f, transform.position.z),
                         new Vector3(player.position.x, 0f, player.position.z)) < range;
}
