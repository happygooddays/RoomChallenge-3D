using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // 추가: 씬 전환을 위해
using Firebase;
using Firebase.Database;
using Firebase.Extensions;

public class CameraSwitcher : MonoBehaviour
{
    public Camera mainCamera;  // 메인 카메라
    public Camera subCamera;   // 서브 카메라
    public Canvas canvas;      // 캔버스

    public GameObject closeGO;
    public GameObject completeGO;
    public GameObject switchCamera;

    private bool isRotating = false;  // 서브 카메라 회전 상태 추적
    private float rotationSpeed = 30f; // 회전 속도
    private Vector3 a = Vector3.zero;  // 초기값을 Vector3.zero로 설정

    private float sideLength = 5f;  // 한 변의 길이
    private float diagonalLength;  // 대각선 길이

    public int gridSizeX = 5; // X 방향 그리드 크기
    public int gridSizeY = 5; // Y 방향 그리드 크기

    private int passingScore = 0;

    private DatabaseReference databaseReference;

    [SerializeField]
    public FurnitureManager furnitureManager;  // FurnitureManager 인스턴스 참조

    [SerializeField]
    public scorePopup sP; // 점수 팝업

    private float currentAngle = 0f;  // 카메라의 현재 각도

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

    void Start()
    {
        closeGO.SetActive(true);
        completeGO.SetActive(true);
        switchCamera.SetActive(true);
        InitializeFirebase();
        diagonalLength = Mathf.Sqrt(2 * Mathf.Pow(sideLength, 2));
        
        // 게임 시작 시 main 카메라만 활성화
        SwitchToMainCamera();
        
        subCamera.nearClipPlane = 0.1f;  // 가까운 클리핑 평면
        subCamera.farClipPlane = diagonalLength / 2;
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
            if (closestSize.x >= closestSize.y)
                sideLength = closestSize.x;
            else
                sideLength = closestSize.y;
        }
        else
        {
            Debug.LogWarning("적합한 그리드 크기를 찾을 수 없습니다.");
        }
    }

    // 카메라 전환 메서드
    public void SwitchCamera()
    {
        if (!isRotating)  // 회전 중이지 않으면
        {
            SwitchToSubCamera();  // 서브 카메라로 전환
        }
    }

    // main 카메라로 전환
    private void SwitchToMainCamera()
    {
        mainCamera.enabled = true;
        subCamera.enabled = false;

        isRotating = false;  // 회전 상태 해제
    }

    // sub 카메라로 전환
    private void SwitchToSubCamera()
    {
        mainCamera.enabled = false;
        subCamera.enabled = true;

        isRotating = true;  // 회전 시작
    }

    void Update()
    {
        // 서브 카메라가 활성화되어 있고 회전 중일 때
        if (isRotating && subCamera.enabled)
        {
            // subCamera의 far clipping plane을 3으로 설정
            subCamera.GetComponent<Camera>().farClipPlane = Mathf.Sqrt(gridSizeX * gridSizeX + gridSizeY * gridSizeY)*2;

            // 회전할 각도 계산 (회전 속도에 따라)
            currentAngle += rotationSpeed * Time.deltaTime;

            // 360도를 한 바퀴 돌면 회전을 멈추도록 설정
            if (currentAngle >= 340f)
            {
                subCamera.transform.position = new Vector3(0f, 0f, -1f);
                subCamera.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
                currentAngle = 340f; // 360도를 넘지 않도록

                subCamera.transform.eulerAngles = new Vector3(0f, 90f, 0f);
                // 메인 카메라로 전환
                SwitchToMainCamera();

                // 회전 상태 해제
                isRotating = false;

                // X, Y 순서를 자동으로 처리하도록 개선
                int x = Mathf.Min(gridSizeX, gridSizeY);
                int y = Mathf.Max(gridSizeX, gridSizeY);

                if (IsValidScore(x, y, furnitureManager.aestheticScore))
                {
                    Debug.Log($"통과: {x}x{y} 크기에서 점수 {furnitureManager.aestheticScore}로 조건을 만족했습니다.");

                    rtSceneNum rtSceneNumComponent = GameObject.Find("GameObject").GetComponent<rtSceneNum>();
                    int reference = rtSceneNumComponent.apartNum();
                        
                    OnStageComplete(reference);

                    sP.scoreText.text = "미적점수: " + furnitureManager.aestheticScore + "점\n통과점수: " + passingScore + "점\n클리어";
                }
                else
                {
                    Debug.Log($"불통과: {x}x{y} 크기에서 점수 {furnitureManager.aestheticScore}로 조건을 만족하지 못했습니다.");
                    sP.scoreText.text = "미적점수: " + furnitureManager.aestheticScore + "점\n통과점수: " + passingScore + "점\n실패";
                }
                closeGO.SetActive(false);
                completeGO.SetActive(false);
                switchCamera.SetActive(false);
                sP.OpenScorePopup();

                return;
            }

            currentAngle = (currentAngle < 200f) ? 200f : currentAngle;

            // 카메라의 새로운 위치 계산 (원형 경로)
            float xx = Mathf.Sqrt(gridSizeX * gridSizeX + gridSizeY * gridSizeY) * Mathf.Cos(Mathf.Deg2Rad * currentAngle);
            float zz = Mathf.Sqrt(gridSizeX * gridSizeX + gridSizeY * gridSizeY) * Mathf.Sin(Mathf.Deg2Rad * currentAngle);

            // 카메라 위치 업데이트
            subCamera.transform.position = new Vector3(xx, 0, zz);
            Debug.Log(currentAngle);
            // 카메라가 중심을 바라보도록 설정
            subCamera.transform.LookAt(Vector3.zero);
        }
    }

    // 완료 버튼 클릭 시 호출될 함수
    public void OnStageComplete(int stageNumber)
    {
        // 스테이지 클리어 정보 가져오기
        int stageClearStatus = PlayerPrefs.GetInt("Stage" + stageNumber + "Clear", 0); // 0은 기본값, 즉 클리어되지 않은 상태

        // 이미 클리어 상태라면 저장하지 않음
        if (stageClearStatus == 0)
        {
            // 스테이지 클리어 정보 저장
            PlayerPrefs.SetInt("Stage" + stageNumber + "Clear", 1); // 1은 클리어 상태
            PlayerPrefs.Save();
        }
    }

    bool IsValidScore(int gridX, int gridY, float score)
    {
        // 각 gridX, gridY에 대응하는 최소 점수를 딕셔너리로 정의
        var scoreRequirements = new Dictionary<(int, int), float>
        {
            {(3, 3), 45},
            {(3, 4), 60},
            {(3, 5), 75},
            {(3, 6), 85},
            {(4, 4), 75},
            {(4, 5), 95},
            {(4, 6), 115},
            {(5, 5), 120},
            {(5, 6), 145},
            {(6, 6), 175}
        };

        // (gridX, gridY)에 대한 최소 점수를 가져옴
        if (scoreRequirements.TryGetValue((gridX, gridY), out float requiredScore))
        {
            // passingScore에 해당하는 최소 점수 값을 설정
            passingScore = (int)requiredScore;

            // passingScore와 비교하여 통과 여부 반환
            return score >= passingScore;
        }

        // 해당 조건이 없으면 false 반환
        return false;
    }


    public void goStage(string sceneName)
    {
        SceneManager.LoadScene(sceneName); // 씬 변경
    }
}
