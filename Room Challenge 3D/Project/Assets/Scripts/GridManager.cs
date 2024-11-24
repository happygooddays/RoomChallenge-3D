using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;

public class GridManager : MonoBehaviour
{
    public GameObject gridCellPrefab; // 그리드 셀로 사용할 프리팹
    public int gridSizeX = 5; // X 방향 그리드 크기
    public int gridSizeY = 5; // Y 방향 그리드 크기
    public float cellSize = 1f; // 셀 크기
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
            CreateGrid();
        }
        else
        {
            Debug.LogWarning("적합한 그리드 크기를 찾을 수 없습니다.");
        }
    }

    // 그리드 생성
    void CreateGrid()
    {
        // 설정된 gridSizeX와 gridSizeY로 그리드 생성
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 position = new Vector3(x - (gridSizeX - 1) / 2f, y - (gridSizeY - 1) / 2f, 0);
                Quaternion rotation = Quaternion.Euler(-90, 0, 0);
                Instantiate(gridCellPrefab, position, rotation);
            }
        }
    }
}
