using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMeshPro 관련 네임스페이스 추가

public class StageManager : MonoBehaviour
{
    public GameObject stage1Button;
    public GameObject stage2Button;
    public GameObject stage3Button;

    public Material defaultMaterial;  // 기본 Material (Default-Material)
    public Material mat1;  // Mat 1 Material

    public Camera mainCamera;  // 메인 카메라

    private float lastPressTime = 0f; // 마지막으로 눌린 시간
    private float doublePressTime = 1f; // 두 번 누를 때까지의 시간 간격 (1초로 설정)

    private CameraDragController cameraDragController;

    private void Start()
    {
        UpdateStageButtons();
    }

    void Update()
    {
        // 안드로이드에서 하드웨어 뒤로가기 버튼 (Escape 키) 감지
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 두 번 연속으로 눌렸는지 확인
            if (Time.time - lastPressTime <= doublePressTime)
            {
                // 뒤로가기 버튼이 두 번 눌렸을 때 처리
                Debug.Log("뒤로가기 버튼을 두 번 눌렀습니다.");

                // 두 번 누르면 애플리케이션 종료
                Application.Quit();  // 앱 종료를 원하면 이 줄을 활성화
            }

            // 마지막 눌린 시간 갱신
            lastPressTime = Time.time;
        }
    }

    // 각 스테이지 버튼의 상태 업데이트
    private void UpdateStageButtons()
    {
        // 각 스테이지의 클리어 상태 가져오기
        bool isStage1Clear = PlayerPrefs.GetInt("Stage1Clear", 0) == 1;
        bool isStage2Clear = PlayerPrefs.GetInt("Stage2Clear", 0) == 1;
        bool isStage3Clear = PlayerPrefs.GetInt("Stage3Clear", 0) == 1;

        // Stage 1 버튼 상태 설정
        SetButtonState(stage1Button, true);

        // Stage 2 버튼 상태 설정 (Stage 1 클리어되어야만 활성화됨)
        SetButtonState(stage2Button, isStage1Clear);

        // Stage 3 버튼 상태 설정 (Stage 2 클리어되어야만 활성화됨)
        SetButtonState(stage3Button, isStage2Clear);
    }

    // 버튼 상태 설정 (클리어 여부에 따라)
    private void SetButtonState(GameObject GO, bool isClear)
    {
        // Stage 스크립트를 가져옴
        Stage stageScript = GO.GetComponent<Stage>();

        // Material 변경
        MeshRenderer GORenderer = GO.GetComponent<MeshRenderer>();

        // 버튼이 비활성화 되어야 할 때
        if (!isClear)
        {
            // MonoBehaviour 비활성화
            if (stageScript != null)
            {
                stageScript.DisableClick();  // 클릭 비활성화
            }

            // Material 변경
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

            // MonoBehaviour 활성화
            if (stageScript != null)
            {
                stageScript.EnableClick();  // 클릭 활성화
            }

            // Material 변경
            if (GORenderer != null)
            {
                GORenderer.material = defaultMaterial;
            }
        }
    }
}
