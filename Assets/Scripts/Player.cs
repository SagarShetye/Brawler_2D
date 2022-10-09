using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils{
    // Input from mouse axis may not be zero for no user input
    // Snap it to zero if its low enough to avoid passive camera movement
    public class MathUtils{
        public static float AdjustInputWithThreshold(float inputValue, float inputThreshold){
            if(Mathf.Abs(inputValue) < inputThreshold)
                return 0.0f;
            
            return inputValue;
        }
    }
}

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 0.5f;
    [SerializeField] private float jumpHeight = 3.0f;
    [SerializeField] private float inputThreshold = 1e-3f;

    private Rigidbody2D body;
    private float horizontalInput = 0.0f;
    private bool isJumping = false;
    private bool isWalking = false;
    private Vector3 moveDirection = new Vector3(1.0f, 0.0f, 0.0f);
    private Vector3 scale = new Vector3(1.0f, 1.0f, 1.0f);

    // DEBUG - For drawing the collider during gameplay
    [SerializeField] private GameObject linePrefab;
    private BoxCollider2D boxCollider;
    private EdgeCollider2D edgeCollider;
    private PolygonCollider2D polygonCollider;
    LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start(){
        body = GetComponent<Rigidbody2D>();

        lineRenderer = Instantiate(linePrefab).GetComponent<LineRenderer>();
        lineRenderer.transform.SetParent(transform);
        lineRenderer.transform.localPosition = body.transform.position;
        boxCollider = GetComponent<BoxCollider2D>();
        edgeCollider = GetComponent<EdgeCollider2D>();
        polygonCollider = GetComponent<PolygonCollider2D>();
    }

    // Update is called once per frame
    void Update(){
        if (Input.GetKeyDown(KeyCode.Joystick1Button1))  // TODO: Change to Square button PS4  
            isJumping = true;

        horizontalInput = Utils.MathUtils.AdjustInputWithThreshold(Input.GetAxis("Horizontal"), inputThreshold);
        moveDirection.x = (horizontalInput > 0.0f) ? 1.0f : -1.0f;

        isWalking = Mathf.Abs(horizontalInput) > 0.0f;
        if(isWalking){
            scale.x = moveDirection.x;
            transform.localScale = scale;
        }

        // // DEBUG - Render the collider
        // DrawBoxCollider();
        // // DrawEdgeCollider();
    }

    void FixedUpdate(){
        // groundCollisionCheck will always report collision with parent i.e. body
        // Hence, check for number of collisions = 1
        // Debug.Log("Box collider position: " + boxCollider.transform.position);
        // Debug.Log("Character position: " + transform.position);

        // TODO: Check if OverlapCircle is better than using OverlapCollider
        if (Physics.OverlapSphere(transform.position, 0.1f).Length == 1)
            return;

        if (isJumping)
        {
            // NOTE: Currently direction can be changed mid-Jump
            // to make the control more interesting
            Jump();
            isJumping = false;
        }

        if(isWalking)
            Walk();
    }

    void Walk(){
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
    }

    void Jump(){
        body.AddForce(jumpHeight * (Vector3.up), ForceMode2D.Impulse);      // Ideally, the direction vector should be normalized.
                                                                            // However, we go for the option that doesn't involve a sqrt
    }

    public Vector3 GetScale(){
        return scale;
    }

    public bool IsWalking(){
        return isWalking;
    }

    public bool IsJumping(){
        return isJumping;
    }

    // DEBUG - For rendering the collider
    void DrawBoxCollider()
    {
        Vector3[] positions = new Vector3[4];
        positions[0] = transform.TransformPoint(new Vector3(boxCollider.size.x / 2.0f, 0.0f, 0));                   // Bottom right
        positions[1] = transform.TransformPoint(new Vector3(boxCollider.size.x / 2.0f, boxCollider.size.y, 0));     // Top right
        positions[2] = transform.TransformPoint(new Vector3(-boxCollider.size.x / 2.0f, boxCollider.size.y, 0));    // Top left
        positions[3] = transform.TransformPoint(new Vector3(-boxCollider.size.x / 2.0f, 0.0f, 0));                  // Bottom left
        lineRenderer.SetPositions(positions);
    }

    void DrawEdgeCollider()
    {
        Vector3[] positions = new Vector3[polygonCollider.points.Length];
        for(int i = 0; i < polygonCollider.points.Length; i++)
            positions[i] = transform.TransformPoint(polygonCollider.points[i]);

        lineRenderer.positionCount = polygonCollider.points.Length;
        lineRenderer.SetPositions(positions);
    }
}
