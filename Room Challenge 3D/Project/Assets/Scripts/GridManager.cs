using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;

public class GridManager : MonoBehaviour
{
    public GameObject gridCellPrefab; // �׸��� ���� ����� ������
    public int gridSizeX = 5; // X ���� �׸��� ũ��
    public int gridSizeY = 5; // Y ���� �׸��� ũ��
    public float cellSize = 1f; // �� ũ��
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
                        Debug.Log("�������: " + �������);
                        SetGridSizeForArea(�������);
                    }
                    else
                    {
                        Debug.LogWarning("�����Ͱ� �������� �ʽ��ϴ�.");
                    }
                }
            });
    }

    // ��������� �´� ���� ����� �׸��� ũ�� ����
    void SetGridSizeForArea(float �������)
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
            CreateGrid();
        }
        else
        {
            Debug.LogWarning("������ �׸��� ũ�⸦ ã�� �� �����ϴ�.");
        }
    }

    // �׸��� ����
    void CreateGrid()
    {
        // ������ gridSizeX�� gridSizeY�� �׸��� ����
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
