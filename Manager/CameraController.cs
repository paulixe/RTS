using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private float offset=10;
    private float speed=10;
    private Vector2 minMaxXPosition;
    private Vector2 minMaxYPosition;
    private float zoomSpeed = 1;
    private float screenWidth;
    private float screenHeight;
    private Vector3 cameraMove;
    void Start()
    {
        Vector2Int dimensions = Map.GetDimension();
        minMaxXPosition = new Vector2Int(0, dimensions.x);
        minMaxYPosition = new Vector2Int(0,dimensions.y);
        screenWidth = Screen.width;
        screenHeight = Screen.height;

        cameraMove = transform.position;
    }
    // Update is called once per frame
    void Update()
    {
        //Move camera
        if (InputManager.IsGameOn)
        {
            if ((Input.mousePosition.x > screenWidth - offset) && transform.position.x < minMaxXPosition.y)
            {
                cameraMove.x += MoveSpeed();
            }
            if ((Input.mousePosition.x < offset) && transform.position.x > minMaxXPosition.x)
            {
                cameraMove.x -= MoveSpeed();
            }
            if ((Input.mousePosition.y > screenHeight - offset) && transform.position.y < minMaxYPosition.y)
            {
                cameraMove.y += MoveSpeed();
            }
            if ((Input.mousePosition.y < offset) && transform.position.y > minMaxYPosition.x)
            {
                cameraMove.y -= MoveSpeed();
            }
            transform.position = cameraMove;
            Camera camera = GetComponent<Camera>();
            camera.orthographicSize -= Input.mouseScrollDelta.y * zoomSpeed;
        }
    }
    public void ChangePosition(Vector2Int cellPos)
    {
        cameraMove = new Vector3(cellPos.x, cellPos.y,cameraMove.z);
    }
    float MoveSpeed()
    {
        return speed * Time.deltaTime;
    }
}