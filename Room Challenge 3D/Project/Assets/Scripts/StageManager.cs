using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMeshPro ���� ���ӽ����̽� �߰�

public class StageManager : MonoBehaviour
{
    public GameObject stage1Button;
    public GameObject stage2Button;
    public GameObject stage3Button;

    public Material defaultMaterial;  // �⺻ Material (Default-Material)
    public Material mat1;  // Mat 1 Material

    public Camera mainCamera;  // ���� ī�޶�

    private float lastPressTime = 0f; // ���������� ���� �ð�
    private float doublePressTime = 1f; // �� �� ���� �������� �ð� ���� (1�ʷ� ����)

    private CameraDragController cameraDragController;

    private void Start()
    {
        UpdateStageButtons();
    }

    void Update()
    {
        // �ȵ���̵忡�� �ϵ���� �ڷΰ��� ��ư (Escape Ű) ����
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // �� �� �������� ���ȴ��� Ȯ��
            if (Time.time - lastPressTime <= doublePressTime)
            {
                // �ڷΰ��� ��ư�� �� �� ������ �� ó��
                Debug.Log("�ڷΰ��� ��ư�� �� �� �������ϴ�.");

                // �� �� ������ ���ø����̼� ����
                Application.Quit();  // �� ���Ḧ ���ϸ� �� ���� Ȱ��ȭ
            }

            // ������ ���� �ð� ����
            lastPressTime = Time.time;
        }
    }

    // �� �������� ��ư�� ���� ������Ʈ
    private void UpdateStageButtons()
    {
        // �� ���������� Ŭ���� ���� ��������
        bool isStage1Clear = PlayerPrefs.GetInt("Stage1Clear", 0) == 1;
        bool isStage2Clear = PlayerPrefs.GetInt("Stage2Clear", 0) == 1;
        bool isStage3Clear = PlayerPrefs.GetInt("Stage3Clear", 0) == 1;

        // Stage 1 ��ư ���� ����
        SetButtonState(stage1Button, true);

        // Stage 2 ��ư ���� ���� (Stage 1 Ŭ����Ǿ�߸� Ȱ��ȭ��)
        SetButtonState(stage2Button, isStage1Clear);

        // Stage 3 ��ư ���� ���� (Stage 2 Ŭ����Ǿ�߸� Ȱ��ȭ��)
        SetButtonState(stage3Button, isStage2Clear);
    }

    // ��ư ���� ���� (Ŭ���� ���ο� ����)
    private void SetButtonState(GameObject GO, bool isClear)
    {
        // Stage ��ũ��Ʈ�� ������
        Stage stageScript = GO.GetComponent<Stage>();

        // Material ����
        MeshRenderer GORenderer = GO.GetComponent<MeshRenderer>();

        // ��ư�� ��Ȱ��ȭ �Ǿ�� �� ��
        if (!isClear)
        {
            // MonoBehaviour ��Ȱ��ȭ
            if (stageScript != null)
            {
                stageScript.DisableClick();  // Ŭ�� ��Ȱ��ȭ
            }

            // Material ����
            if (GORenderer != null)
            {
                GORenderer.material = mat1;
            }
        }
        else
        {
            Vector3 challengePos = mainCamera.transform.position;
            challengePos.z = GO.transform.position.z - 5;
            mainCamera.transform.position = challengePos;

            cameraDragController = FindObjectOfType<CameraDragController>();

            int stageIndex = int.Parse(GO.name.Replace("Stage", ""));
            cameraDragController.challengeStageIndex(stageIndex-1);

            // MonoBehaviour Ȱ��ȭ
            if (stageScript != null)
            {
                stageScript.EnableClick();  // Ŭ�� Ȱ��ȭ
            }

            // Material ����
            if (GORenderer != null)
            {
                GORenderer.material = defaultMaterial;
            }
        }
    }
}
