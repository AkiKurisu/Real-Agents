using UnityEngine;
namespace Kurisu.RealAgents.Example.Camera
{
    public class TopDownCameraController : MonoBehaviour
    {
        public float moveSpeed = 10f;
        public float zoomSpeed = 5f;
        private bool isDragging = false;
        private Vector3 lastMousePosition;
        private Vector3 startPosition;
        public Vector2 ZoomInput { get; set; }
        private float initialDistance;
        private void Awake()
        {
            startPosition = transform.position;
#if UNITY_ANDROID
            moveSpeed /= 10;
            zoomSpeed /= 10;
#endif
        }
        private void Update()
        {

            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            float zoomInput = Input.GetAxis("Mouse ScrollWheel");

            if (Input.GetKeyDown(KeyCode.R))
            {
                transform.position = startPosition;
                return;
            }

            if (Input.touchCount == 2)
            {
                Touch touch1 = Input.GetTouch(0);
                Touch touch2 = Input.GetTouch(1);
                if (touch2.phase == TouchPhase.Began)
                {
                    initialDistance = Vector2.Distance(touch1.position, touch2.position);
                }
                else if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
                {
                    float currentDistance = Vector2.Distance(touch1.position, touch2.position);
                    zoomInput = (currentDistance - initialDistance) / 100;
                }
                isDragging = false;
            }
            else if (Input.GetMouseButtonDown(0))
            {
                isDragging = true;
                lastMousePosition = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
            }

            if (isDragging)
            {
                Vector3 dragOffset = moveSpeed * Time.deltaTime * (Input.mousePosition - lastMousePosition);
                Vector3 transformedOffset = transform.TransformDirection(new Vector3(-dragOffset.x, 0, -dragOffset.y));
                transform.Translate(transformedOffset, Space.World);
            }
            else
            {
                Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;
                Vector3 transformedMoveDirection = transform.TransformDirection(moveDirection);
                transform.Translate(transformedMoveDirection * moveSpeed * Time.deltaTime, Space.World);
            }

            Vector3 zoomDirection = transform.up * -zoomInput;
            transform.Translate(zoomDirection * zoomSpeed, Space.World);

            lastMousePosition = Input.mousePosition;
        }
    }
}
