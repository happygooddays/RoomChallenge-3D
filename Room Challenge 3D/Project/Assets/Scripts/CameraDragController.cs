//����� �巡��
/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDragController : MonoBehaviour
{
    private Vector2 dragStartPos;  // �巡�� ���� ��ġ
    private bool isDragging = false; // �巡�� ������ ����
    private Camera mainCamera;  // ���� ī�޶�
    public float dragSpeed = 0.1f;   // �巡�� �ӵ� ����

    // ī�޶� �̵��� z ����
    private float[] stageZPositions = new float[] { -5f, 5f, 15f };
    private int currentStageIndex = 0; // ���� ���������� �ε��� (0: -5, 1: 5, 2: 15)

    // �ʱ� ī�޶� ��ġ
    private Vector3 initialCameraPos = new Vector3(0, 1, -5);

    private void Start()
    {
        mainCamera = Camera.main;  // ���� ī�޶� ��������
        mainCamera.transform.position = initialCameraPos; // ī�޶� �ʱ� ��ġ ����
    }

    private void Update()
    {
        // ��ġ �Է� (�ȵ���̵� ��ġ �Է� ó��)
        if (Input.touchCount > 0) // ȭ�鿡 �ϳ� �̻��� ��ġ�� ���� ��
        {
            Touch touch = Input.GetTouch(0);  // ù ��° ��ġ �Է� ��������

            if (touch.phase == TouchPhase.Began)  // ��ġ ����
            {
                isDragging = true;
                dragStartPos = touch.position;  // �巡�� ���� ��ġ
            }

            // �巡�� ���� �� (��ġ�� ������ ��)
            else if (touch.phase == TouchPhase.Ended)  // ��ġ�� ������ ��
            {
                if (isDragging)
                {
                    isDragging = false;

                    // �巡���� �Ÿ���ŭ ī�޶� �̵�
                    Vector2 dragDelta = touch.position - dragStartPos;
                    if (dragDelta.y < 0) // �Ʒ����� ���� �巡��
                    {
                        // ���� ������������ ū ���, ��, ������ ���õ� Stage���� ���� �巡���ϸ� ���� Stage�� �̵�
                        currentStageIndex = Mathf.Min(currentStageIndex + 1, stageZPositions.Length - 1);
                    }
                    else if (dragDelta.y > 0) // ������ �Ʒ��� �巡��
                    {
                        // ���� ������������ ���� ���, ��, ������ ���õ� Stage���� �Ʒ��� �巡���ϸ� ���� Stage�� �̵�
                        currentStageIndex = Mathf.Max(currentStageIndex - 1, 0);
                    }

                    // ī�޶� �̵�
                    MoveCameraToStage(currentStageIndex);
                }
            }
        }
    }

    // ī�޶� �ش� ���������� �̵�
    private void MoveCameraToStage(int stageIndex)
    {
        float targetZ = stageZPositions[stageIndex];
        mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, targetZ);
    }
}*/

//����� �巡��2
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDragController : MonoBehaviour
{
    private Vector2 dragStartPos;  // �巡�� ���� ��ġ
    private bool isDragging = false; // �巡�� ������ ����
    private Camera mainCamera;  // ���� ī�޶�
    public float dragSpeed = 0.1f;   // �巡�� �ӵ� ����
    public float moveSpeed = 5f;  // ī�޶� �̵� �ӵ� (�ε巴�� �̵��ϱ� ����)

    // ī�޶� �̵��� z ����
    private float[] stageZPositions = new float[] { -5f, 5f, 15f };
    private int currentStageIndex = 0; // ���� ���������� �ε��� (0: -5, 1: 5, 2: 15)

    // �ʱ� ī�޶� ��ġ
    //private Vector3 initialCameraPos = new Vector3(0, 1, -5);
    private float targetZPosition;  // ī�޶� �̵��� ��ǥ z ��ǥ
    private bool isMoving = false;  // ī�޶� �̵� ������ ����

    private void Start()
    {
        mainCamera = Camera.main;  // ���� ī�޶� ��������
        //mainCamera.transform.position = initialCameraPos; // ī�޶� �ʱ� ��ġ ����
    }

    public void challengeStageIndex(int cSI)
    {
        currentStageIndex = cSI;
    }

    private void Update()
    {
        // ����� ��ġ �Է� ó��
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);  // ù ��° ��ġ�� ���

            if (touch.phase == TouchPhase.Began)  // ��ġ ����
            {
                isDragging = true;
                dragStartPos = touch.position; // �巡�� ���� ��ġ
            }
            else if (touch.phase == TouchPhase.Ended)  // ��ġ ����
            {
                if (isDragging)
                {
                    isDragging = false;

                    // �巡���� �Ÿ���ŭ ī�޶� �̵�
                    Vector2 dragDelta = touch.position - dragStartPos;
                    if (dragDelta.y < 0) // �Ʒ����� ���� �巡��
                    {
                        // ���� ������������ ū ���, ��, ������ ���õ� Stage���� ���� �巡���ϸ� ���� Stage�� �̵�
                        currentStageIndex = Mathf.Min(currentStageIndex + 1, stageZPositions.Length - 1);
                    }
                    else if (dragDelta.y > 0) // ������ �Ʒ��� �巡��
                    {
                        // ���� ������������ ���� ���, ��, ������ ���õ� Stage���� �Ʒ��� �巡���ϸ� ���� Stage�� �̵�
                        currentStageIndex = Mathf.Max(currentStageIndex - 1, 0);
                    }

                    // ��ǥ z �� ����
                    targetZPosition = stageZPositions[currentStageIndex];

                    // ī�޶� �̵� ����
                    isMoving = true;
                }
            }
        }

        // ī�޶� �̵� ���̸� �ε巴�� �̵�
        if (isMoving)
        {
            // Lerp�� ����Ͽ� ī�޶� �ε巴�� �̵���Ŵ
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, targetZPosition), moveSpeed * Time.deltaTime);

            // ��ǥ ������ ���� �����ϸ� �̵��� ����
            if (Mathf.Abs(mainCamera.transform.position.z - targetZPosition) < 0.1f)
            {
                mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, targetZPosition);
                isMoving = false;  // �̵��� �Ϸ�Ǹ� ����
            }
        }
    }
}


//PC ���콺 �巡��
/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDragController : MonoBehaviour
{
    private Vector2 dragStartPos;  // �巡�� ���� ��ġ
    private bool isDragging = false; // �巡�� ������ ����
    private Camera mainCamera;  // ���� ī�޶�
    public float dragSpeed = 0.1f;   // �巡�� �ӵ� ����
    public float moveSpeed = 5f;  // ī�޶� �̵� �ӵ� (�ε巴�� �̵��ϱ� ����)

    // ī�޶� �̵��� z ����
    private float[] stageZPositions = new float[] { -5f, 5f, 15f };
    private int currentStageIndex = 0; // ���� ���������� �ε��� (0: -5, 1: 5, 2: 15)

    // �ʱ� ī�޶� ��ġ
    private Vector3 initialCameraPos = new Vector3(0, 1, -5);
    private float targetZPosition;  // ī�޶� �̵��� ��ǥ z ��ǥ
    private bool isMoving = false;  // ī�޶� �̵� ������ ����

    private void Start()
    {
        mainCamera = Camera.main;  // ���� ī�޶� ��������
        mainCamera.transform.position = initialCameraPos; // ī�޶� �ʱ� ��ġ ����
    }

    private void Update()
    {
        // ��ġ �Է� (PC���� ���콺 Ŭ�� ���)
        if (Input.GetMouseButtonDown(0))  // ���콺 Ŭ�� ����
        {
            isDragging = true;
            dragStartPos = Input.mousePosition; // �巡�� ���� ��ġ
        }

        // �巡�� ���� �� (��ġ�� ������ ��)
        else if (Input.GetMouseButtonUp(0))  // ���콺 Ŭ���� ������ ��
        {
            if (isDragging)
            {
                isDragging = false;

                // �巡���� �Ÿ���ŭ ī�޶� �̵�
                Vector2 dragDelta = (Vector2)Input.mousePosition - dragStartPos;
                if (dragDelta.y < 0) // �Ʒ����� ���� �巡��
                {
                    // ���� ������������ ū ���, ��, ������ ���õ� Stage���� ���� �巡���ϸ� ���� Stage�� �̵�
                    currentStageIndex = Mathf.Min(currentStageIndex + 1, stageZPositions.Length - 1);
                }
                else if (dragDelta.y > 0) // ������ �Ʒ��� �巡��
                {
                    // ���� ������������ ���� ���, ��, ������ ���õ� Stage���� �Ʒ��� �巡���ϸ� ���� Stage�� �̵�
                    currentStageIndex = Mathf.Max(currentStageIndex - 1, 0);
                }

                // ��ǥ z �� ����
                targetZPosition = stageZPositions[currentStageIndex];

                // ī�޶� �̵� ����
                isMoving = true;
            }
        }

        // ī�޶� �̵� ���̸� �ε巴�� �̵�
        if (isMoving)
        {
            // Lerp�� ����Ͽ� ī�޶� �ε巴�� �̵���Ŵ
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, targetZPosition), moveSpeed * Time.deltaTime);

            // ��ǥ ������ ���� �����ϸ� �̵��� ����
            if (Mathf.Abs(mainCamera.transform.position.z - targetZPosition) < 0.1f)
            {
                mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, targetZPosition);
                isMoving = false;  // �̵��� �Ϸ�Ǹ� ����
            }
        }
    }
}
*/