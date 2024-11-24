using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMeshPro 관련 네임스페이스 추가
using Firebase;
using Firebase.Database;
using Firebase.Extensions;

public class FurnitureManager : MonoBehaviour
{
    public List<GameObject> furniturePrefabs = new List<GameObject>(); // 가구 프리팹 리스트
    private HashSet<Vector3> occupiedPositions = new HashSet<Vector3>(); // 배치된 가구의 위치를 추적하는 집합
    private Dictionary<GameObject, int> furnitureCount = new Dictionary<GameObject, int>(); // 가구별 개수 추적
    public float furnitureHeight = -1f; // 가구의 Z축 높이 설정 (음수)
    private int currentPrefabIndex = 0; // 현재 선택된 프리팹 인덱스
    private GameObject currentFurniture; // 드래그 중인 가구
    private Vector3 originalPosition; // 원래 위치를 저장하는 변수

    public int gridSizeX = 5; // 그리드 X 크기
    public int gridSizeY = 5; // 그리드 Y 크기
    public int aestheticScore = 0; // 미적 점수

    public TextMeshProUGUI ratingText; // Rating 텍스트 UI 컴포넌트

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
        UpdateRatingText(); // 초기 점수 표시
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
                        string 단지명 = snapshot.Child("단지명").Value.ToString();
                        Debug.Log("전용면적: " + 전용면적 + " 단지명: " + 단지명);
                        SetGridSizeForArea(전용면적, 단지명);
                    }
                    else
                    {
                        Debug.LogWarning("데이터가 존재하지 않습니다.");
                    }
                }
            });
    }

    // 전용면적에 맞는 가장 가까운 그리드 크기 설정
    void SetGridSizeForArea(float 전용면적, string 단지명)
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
            if (ratingText != null)
            {
                int 길이 = 단지명.Length; // 문자열의 길이를 구함

                // 폰트 사이즈 조건을 배열로 정의하고, 길이에 따라 해당 값을 설정
                ratingText.fontSize = 길이 <= 18 ? 0.3f :
                                      길이 <= 21 ? 0.25f :
                                      길이 <= 27 ? 0.2f : 0.15f;

                ratingText.text = 단지명; // 점수 표시
            }
        }
        else
        {
            Debug.LogWarning("적합한 그리드 크기를 찾을 수 없습니다.");
        }
    }

    void Update()
    {
        // 마우스 클릭 감지 및 가구 선택
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // 클릭한 객체가 가구인지 확인
                if (hit.transform.gameObject.CompareTag("Furniture"))
                {
                    currentFurniture = hit.transform.gameObject; // 선택된 가구
                    originalPosition = currentFurniture.transform.position; // 원래 위치 저장
                    return; // 가구를 클릭한 경우, 생성 로직을 실행하지 않음
                }

                // 클릭한 위치를 가장 가까운 그리드 셀의 위치로 변환
                Vector3 furniturePosition = GetClosestCellPosition(hit.point);

                // 같은 위치에 가구가 이미 배치되어 있는지 확인
                if (!occupiedPositions.Contains(furniturePosition) ||
                    (currentFurniture != null && furniturePosition == originalPosition))
                {
                    Debug.Log("생성");
                    // 현재 선택된 프리팹으로 가구 생성, X축 -90도 회전
                    Quaternion rotation = Quaternion.Euler(-90f, 0f, 0f);
                    GameObject newFurniture = Instantiate(furniturePrefabs[currentPrefabIndex], furniturePosition, rotation);

                    // 가구 이름과 z 값을 매핑한 Dictionary
                    Dictionary<string, float> positionMap = new Dictionary<string, float>
                    {
                        { "Fridge(Clone)", -1.25f },
                        { "Dryer(Clone)", -0.95f },
                        { "AirFryer(Clone)", -0.7f },
                        { "Coffee Maker(Clone)", -0.95f }
                    };

                    // newFurniture의 이름이 positionMap에 있는지 확인
                    if (positionMap.ContainsKey(newFurniture.name))
                    {
                        // 위치 설정
                        Vector3 newPosition = newFurniture.transform.position;
                        newPosition.z = positionMap[newFurniture.name]; // 해당 이름에 맞는 z 값 설정
                        newFurniture.transform.position = newPosition;

                        // 회전 설정 (x = -270, y = 0, z = 0)
                        Quaternion newRotation = Quaternion.Euler(-270f, 0f, 0f);
                        newFurniture.transform.rotation = newRotation;
                    }
                    occupiedPositions.Add(furniturePosition); // 가구 배치 후 위치 추가
                    UpdateFurnitureCount(newFurniture); // 가구 개수 업데이트
                    CalculateAestheticScore(); // 미적 평점 계산
                }
                else
                {
                    Debug.Log("이 위치에는 이미 가구가 배치되어 있습니다: " + furniturePosition);
                }
            }
        }

        // 드래그 중인 가구 위치 업데이트
        if (currentFurniture != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 dragPosition = GetClosestCellPosition(hit.point);

                // 드래그 위치에 이미 가구가 있는지 확인
                if (!occupiedPositions.Contains(dragPosition) || dragPosition == originalPosition)
                {
                    currentFurniture.transform.position = dragPosition; // 드래그 중인 가구 위치 업데이트
                }
            }
        }

        // 마우스 버튼을 떼면 가구를 원래 위치로 되돌리고 드롭
        if (Input.GetMouseButtonUp(0) && currentFurniture != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 dropPosition = GetClosestCellPosition(currentFurniture.transform.position);

                // 드롭 위치에 가구가 있는지 확인 및 Y가 3 이상인 경우 체크
                if ((!occupiedPositions.Contains(dropPosition) || dropPosition == originalPosition) && dropPosition.y < 3)
                {
                    // 새 가구를 드롭한 위치에 생성
                    Quaternion rotation = Quaternion.Euler(-90f, 0f, 0f);
                    GameObject newFurniture = Instantiate(furniturePrefabs[currentPrefabIndex], dropPosition, rotation);

                    // 가구 이름과 z 값을 매핑한 Dictionary
                    Dictionary<string, float> positionMap = new Dictionary<string, float>
                    {
                        { "Fridge(Clone)", -1.25f },
                        { "Dryer(Clone)", -0.95f },
                        { "AirFryer(Clone)", -0.7f },
                        { "Coffee Maker(Clone)", -0.95f }
                    };

                    // newFurniture의 이름이 positionMap에 있는지 확인
                    if (positionMap.ContainsKey(newFurniture.name))
                    {
                        // 위치 설정
                        Vector3 newPosition = newFurniture.transform.position;
                        newPosition.z = positionMap[newFurniture.name]; // 해당 이름에 맞는 z 값 설정
                        newFurniture.transform.position = newPosition;

                        // 회전 설정 (x = -270, y = 0, z = 0)
                        Quaternion newRotation = Quaternion.Euler(-270f, 0f, 0f);
                        newFurniture.transform.rotation = newRotation;
                    }

                    occupiedPositions.Add(dropPosition); // 배치 후 위치 추가
                    UpdateFurnitureCount(newFurniture); // 가구 개수 업데이트
                    CalculateAestheticScore(); // 미적 평점 계산
                }
                else
                {
                    Debug.Log("이 위치에는 이미 가구가 배치되어 있거나 Y 위치가 3 이상입니다: " + dropPosition);
                }
            }

            // 드래그 종료
            currentFurniture.transform.position = originalPosition; // 원래 위치로 되돌림
            currentFurniture = null; // 현재 가구를 null로 설정
        }
    }

    Vector3 GetClosestCellPosition(Vector3 hitPoint)
    {
        float cellSize = 1f; // 그리드 셀 크기 (필요에 따라 조정)
        float roundedX = gridSizeX % 2 == 0 ? Mathf.Floor(hitPoint.x + gridSizeX / 2f) - gridSizeX / 2f + 0.5f : Mathf.Round(hitPoint.x);
        float roundedY = gridSizeY % 2 == 0 ? Mathf.Floor(hitPoint.y + gridSizeY / 2f) - gridSizeY / 2f + 0.5f : Mathf.Round(hitPoint.y);

        Vector3 closestPosition = new Vector3(roundedX, roundedY, -0.5f);
        return closestPosition;
    }

    // 현재 프리팹 인덱스를 설정하는 메서드
    public void SetCurrentPrefabIndex(int index)
    {
        if (index >= 0 && index < furniturePrefabs.Count)
        {
            currentPrefabIndex = index;
            Debug.Log("현재 프리팹 인덱스 변경: " + currentPrefabIndex);
        }
        else
        {
            Debug.LogError("유효하지 않은 인덱스: " + index);
        }
    }

    // 프리팹을 추가하는 메서드
    public void AddPrefab(GameObject prefab)
    {
        if (furniturePrefabs.Count < 3)
        {
            if (!furniturePrefabs.Contains(prefab))
            {
                furniturePrefabs.Add(prefab);
                currentPrefabIndex = furniturePrefabs.Count - 1;
                Debug.Log("프리팹 추가됨: " + prefab.name);
            }
            else
            {
                Debug.Log("이미 존재하는 프리팹: " + prefab.name);
            }
        }
        else
        {
            Debug.Log("프리팹이 최대 개수(3개)에 도달했습니다.");
        }
    }

    // 가구 개수를 업데이트하고 점수 계산
    private void UpdateFurnitureCount(GameObject newFurniture)
    {
        if (furnitureCount.ContainsKey(newFurniture))
        {
            furnitureCount[newFurniture]++;
        }
        else
        {
            furnitureCount[newFurniture] = 1;
        }

        // 같은 가구가 3개 이상이면 점수 감소
        if (furnitureCount[newFurniture] > 3)
        {
            aestheticScore -= 10; // 중복 penalty
        }
    }

    // 미적 평점 계산 메서드
    private void CalculateAestheticScore()
    {
        aestheticScore = 0; // 점수 초기화

        // 빈 공간에 대해 점수 부여
        int emptySpaces = (gridSizeX * gridSizeY) - occupiedPositions.Count;
        aestheticScore += emptySpaces * 5; // 빈 공간당 점수

        // 이동 가능 공간 평가
        foreach (var position in occupiedPositions)
        {
            aestheticScore += CountMovableSpaces(position) * 3; // 이동 가능한 공간당 점수
        }

        // 중앙에 가까울수록 점수 증가
        Vector3 gridCenter = new Vector3(gridSizeX / 2f, gridSizeY / 2f, 0);
        foreach (var position in occupiedPositions)
        {
            float distanceToCenter = Vector3.Distance(position, gridCenter);
            aestheticScore += Mathf.Max(0, (int)(10 - distanceToCenter)); // 가까울수록 점수 증가
        }

        // 가구 간의 거리 계산
        List<Vector3> positionsList = new List<Vector3>(occupiedPositions);
        for (int i = 0; i < positionsList.Count; i++)
        {
            for (int j = i + 1; j < positionsList.Count; j++)
            {
                float distanceBetweenFurniture = Vector3.Distance(positionsList[i], positionsList[j]);
                if (distanceBetweenFurniture < 1.5f) // 너무 가까우면 점수 감소
                {
                    aestheticScore -= 5; // 가까우면 점수 감소
                }
            }
        }

        // 전반적인 조화 평가 (예시: 특정 가구 조합)
        if (occupiedPositions.Contains(new Vector3(1, 1, 0)) && occupiedPositions.Contains(new Vector3(2, 1, 0))) // (1,1)과 (2,1) 위치
        {
            aestheticScore += 10; // 추가 점수
        }

        Debug.Log("현재 미적 점수: " + aestheticScore);
    }

    // 주어진 위치의 이동 가능한 공간 수를 계산하는 메서드
    private int CountMovableSpaces(Vector3 position)
    {
        int movableSpaceCount = 0;
        Vector3[] directions = {
            Vector3.up,
            Vector3.down,
            Vector3.left,
            Vector3.right
        };

        foreach (var direction in directions)
        {
            Vector3 adjacentPosition = position + direction;

            // 그리드 범위 내에 있는지 확인하고, 해당 위치에 가구가 없으면 이동 가능
            if (occupiedPositions.Contains(adjacentPosition) ||
                adjacentPosition.x < 0 || adjacentPosition.x >= gridSizeX ||
                adjacentPosition.y < 0 || adjacentPosition.y >= gridSizeY)
            {
                continue;
            }

            movableSpaceCount++;
        }

        return movableSpaceCount;
    }

    // 점수 텍스트 업데이트 메서드
    private void UpdateRatingText()
    {
        if (ratingText != null)
        {
            ratingText.text = "미적 점수: " + aestheticScore; // 점수 표시
        }
    }
}
