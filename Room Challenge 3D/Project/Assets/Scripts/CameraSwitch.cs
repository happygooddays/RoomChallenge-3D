using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;

public class CameraSwitch : MonoBehaviour
{
    public int gridSizeX = 5; // X ���� �׸��� ũ��
    public int gridSizeY = 5; // Y ���� �׸��� ũ��

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

    // ī�޶� ��ġ�� ȸ�� �� �迭 (�� ī�޶� ������ ����Ʈ�� ����)
    private Vector3[] cameraPositions = new Vector3[]
    {
        new Vector3(0, 1, -10), // ó�� ��ġ
        new Vector3(-5, 0, -10), // �ι�° ��ġ
        new Vector3(5, 0, -10),  // ����° ��ġ
        new Vector3(0, -5, -10), // �׹�° ��ġ
    };

    private Vector3[] cameraRotations = new Vector3[]
    {
        new Vector3(0, 0, 0),    // ó�� ȸ��
        new Vector3(0, 30, 0),   // �ι�° ȸ��
        new Vector3(0, -30, 0),  // ����° ȸ��
        new Vector3(-45, 0, 0),  // �׹�° ȸ��
    };

    private int currentCameraIndex = 0; // ���� ī�޶� ���� (0���� ����)

    public Camera mainCamera; // ���� ī�޶�

    // Start �޼��忡�� ó�� ��ġ�� ȸ�� ����
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
            // ó�� ������ �� ù ��° ī�޶� ��ġ�� ����
            SetCameraPositionAndRotation();
            int cameraPositionX = gridSizeX + 1;
            int cameraPositionZ = (gridSizeX + 1) * 2;
            cameraPositions = new Vector3[]
            {
                new Vector3(0, 1, -cameraPositionZ), // ó�� ��ġ
                new Vector3(-cameraPositionX, 0, -cameraPositionZ), // �ι�° ��ġ
                new Vector3(cameraPositionX, 0, -cameraPositionZ),  // ����° ��ġ
                new Vector3(0, -cameraPositionX, -cameraPositionZ), // �׹�° ��ġ
            };
        }
        else
        {
            Debug.LogWarning("������ �׸��� ũ�⸦ ã�� �� �����ϴ�.");
        }
    }

    // ��ư Ŭ�� �� ȣ��Ǵ� �޼���
    public void OnButtonClick()
    {
        // ���� ���¿��� ���� ī�޶� ���·� ���� (���������� ����)
        currentCameraIndex++;

        // �迭�� ũ�⸦ �Ѿ�� �ʵ��� ��ȯ (4�� ���� �ٽ� 1������ ���ƿ�)
        if (currentCameraIndex >= cameraPositions.Length)
        {
            currentCameraIndex = 0;
        }

        // ī�޶� ��ġ�� ȸ�� ������Ʈ
        SetCameraPositionAndRotation();
    }

    // ī�޶� ��ġ�� ȸ���� �����ϴ� �޼���
    private void SetCameraPositionAndRotation()
    {
        // ���� �ε����� �ش��ϴ� ��ġ�� ȸ�� �� ����
        mainCamera.transform.position = cameraPositions[currentCameraIndex];
        mainCamera.transform.rotation = Quaternion.Euler(cameraRotations[currentCameraIndex]);

        // ī�޶� ��� ����: ù ��° ī�޶�� Orthographic, �������� Perspective
        if (currentCameraIndex == 0)
        {
            mainCamera.orthographic = true;  // ù ��° ī�޶�� Orthographic
        }
        else
        {
            mainCamera.orthographic = false; // ������ ī�޶�� Perspective
        }

        // ��ġ�� ȸ�� ������ �α�
        Debug.Log("Camera Position: " + cameraPositions[currentCameraIndex]);
        Debug.Log("Camera Rotation: " + cameraRotations[currentCameraIndex]);
    }
}
