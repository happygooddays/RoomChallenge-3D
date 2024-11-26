using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // �߰�: �� ��ȯ�� ����
using Firebase;
using Firebase.Database;
using Firebase.Extensions;

public class CameraSwitcher : MonoBehaviour
{
    public Camera mainCamera;  // ���� ī�޶�
    public Camera subCamera;   // ���� ī�޶�
    public Canvas canvas;      // ĵ����

    public GameObject closeGO;
    public GameObject completeGO;
    public GameObject switchCamera;

    private bool isRotating = false;  // ���� ī�޶� ȸ�� ���� ����
    private float rotationSpeed = 30f; // ȸ�� �ӵ�
    private Vector3 a = Vector3.zero;  // �ʱⰪ�� Vector3.zero�� ����

    private float sideLength = 5f;  // �� ���� ����
    private float diagonalLength;  // �밢�� ����

    public int gridSizeX = 5; // X ���� �׸��� ũ��
    public int gridSizeY = 5; // Y ���� �׸��� ũ��

    private int passingScore = 0;

    private DatabaseReference databaseReference;

    [SerializeField]
    public FurnitureManager furnitureManager;  // FurnitureManager �ν��Ͻ� ����

    [SerializeField]
    public scorePopup sP; // ���� �˾�

    private float currentAngle = 0f;  // ī�޶��� ���� ����

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
        closeGO.SetActive(true);
        completeGO.SetActive(true);
        switchCamera.SetActive(true);
        InitializeFirebase();
        diagonalLength = Mathf.Sqrt(2 * Mathf.Pow(sideLength, 2));
        
        // ���� ���� �� main ī�޶� Ȱ��ȭ
        SwitchToMainCamera();
        
        subCamera.nearClipPlane = 0.1f;  // ����� Ŭ���� ���
        subCamera.farClipPlane = diagonalLength / 2;
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
            if (closestSize.x >= closestSize.y)
                sideLength = closestSize.x;
            else
                sideLength = closestSize.y;
        }
        else
        {
            Debug.LogWarning("������ �׸��� ũ�⸦ ã�� �� �����ϴ�.");
        }
    }

    // ī�޶� ��ȯ �޼���
    public void SwitchCamera()
    {
        if (!isRotating)  // ȸ�� ������ ������
        {
            SwitchToSubCamera();  // ���� ī�޶�� ��ȯ
        }
    }

    // main ī�޶�� ��ȯ
    private void SwitchToMainCamera()
    {
        mainCamera.enabled = true;
        subCamera.enabled = false;

        isRotating = false;  // ȸ�� ���� ����
    }

    // sub ī�޶�� ��ȯ
    private void SwitchToSubCamera()
    {
        mainCamera.enabled = false;
        subCamera.enabled = true;

        isRotating = true;  // ȸ�� ����
    }

    void Update()
    {
        // ���� ī�޶� Ȱ��ȭ�Ǿ� �ְ� ȸ�� ���� ��
        if (isRotating && subCamera.enabled)
        {
            // subCamera�� far clipping plane�� 3���� ����
            subCamera.GetComponent<Camera>().farClipPlane = Mathf.Sqrt(gridSizeX * gridSizeX + gridSizeY * gridSizeY)*2;

            // ȸ���� ���� ��� (ȸ�� �ӵ��� ����)
            currentAngle += rotationSpeed * Time.deltaTime;

            // 360���� �� ���� ���� ȸ���� ���ߵ��� ����
            if (currentAngle >= 340f)
            {
                subCamera.transform.position = new Vector3(0f, 0f, -1f);
                subCamera.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
                currentAngle = 340f; // 360���� ���� �ʵ���

                subCamera.transform.eulerAngles = new Vector3(0f, 90f, 0f);
                // ���� ī�޶�� ��ȯ
                SwitchToMainCamera();

                // ȸ�� ���� ����
                isRotating = false;

                // X, Y ������ �ڵ����� ó���ϵ��� ����
                int x = Mathf.Min(gridSizeX, gridSizeY);
                int y = Mathf.Max(gridSizeX, gridSizeY);

                if (IsValidScore(x, y, furnitureManager.aestheticScore))
                {
                    Debug.Log($"���: {x}x{y} ũ�⿡�� ���� {furnitureManager.aestheticScore}�� ������ �����߽��ϴ�.");

                    rtSceneNum rtSceneNumComponent = GameObject.Find("GameObject").GetComponent<rtSceneNum>();
                    int reference = rtSceneNumComponent.apartNum();
                        
                    OnStageComplete(reference);

                    sP.scoreText.text = "��������: " + furnitureManager.aestheticScore + "��\n�������: " + passingScore + "��\nŬ����";
                }
                else
                {
                    Debug.Log($"�����: {x}x{y} ũ�⿡�� ���� {furnitureManager.aestheticScore}�� ������ �������� ���߽��ϴ�.");
                    sP.scoreText.text = "��������: " + furnitureManager.aestheticScore + "��\n�������: " + passingScore + "��\n����";
                }
                closeGO.SetActive(false);
                completeGO.SetActive(false);
                switchCamera.SetActive(false);
                sP.OpenScorePopup();

                return;
            }

            currentAngle = (currentAngle < 200f) ? 200f : currentAngle;

            // ī�޶��� ���ο� ��ġ ��� (���� ���)
            float xx = Mathf.Sqrt(gridSizeX * gridSizeX + gridSizeY * gridSizeY) * Mathf.Cos(Mathf.Deg2Rad * currentAngle);
            float zz = Mathf.Sqrt(gridSizeX * gridSizeX + gridSizeY * gridSizeY) * Mathf.Sin(Mathf.Deg2Rad * currentAngle);

            // ī�޶� ��ġ ������Ʈ
            subCamera.transform.position = new Vector3(xx, 0, zz);
            Debug.Log(currentAngle);
            // ī�޶� �߽��� �ٶ󺸵��� ����
            subCamera.transform.LookAt(Vector3.zero);
        }
    }

    // �Ϸ� ��ư Ŭ�� �� ȣ��� �Լ�
    public void OnStageComplete(int stageNumber)
    {
        // �������� Ŭ���� ���� ��������
        int stageClearStatus = PlayerPrefs.GetInt("Stage" + stageNumber + "Clear", 0); // 0�� �⺻��, �� Ŭ������� ���� ����

        // �̹� Ŭ���� ���¶�� �������� ����
        if (stageClearStatus == 0)
        {
            // �������� Ŭ���� ���� ����
            PlayerPrefs.SetInt("Stage" + stageNumber + "Clear", 1); // 1�� Ŭ���� ����
            PlayerPrefs.Save();
        }
    }

    bool IsValidScore(int gridX, int gridY, float score)
    {
        // �� gridX, gridY�� �����ϴ� �ּ� ������ ��ųʸ��� ����
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

        // (gridX, gridY)�� ���� �ּ� ������ ������
        if (scoreRequirements.TryGetValue((gridX, gridY), out float requiredScore))
        {
            // passingScore�� �ش��ϴ� �ּ� ���� ���� ����
            passingScore = (int)requiredScore;

            // passingScore�� ���Ͽ� ��� ���� ��ȯ
            return score >= passingScore;
        }

        // �ش� ������ ������ false ��ȯ
        return false;
    }


    public void goStage(string sceneName)
    {
        SceneManager.LoadScene(sceneName); // �� ����
    }
}
