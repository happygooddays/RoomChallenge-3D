//모바일 드래그
/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDragController : MonoBehaviour
{
    private Vector2 dragStartPos;  // 드래그 시작 위치
    private bool isDragging = false; // 드래그 중인지 여부
    private Camera mainCamera;  // 메인 카메라
    public float dragSpeed = 0.1f;   // 드래그 속도 조절

    // 카메라 이동할 z 값들
    private float[] stageZPositions = new float[] { -5f, 5f, 15f };
    private int currentStageIndex = 0; // 현재 스테이지의 인덱스 (0: -5, 1: 5, 2: 15)

    // 초기 카메라 위치
    private Vector3 initialCameraPos = new Vector3(0, 1, -5);

    private void Start()
    {
        mainCamera = Camera.main;  // 메인 카메라 가져오기
        mainCamera.transform.position = initialCameraPos; // 카메라 초기 위치 설정
    }

    private void Update()
    {
        // 터치 입력 (안드로이드 터치 입력 처리)
        if (Input.touchCount > 0) // 화면에 하나 이상의 터치가 있을 때
        {
            Touch touch = Input.GetTouch(0);  // 첫 번째 터치 입력 가져오기

            if (touch.phase == TouchPhase.Began)  // 터치 시작
            {
                isDragging = true;
                dragStartPos = touch.position;  // 드래그 시작 위치
            }

            // 드래그 끝날 때 (터치가 끝났을 때)
            else if (touch.phase == TouchPhase.Ended)  // 터치가 끝났을 때
            {
                if (isDragging)
                {
                    isDragging = false;

                    // 드래그한 거리만큼 카메라 이동
                    Vector2 dragDelta = touch.position - dragStartPos;
                    if (dragDelta.y < 0) // 아래에서 위로 드래그
                    {
                        // 이전 스테이지보다 큰 경우, 즉, 이전에 선택된 Stage보다 위로 드래그하면 다음 Stage로 이동
                        currentStageIndex = Mathf.Min(currentStageIndex + 1, stageZPositions.Length - 1);
                    }
                    else if (dragDelta.y > 0) // 위에서 아래로 드래그
                    {
                        // 이전 스테이지보다 작은 경우, 즉, 이전에 선택된 Stage보다 아래로 드래그하면 이전 Stage로 이동
                        currentStageIndex = Mathf.Max(currentStageIndex - 1, 0);
                    }

                    // 카메라를 이동
                    MoveCameraToStage(currentStageIndex);
                }
            }
        }
    }

    // 카메라를 해당 스테이지로 이동
    private void MoveCameraToStage(int stageIndex)
    {
        float targetZ = stageZPositions[stageIndex];
        mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, targetZ);
    }
}*/

//모바일 드래그2
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDragController : MonoBehaviour
{
    private Vector2 dragStartPos;  // 드래그 시작 위치
    private bool isDragging = false; // 드래그 중인지 여부
    private Camera mainCamera;  // 메인 카메라
    public float dragSpeed = 0.1f;   // 드래그 속도 조절
    public float moveSpeed = 5f;  // 카메라 이동 속도 (부드럽게 이동하기 위해)

    // 카메라 이동할 z 값들
    private float[] stageZPositions = new float[] { -5f, 5f, 15f };
    private int currentStageIndex = 0; // 현재 스테이지의 인덱스 (0: -5, 1: 5, 2: 15)

    // 초기 카메라 위치
    //private Vector3 initialCameraPos = new Vector3(0, 1, -5);
    private float targetZPosition;  // 카메라가 이동할 목표 z 좌표
    private bool isMoving = false;  // 카메라가 이동 중인지 여부

    private void Start()
    {
        mainCamera = Camera.main;  // 메인 카메라 가져오기
        //mainCamera.transform.position = initialCameraPos; // 카메라 초기 위치 설정
    }

    public void challengeStageIndex(int cSI)
    {
        currentStageIndex = cSI;
    }

    private void Update()
    {
        // 모바일 터치 입력 처리
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);  // 첫 번째 터치만 사용

            if (touch.phase == TouchPhase.Began)  // 터치 시작
            {
                isDragging = true;
                dragStartPos = touch.position; // 드래그 시작 위치
            }
            else if (touch.phase == TouchPhase.Ended)  // 터치 종료
            {
                if (isDragging)
                {
                    isDragging = false;

                    // 드래그한 거리만큼 카메라 이동
                    Vector2 dragDelta = touch.position - dragStartPos;
                    if (dragDelta.y < 0) // 아래에서 위로 드래그
                    {
                        // 이전 스테이지보다 큰 경우, 즉, 이전에 선택된 Stage보다 위로 드래그하면 다음 Stage로 이동
                        currentStageIndex = Mathf.Min(currentStageIndex + 1, stageZPositions.Length - 1);
                    }
                    else if (dragDelta.y > 0) // 위에서 아래로 드래그
                    {
                        // 이전 스테이지보다 작은 경우, 즉, 이전에 선택된 Stage보다 아래로 드래그하면 이전 Stage로 이동
                        currentStageIndex = Mathf.Max(currentStageIndex - 1, 0);
                    }

                    // 목표 z 값 설정
                    targetZPosition = stageZPositions[currentStageIndex];

                    // 카메라 이동 시작
                    isMoving = true;
                }
            }
        }

        // 카메라가 이동 중이면 부드럽게 이동
        if (isMoving)
        {
            // Lerp를 사용하여 카메라를 부드럽게 이동시킴
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, targetZPosition), moveSpeed * Time.deltaTime);

            // 목표 지점에 거의 도달하면 이동을 멈춤
            if (Mathf.Abs(mainCamera.transform.position.z - targetZPosition) < 0.1f)
            {
                mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, targetZPosition);
                isMoving = false;  // 이동이 완료되면 멈춤
            }
        }
    }
}


//PC 마우스 드래그
/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDragController : MonoBehaviour
{
    private Vector2 dragStartPos;  // 드래그 시작 위치
    private bool isDragging = false; // 드래그 중인지 여부
    private Camera mainCamera;  // 메인 카메라
    public float dragSpeed = 0.1f;   // 드래그 속도 조절
    public float moveSpeed = 5f;  // 카메라 이동 속도 (부드럽게 이동하기 위해)

    // 카메라 이동할 z 값들
    private float[] stageZPositions = new float[] { -5f, 5f, 15f };
    private int currentStageIndex = 0; // 현재 스테이지의 인덱스 (0: -5, 1: 5, 2: 15)

    // 초기 카메라 위치
    private Vector3 initialCameraPos = new Vector3(0, 1, -5);
    private float targetZPosition;  // 카메라가 이동할 목표 z 좌표
    private bool isMoving = false;  // 카메라가 이동 중인지 여부

    private void Start()
    {
        mainCamera = Camera.main;  // 메인 카메라 가져오기
        mainCamera.transform.position = initialCameraPos; // 카메라 초기 위치 설정
    }

    private void Update()
    {
        // 터치 입력 (PC에서 마우스 클릭 사용)
        if (Input.GetMouseButtonDown(0))  // 마우스 클릭 시작
        {
            isDragging = true;
            dragStartPos = Input.mousePosition; // 드래그 시작 위치
        }

        // 드래그 끝날 때 (터치가 끝났을 때)
        else if (Input.GetMouseButtonUp(0))  // 마우스 클릭이 끝났을 때
        {
            if (isDragging)
            {
                isDragging = false;

                // 드래그한 거리만큼 카메라 이동
                Vector2 dragDelta = (Vector2)Input.mousePosition - dragStartPos;
                if (dragDelta.y < 0) // 아래에서 위로 드래그
                {
                    // 이전 스테이지보다 큰 경우, 즉, 이전에 선택된 Stage보다 위로 드래그하면 다음 Stage로 이동
                    currentStageIndex = Mathf.Min(currentStageIndex + 1, stageZPositions.Length - 1);
                }
                else if (dragDelta.y > 0) // 위에서 아래로 드래그
                {
                    // 이전 스테이지보다 작은 경우, 즉, 이전에 선택된 Stage보다 아래로 드래그하면 이전 Stage로 이동
                    currentStageIndex = Mathf.Max(currentStageIndex - 1, 0);
                }

                // 목표 z 값 설정
                targetZPosition = stageZPositions[currentStageIndex];

                // 카메라 이동 시작
                isMoving = true;
            }
        }

        // 카메라가 이동 중이면 부드럽게 이동
        if (isMoving)
        {
            // Lerp를 사용하여 카메라를 부드럽게 이동시킴
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, targetZPosition), moveSpeed * Time.deltaTime);

            // 목표 지점에 거의 도달하면 이동을 멈춤
            if (Mathf.Abs(mainCamera.transform.position.z - targetZPosition) < 0.1f)
            {
                mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, targetZPosition);
                isMoving = false;  // 이동이 완료되면 멈춤
            }
        }
    }
}
*/