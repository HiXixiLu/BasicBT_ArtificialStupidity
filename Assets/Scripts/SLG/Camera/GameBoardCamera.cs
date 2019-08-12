using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoardCamera : MonoBehaviour
{
    Transform swival, stick;    // swival 控制其旋转角度，stick控制其运动远近
    float zoom = 1f;
    [SerializeField]
    public float stickMinZoom = -250, stickMaxZoom = -45;    // 用于限制相机缩放的范围
    [SerializeField]
    public float moveSpeed = 100f;
    [SerializeField]
    public float rotationSpeed = 180f;
    float rotationAngle;

    private void Awake()
    {
        swival = transform.GetChild(0);
        stick = swival.GetChild(0);
    }

    private void Update()
    {
        float zoomDelta = Input.GetAxis("Mouse ScrollWheel");   // 各轴名字可在 Edit/Project Settings/Input 里查看
        if (zoomDelta != 0f)
        {
            AdjustZoom(zoomDelta);
        }

        float rotationDelta = Input.GetAxis("Rotation");    // There's no default Rotation axis. Create one and control it with QE keys
        if (rotationDelta != 0f)
        {
            AdjustRotation(rotationDelta);
        }

        float xDelta = Input.GetAxis("Horizontal");
        float zDelta = Input.GetAxis("Vertical");
        if (xDelta != 0f || zDelta != 0f)
        {
            AdjustPosition(xDelta, zDelta);
        }
    }

    void AdjustZoom(float delta)
    {
        zoom = Mathf.Clamp01(zoom + delta);

        float distance = Mathf.Lerp(stickMinZoom, stickMaxZoom, zoom);
        stick.localPosition = new Vector3(0f, 0f, distance);
    }
    void AdjustPosition(float xDelta, float zDelta)
    {
        float distanceFactor = moveSpeed * Time.deltaTime;
        Vector3 direction = transform.localRotation * new Vector3(xDelta, 0f, zDelta).normalized;

        Vector3 position = transform.localPosition;
        position += direction * distanceFactor;   // 比较友好的方式应该是相对摄像机坐标系移动
        transform.localPosition = ClampPosition(position);
    }
    Vector3 ClampPosition(Vector3 position)
    {
        // TODO: 未来应该控制 x，z轴的移动范围，不能超出地图边缘
        return position;
    }
    void AdjustRotation(float delta)
    {
        rotationAngle += delta * rotationSpeed * Time.deltaTime;

        if (rotationAngle < 0f)
            rotationAngle += 360f;
        if (rotationAngle > 360f)
            rotationAngle -= 360f;

        transform.localRotation = Quaternion.Euler(0f, rotationAngle, 0f);
    }
}
