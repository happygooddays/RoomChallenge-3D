using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;

public class CameraSwitch : MonoBehaviour
{
    public int gridSizeX = 5; // X 방향 그리드 크기
    public int gridSizeY = 5; // Y 방향 그리드 크기

    private DatabaseReference databaseReference;

    // 가능한 방 크기 목록 (NxM 방식으로 전용면적을 정의)
    private Dictionary<Vector2Int, int> gridOptions = new Dictionary<Vector2Int, int>()
    {
        { new Vector2Int(3, 3), 9 },
        { new Vector2Int(3, 4), 12 },
        { new Vector2Int(3, 5), 15 },
        { new Vector2Int(4, 4), 16 },
        { new Vector2Int(3, 6), 18 },
        { new Vector2Int(4, 5), 20 },
        { new Vector2Int(4, 6), 24 },
        { new Vector2Int(5, 5), 25 },
        { new Vector2Int(5, 6), 30 },
        { new Vector2Int(6, 6), 36 }
    };

    // 카메라 위치와 회전 값 배열 (각 카메라 설정을 리스트로 저장)
    private Vector3[] cameraPositions = new Vector3[]
    {
        new Vector3(0, 1, -10), // 처음 위치
        new Vector3(-5, 0, -10), // 두번째 위치
        new Vector3(5, 0, -10),  // 세번째 위치
        new Vector3(0, -5, -10), // 네번째 위치
    };

    private Vector3[] cameraRotations = new Vector3[]
    {
        new Vector3(0, 0, 0),    // 처음 회전
        new Vector3(0, 30, 0),   // 두번째 회전
        new Vector3(0, -30, 0),  // 세번째 회전
        new Vector3(-45, 0, 0),  // 네번째 회전
    };

    private int currentCameraIndex = 0; // 현재 카메라 상태 (0부터 시작)

    public Camera mainCamera; // 메인 카메라

    // Start 메서드에서 처음 위치와 회전 설정
    void Start()
    {
        InitializeFirebase();
    }

    // Firebase 초기화
    void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
            GetApartmentData();
        });
    }

    // 아파트 데이터 가져오기
    void GetApartmentData()
    {
        rtSceneNum rtSceneNumComponent = GameObject.Find("GameObject").GetComponent<rtSceneNum>();
        string reference = rtSceneNumComponent.referencePath();

        FirebaseDatabase.DefaultInstance
            .GetReference(reference)
            .GetValueAsync().ContinueWithOnMainThread(task => {
                if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    if (snapshot.Exists)
                    {
                        float 전용면적 = float.Parse(snapshot.Child("전용면적").Value.ToString());
                        Debug.Log("전용면적: " + 전용면적);
                        SetGridSizeForArea(전용면적);
                    }
                    else
                    {
                        Debug.LogWarning("데이터가 존재하지 않습니다.");
                    }
                }
            });
    }

    // 전용면적에 맞는 가장 가까운 그리드 크기 설정
    void SetGridSizeForArea(float 전용면적)
    {
        float closestArea = float.MinValue;
        Vector2Int closestSize = Vector2Int.zero;

        foreach (var gridOption in gridOptions)
        {
            // 전용면적보다 작은 값 중에서 가장 큰 값 찾기
            if (gridOption.Value <= 전용면적 && gridOption.Value > closestArea)
            {
                closestArea = gridOption.Value;
                closestSize = gridOption.Key;
            }
        }

        if (closestSize != Vector2Int.zero)
        {
            gridSizeX = closestSize.x;
            gridSizeY = closestSize.y;
            Debug.Log($"전용면적에 맞는 가장 가까운 그리드 크기: {gridSizeX}x{gridSizeY} (전용면적: {closestArea})");
            // 처음 시작할 때 첫 번째 카메라 위치로 설정
            SetCameraPositionAndRotation();
            int cameraPositionX = gridSizeX + 1;
            int cameraPositionZ = (gridSizeX + 1) * 2;
            cameraPositions = new Vector3[]
            {
                new Vector3(0, 1, -cameraPositionZ), // 처음 위치
                new Vector3(-cameraPositionX, 0, -cameraPositionZ), // 두번째 위치
                new Vector3(cameraPositionX, 0, -cameraPositionZ),  // 세번째 위치
                new Vector3(0, -cameraPositionX, -cameraPositionZ), // 네번째 위치
            };
        }
        else
        {
            Debug.LogWarning("적합한 그리드 크기를 찾을 수 없습니다.");
        }
    }

    // 버튼 클릭 시 호출되는 메서드
    public void OnButtonClick()
    {
        // 현재 상태에서 다음 카메라 상태로 변경 (순차적으로 진행)
        currentCameraIndex++;

        // 배열의 크기를 넘어가지 않도록 순환 (4번 이후 다시 1번으로 돌아옴)
        if (currentCameraIndex >= cameraPositions.Length)
        {
            currentCameraIndex = 0;
        }

        // 카메라 위치와 회전 업데이트
        SetCameraPositionAndRotation();
    }

    // 카메라 위치와 회전을 설정하는 메서드
    private void SetCameraPositionAndRotation()
    {
        // 현재 인덱스에 해당하는 위치와 회전 값 설정
        mainCamera.transform.position = cameraPositions[currentCameraIndex];
        mainCamera.transform.rotation = Quaternion.Euler(cameraRotations[currentCameraIndex]);

        // 카메라 모드 설정: 첫 번째 카메라는 Orthographic, 나머지는 Perspective
        if (currentCameraIndex == 0)
        {
            mainCamera.orthographic = true;  // 첫 번째 카메라는 Orthographic
        }
        else
        {
            mainCamera.orthographic = false; // 나머지 카메라는 Perspective
        }

        // 위치와 회전 디버깅용 로그
        Debug.Log("Camera Position: " + cameraPositions[currentCameraIndex]);
        Debug.Log("Camera Rotation: " + cameraRotations[currentCameraIndex]);
    }
}
