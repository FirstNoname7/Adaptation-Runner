using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Vector3 startGamePosition;
    Quaternion startGameRotation;
    [SerializeField] private RoadSpawner roadSpawner;
    float laneOffset;
    float laneChangeSpeed = 15;
    Rigidbody rigidbody;
    float pointStart;
    float pointFinish;
    bool isMoving = false;
    Coroutine movingCoroutine;
    bool isJumping = false;
    float jumpPower = 15;
    float jumpGravity = -40;
    float realGravity = -9.8f;

    private void Start()
    {
        laneOffset = MapSpawn.instance.laneOffset;
        startGamePosition = transform.position;
        startGameRotation = transform.rotation;
        rigidbody = GetComponent<Rigidbody>();
        SwipeManager.instance.MoveEvent += MovePlayer;
    }

    void MovePlayer(bool[] swipes)
    {
        if (swipes[(int)SwipeManager.Direction.Left] && pointFinish > -laneOffset)
        {
            MoveHorizontal(-laneChangeSpeed);
        }
        if (swipes[(int)SwipeManager.Direction.Right] && pointFinish < laneOffset)
        {
            MoveHorizontal(laneChangeSpeed);
        }
        if (swipes[(int)SwipeManager.Direction.Up] && isJumping == false)
        {
            Jump();
        }
    }

    void Jump()
    {
        isJumping = true;
        rigidbody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        Physics.gravity = new Vector3(0, jumpGravity, 0);
        StartCoroutine(StopJumpCoroutine());
    }

    IEnumerator StopJumpCoroutine()
    {
        do
        {
            yield return new WaitForSeconds(0.02f);
        } while (rigidbody.velocity.y != 0);
        isJumping = false;
        Physics.gravity = new Vector3(0, realGravity, 0);
    }


    void MoveHorizontal(float speed)
    {
        pointStart = pointFinish;
        pointFinish += Mathf.Sign(speed)*laneOffset;

        if (isMoving)
        {
            StopCoroutine(movingCoroutine);
            isMoving = false;
        }
        movingCoroutine = StartCoroutine(MoveCoroutine(speed));      
    }

    IEnumerator MoveCoroutine(float vectorX)
    {
        isMoving = true;
        while (Mathf.Abs(pointStart - transform.position.x) < laneOffset)
        {
            yield return new WaitForFixedUpdate();
            rigidbody.velocity = new Vector3(vectorX, rigidbody.velocity.y, 0);

            float x = Mathf.Clamp(transform.position.x, Mathf.Min(pointStart, pointFinish), Mathf.Max(pointStart, pointFinish));
            transform.position = new Vector3(x, transform.position.y, transform.position.z);
        }
        rigidbody.velocity = Vector3.zero;
        transform.position = new Vector3(pointFinish, transform.position.y, transform.position.z);
        if (transform.position.y > 1)
        {
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, -10, rigidbody.velocity.z);
        }
        isMoving = false;
    }

    public void StartGame()
    {
        roadSpawner.StartLevel();
    }

    public void ResetGame()
    {
        rigidbody.velocity = Vector3.zero;
        pointStart = 0;
        pointFinish = 0;
        transform.position = startGamePosition;
        transform.rotation = startGameRotation;
        roadSpawner.ResetLevel();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Ramp")
        {
            rigidbody.constraints |= RigidbodyConstraints.FreezePositionZ;
        }
        if(other.gameObject.tag == "Lose")
        {
            ResetGame();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Ramp")
        {
            rigidbody.constraints &= ~RigidbodyConstraints.FreezePositionZ;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.tag == "RampPlane")
        {
            if(rigidbody.velocity.x == 0 && isJumping == false)
            {
                rigidbody.velocity = new Vector3(rigidbody.velocity.x, -10, rigidbody.velocity.z);
            }
        }
    }
}
