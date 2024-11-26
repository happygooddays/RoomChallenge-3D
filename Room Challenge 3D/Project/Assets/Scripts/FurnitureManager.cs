using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMeshPro ���� ���ӽ����̽� �߰�
using Firebase;
using Firebase.Database;
using Firebase.Extensions;

public class FurnitureManager : MonoBehaviour
{
    public List<GameObject> furniturePrefabs = new List<GameObject>(); // ���� ������ ����Ʈ
    private HashSet<Vector3> occupiedPositions = new HashSet<Vector3>(); // ��ġ�� ������ ��ġ�� �����ϴ� ����
    private Dictionary<GameObject, int> furnitureCount = new Dictionary<GameObject, int>(); // ������ ���� ����
    public float furnitureHeight = -1f; // ������ Z�� ���� ���� (����)
    private int currentPrefabIndex = 0; // ���� ���õ� ������ �ε���
    private GameObject currentFurniture; // �巡�� ���� ����
    private Vector3 originalPosition; // ���� ��ġ�� �����ϴ� ����

    public int gridSizeX = 5; // �׸��� X ũ��
    public int gridSizeY = 5; // �׸��� Y ũ��
    public int aestheticScore = 0; // ���� ����

    public TextMeshProUGUI ratingText; // Rating �ؽ�Ʈ UI ������Ʈ

    private DatabaseReference databaseReference;

    // ������ �� ũ�� ��� (NxM ������� ��������� ����)
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
        UpdateRatingText(); // �ʱ� ���� ǥ��
    }

    // Firebase �ʱ�ȭ
    void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
            GetApartmentData();
        });
    }

    // ����Ʈ ������ ��������
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
                        float ������� = float.Parse(snapshot.Child("�������").Value.ToString());
                        string ������ = snapshot.Child("������").Value.ToString();
                        Debug.Log("�������: " + ������� + " ������: " + ������);
                        SetGridSizeForArea(�������, ������);
                    }
                    else
                    {
                        Debug.LogWarning("�����Ͱ� �������� �ʽ��ϴ�.");
                    }
                }
            });
    }

    // ��������� �´� ���� ����� �׸��� ũ�� ����
    void SetGridSizeForArea(float �������, string ������)
    {
        float closestArea = float.MinValue;
        Vector2Int closestSize = Vector2Int.zero;

        foreach (var gridOption in gridOptions)
        {
            // ����������� ���� �� �߿��� ���� ū �� ã��
            if (gridOption.Value <= ������� && gridOption.Value > closestArea)
            {
                closestArea = gridOption.Value;
                closestSize = gridOption.Key;
            }
        }

        if (closestSize != Vector2Int.zero)
        {
            gridSizeX = closestSize.x;
            gridSizeY = closestSize.y;
            Debug.Log($"��������� �´� ���� ����� �׸��� ũ��: {gridSizeX}x{gridSizeY} (�������: {closestArea})");
            if (ratingText != null)
            {
                int ���� = ������.Length; // ���ڿ��� ���̸� ����

                // ��Ʈ ������ ������ �迭�� �����ϰ�, ���̿� ���� �ش� ���� ����
                ratingText.fontSize = ���� <= 18 ? 0.3f :
                                      ���� <= 21 ? 0.25f :
                                      ���� <= 27 ? 0.2f : 0.15f;

                ratingText.text = ������; // ���� ǥ��
            }
        }
        else
        {
            Debug.LogWarning("������ �׸��� ũ�⸦ ã�� �� �����ϴ�.");
        }
    }

    void Update()
    {
        // ���콺 Ŭ�� ���� �� ���� ����
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Ŭ���� ��ü�� �������� Ȯ��
                if (hit.transform.gameObject.CompareTag("Furniture"))
                {
                    currentFurniture = hit.transform.gameObject; // ���õ� ����
                    originalPosition = currentFurniture.transform.position; // ���� ��ġ ����
                    return; // ������ Ŭ���� ���, ���� ������ �������� ����
                }

                // Ŭ���� ��ġ�� ���� ����� �׸��� ���� ��ġ�� ��ȯ
                Vector3 furniturePosition = GetClosestCellPosition(hit.point);

                // ���� ��ġ�� ������ �̹� ��ġ�Ǿ� �ִ��� Ȯ��
                if (!occupiedPositions.Contains(furniturePosition) ||
                    (currentFurniture != null && furniturePosition == originalPosition))
                {
                    Debug.Log("����");
                    // ���� ���õ� ���������� ���� ����, X�� -90�� ȸ��
                    Quaternion rotation = Quaternion.Euler(-90f, 0f, 0f);
                    GameObject newFurniture = Instantiate(furniturePrefabs[currentPrefabIndex], furniturePosition, rotation);

                    // ���� �̸��� z ���� ������ Dictionary
                    Dictionary<string, float> positionMap = new Dictionary<string, float>
                    {
                        { "Fridge(Clone)", -1.25f },
                        { "Dryer(Clone)", -0.95f },
                        { "AirFryer(Clone)", -0.7f },
                        { "Coffee Maker(Clone)", -0.95f }
                    };

                    // newFurniture�� �̸��� positionMap�� �ִ��� Ȯ��
                    if (positionMap.ContainsKey(newFurniture.name))
                    {
                        // ��ġ ����
                        Vector3 newPosition = newFurniture.transform.position;
                        newPosition.z = positionMap[newFurniture.name]; // �ش� �̸��� �´� z �� ����
                        newFurniture.transform.position = newPosition;

                        // ȸ�� ���� (x = -270, y = 0, z = 0)
                        Quaternion newRotation = Quaternion.Euler(-270f, 0f, 0f);
                        newFurniture.transform.rotation = newRotation;
                    }
                    occupiedPositions.Add(furniturePosition); // ���� ��ġ �� ��ġ �߰�
                    UpdateFurnitureCount(newFurniture); // ���� ���� ������Ʈ
                    CalculateAestheticScore(); // ���� ���� ���
                }
                else
                {
                    Debug.Log("�� ��ġ���� �̹� ������ ��ġ�Ǿ� �ֽ��ϴ�: " + furniturePosition);
                }
            }
        }

        // �巡�� ���� ���� ��ġ ������Ʈ
        if (currentFurniture != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 dragPosition = GetClosestCellPosition(hit.point);

                // �巡�� ��ġ�� �̹� ������ �ִ��� Ȯ��
                if (!occupiedPositions.Contains(dragPosition) || dragPosition == originalPosition)
                {
                    currentFurniture.transform.position = dragPosition; // �巡�� ���� ���� ��ġ ������Ʈ
                }
            }
        }

        // ���콺 ��ư�� ���� ������ ���� ��ġ�� �ǵ����� ���
        if (Input.GetMouseButtonUp(0) && currentFurniture != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 dropPosition = GetClosestCellPosition(currentFurniture.transform.position);

                // ��� ��ġ�� ������ �ִ��� Ȯ�� �� Y�� 3 �̻��� ��� üũ
                if ((!occupiedPositions.Contains(dropPosition) || dropPosition == originalPosition) && dropPosition.y < 3)
                {
                    // �� ������ ����� ��ġ�� ����
                    Quaternion rotation = Quaternion.Euler(-90f, 0f, 0f);
                    GameObject newFurniture = Instantiate(furniturePrefabs[currentPrefabIndex], dropPosition, rotation);

                    // ���� �̸��� z ���� ������ Dictionary
                    Dictionary<string, float> positionMap = new Dictionary<string, float>
                    {
                        { "Fridge(Clone)", -1.25f },
                        { "Dryer(Clone)", -0.95f },
                        { "AirFryer(Clone)", -0.7f },
                        { "Coffee Maker(Clone)", -0.95f }
                    };

                    // newFurniture�� �̸��� positionMap�� �ִ��� Ȯ��
                    if (positionMap.ContainsKey(newFurniture.name))
                    {
                        // ��ġ ����
                        Vector3 newPosition = newFurniture.transform.position;
                        newPosition.z = positionMap[newFurniture.name]; // �ش� �̸��� �´� z �� ����
                        newFurniture.transform.position = newPosition;

                        // ȸ�� ���� (x = -270, y = 0, z = 0)
                        Quaternion newRotation = Quaternion.Euler(-270f, 0f, 0f);
                        newFurniture.transform.rotation = newRotation;
                    }

                    occupiedPositions.Add(dropPosition); // ��ġ �� ��ġ �߰�
                    UpdateFurnitureCount(newFurniture); // ���� ���� ������Ʈ
                    CalculateAestheticScore(); // ���� ���� ���
                }
                else
                {
                    Debug.Log("�� ��ġ���� �̹� ������ ��ġ�Ǿ� �ְų� Y ��ġ�� 3 �̻��Դϴ�: " + dropPosition);
                }
            }

            // �巡�� ����
            currentFurniture.transform.position = originalPosition; // ���� ��ġ�� �ǵ���
            currentFurniture = null; // ���� ������ null�� ����
        }
    }

    Vector3 GetClosestCellPosition(Vector3 hitPoint)
    {
        float cellSize = 1f; // �׸��� �� ũ�� (�ʿ信 ���� ����)
        float roundedX = gridSizeX % 2 == 0 ? Mathf.Floor(hitPoint.x + gridSizeX / 2f) - gridSizeX / 2f + 0.5f : Mathf.Round(hitPoint.x);
        float roundedY = gridSizeY % 2 == 0 ? Mathf.Floor(hitPoint.y + gridSizeY / 2f) - gridSizeY / 2f + 0.5f : Mathf.Round(hitPoint.y);

        Vector3 closestPosition = new Vector3(roundedX, roundedY, -0.5f);
        return closestPosition;
    }

    // ���� ������ �ε����� �����ϴ� �޼���
    public void SetCurrentPrefabIndex(int index)
    {
        if (index >= 0 && index < furniturePrefabs.Count)
        {
            currentPrefabIndex = index;
            Debug.Log("���� ������ �ε��� ����: " + currentPrefabIndex);
        }
        else
        {
            Debug.LogError("��ȿ���� ���� �ε���: " + index);
        }
    }

    // �������� �߰��ϴ� �޼���
    public void AddPrefab(GameObject prefab)
    {
        if (furniturePrefabs.Count < 3)
        {
            if (!furniturePrefabs.Contains(prefab))
            {
                furniturePrefabs.Add(prefab);
                currentPrefabIndex = furniturePrefabs.Count - 1;
                Debug.Log("������ �߰���: " + prefab.name);
            }
            else
            {
                Debug.Log("�̹� �����ϴ� ������: " + prefab.name);
            }
        }
        else
        {
            Debug.Log("�������� �ִ� ����(3��)�� �����߽��ϴ�.");
        }
    }

    // ���� ������ ������Ʈ�ϰ� ���� ���
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

        // ���� ������ 3�� �̻��̸� ���� ����
        if (furnitureCount[newFurniture] > 3)
        {
            aestheticScore -= 10; // �ߺ� penalty
        }
    }

    // ���� ���� ��� �޼���
    private void CalculateAestheticScore()
    {
        aestheticScore = 0; // ���� �ʱ�ȭ

        // �� ������ ���� ���� �ο�
        int emptySpaces = (gridSizeX * gridSizeY) - occupiedPositions.Count;
        aestheticScore += emptySpaces * 5; // �� ������ ����

        // �̵� ���� ���� ��
        foreach (var position in occupiedPositions)
        {
            aestheticScore += CountMovableSpaces(position) * 3; // �̵� ������ ������ ����
        }

        // �߾ӿ� �������� ���� ����
        Vector3 gridCenter = new Vector3(gridSizeX / 2f, gridSizeY / 2f, 0);
        foreach (var position in occupiedPositions)
        {
            float distanceToCenter = Vector3.Distance(position, gridCenter);
            aestheticScore += Mathf.Max(0, (int)(10 - distanceToCenter)); // �������� ���� ����
        }

        // ���� ���� �Ÿ� ���
        List<Vector3> positionsList = new List<Vector3>(occupiedPositions);
        for (int i = 0; i < positionsList.Count; i++)
        {
            for (int j = i + 1; j < positionsList.Count; j++)
            {
                float distanceBetweenFurniture = Vector3.Distance(positionsList[i], positionsList[j]);
                if (distanceBetweenFurniture < 1.5f) // �ʹ� ������ ���� ����
                {
                    aestheticScore -= 5; // ������ ���� ����
                }
            }
        }

        // �������� ��ȭ �� (����: Ư�� ���� ����)
        if (occupiedPositions.Contains(new Vector3(1, 1, 0)) && occupiedPositions.Contains(new Vector3(2, 1, 0))) // (1,1)�� (2,1) ��ġ
        {
            aestheticScore += 10; // �߰� ����
        }

        Debug.Log("���� ���� ����: " + aestheticScore);
    }

    // �־��� ��ġ�� �̵� ������ ���� ���� ����ϴ� �޼���
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

            // �׸��� ���� ���� �ִ��� Ȯ���ϰ�, �ش� ��ġ�� ������ ������ �̵� ����
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

    // ���� �ؽ�Ʈ ������Ʈ �޼���
    private void UpdateRatingText()
    {
        if (ratingText != null)
        {
            ratingText.text = "���� ����: " + aestheticScore; // ���� ǥ��
        }
    }
}
