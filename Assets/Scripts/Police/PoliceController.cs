using System;
using System.Collections;
using UnityEngine;

namespace Police
{
    public class PoliceController : MonoBehaviour
    {
        public static event Action OnPlayerCaught;

        [SerializeField] private Transform pathHolder;
        [SerializeField] private float moveSpeed = 1.65f;
        [SerializeField] private float turnSpeed = 90f;
        [SerializeField] private float waitTime = .3f;

        [SerializeField] private Light spotLight;
        [SerializeField] private float viewDistance = 10f;
        [SerializeField] private float timeToSpotPlayer = .5f;
        [SerializeField] private LayerMask viewMask;

        private Transform player;
        private Color originalLightColor;
        private float viewAngle;
        private float playerVisibleTimer;
        public static bool WalkState;
    
        private void Start()
        {
            originalLightColor = spotLight.color;
            player = GameObject.FindGameObjectWithTag("Player").transform;
            viewAngle = spotLight.spotAngle;

            var wayPoints = new Vector3[pathHolder.childCount];

            for (var i = 0; i < wayPoints.Length; i++)
            {
                wayPoints[i] = pathHolder.GetChild(i).position;
                var newPosition = new Vector3(wayPoints[i].x, transform.position.y, wayPoints[i].z);
                wayPoints[i] = newPosition;
            }
        
            StartCoroutine(FollowPath(wayPoints));
        }

        private void Update()
        {
            var catchState = OnCatchThePlayer();
        
            if (catchState)
                playerVisibleTimer += Time.deltaTime;
            else
                playerVisibleTimer -= Time.deltaTime;

            playerVisibleTimer = Mathf.Clamp(playerVisibleTimer, 0, timeToSpotPlayer);
            spotLight.color = Color.Lerp(originalLightColor, Color.red, playerVisibleTimer / timeToSpotPlayer);

            if (playerVisibleTimer >= timeToSpotPlayer) { OnPlayerCaught?.Invoke(); }
        }

        private bool OnCatchThePlayer()
        {
            if (Vector3.Distance(transform.position, player.position) < viewDistance)
            {
                var directionToPlayer = (player.position - transform.position).normalized;
                var angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, directionToPlayer);

                if (angleBetweenGuardAndPlayer < viewAngle / 2f)
                {
                    if (!Physics.Linecast(transform.position, player.position, viewMask))
                        return true;
                }
            }
            return false;
        }
    
        private IEnumerator FollowPath(Vector3[] waypoints)
        {
            transform.position = waypoints[0];
            var targetWayPointIndex = 1;
            var targetWayPoint = waypoints[targetWayPointIndex];
            transform.LookAt(targetWayPoint);
        
            while (true)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetWayPoint, moveSpeed * Time.deltaTime);
                WalkState = true;
                if (transform.position == targetWayPoint)
                {
                    targetWayPointIndex = (targetWayPointIndex + 1) % waypoints.Length;
                    targetWayPoint = waypoints[targetWayPointIndex];
                    yield return new WaitForSeconds(waitTime);
                    yield return StartCoroutine(TurnToFace(targetWayPoint)); // Coroutine'ni cagirir ve tamamlanmasini bekler.
                }
                yield return null;
            }
        }

        private IEnumerator TurnToFace(Vector3 lookTarget) // Gardiyanin hareket ederken donmesini saglar.
        {
            WalkState = false;
            var directionToLookTarget = (lookTarget - transform.position).normalized;
            var targetAngle = 90 - Mathf.Atan2(directionToLookTarget.z, directionToLookTarget.x) * Mathf.Rad2Deg;

            while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f)
            {   
                var angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
                transform.eulerAngles = Vector3.up * angle;
                yield return null;
            }
        }

        private void OnDrawGizmos()
        {
            var startPosition = pathHolder.GetChild(0).position;
            var previousPosition = startPosition;
        
            foreach (Transform waypoint in pathHolder)
            {
                var wayPointPosition = waypoint.position;
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(wayPointPosition, 0.25f);
                Gizmos.DrawLine(previousPosition, wayPointPosition);
                previousPosition = wayPointPosition;
            }
        
            Gizmos.DrawLine(previousPosition, startPosition);
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
        }
    }
}
