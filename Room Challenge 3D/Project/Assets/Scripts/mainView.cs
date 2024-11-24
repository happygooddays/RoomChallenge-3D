using System.Collections;
using System.Collections.Generic;
using TMPro; // TextMeshPro 관련 네임스페이스 추가
using UnityEngine;
using UnityEngine.EventSystems; // 클릭 이벤트를 위한 네임스페이스
using UnityEngine.SceneManagement; // 씬 관리를 위한 네임스페이스
using UnityEngine.UI; // UI 요소를 사용하기 위한 네임스페이스

public class mainView : MonoBehaviour
{
    public GameObject cube_Start; // 팝업창 게임 오브젝트
    public GameObject cube_Help; // Start 클릭을 위한 3D 오브젝트
    public GameObject helpPopup; // 팝업창 게임 오브젝트
    
    public Button closeButton; // 닫기 버튼
    public Button firstButton; // 1페이지 이동 버튼
    public Button secondButton; // 2페이지 이동 버튼

    public TextMeshProUGUI helpText;

    private float lastPressTime = 0f; // 마지막으로 눌린 시간
    private float doublePressTime = 1f; // 두 번 누를 때까지의 시간 간격 (1초로 설정)

    void Start()
    {
        helpPopup.SetActive(false); // 시작할 때 팝업창 숨기기
        firstButton.gameObject.SetActive(false);
        closeButton.onClick.AddListener(CloseHelpPopup); // 닫기 버튼 클릭 리스너 추가
        firstButton.onClick.AddListener(firstHelpText); // 1페이지 이동 버튼 클릭 리스너 추가
        secondButton.onClick.AddListener(secondHelpText); // 2페이지 이동 버튼 클릭 리스너 추가
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

    public void firstHelpText()
    {
        firstButton.gameObject.SetActive(false);
        secondButton.gameObject.SetActive(true);
        helpText.text = "게임 목표\n주어진 방 사이즈와 가구를 이용해\n방을 꾸미고, 목표 평점을 달성하세요.\n\n주요 요소\n방: 방의 구조는 각 스테이지마다 다릅니다.\n가구: 다양한 가구 모델을 드래그하여 방에 배치하세요.\n\n평점 시스템\n가구 배치와 색상 조화에 따라 점수를 받습니다. 목표 평점에 도달해야 다음 스테이지로 넘어갈 수 있습니다.\n\n조작 방법\n가구를 선택하고 드래그하여 원하는 위치에 놓으세요.";
        helpText.alignment = TMPro.TextAlignmentOptions.Center; // 가운데 정렬
    }

    public void secondHelpText()
    {
        firstButton.gameObject.SetActive(true);
        secondButton.gameObject.SetActive(false);
        helpText.text = "미적 점수는\n가구 배치의 효율성과 조화를 평가하는 점수입니다.\n다음 요소들을 기준으로 점수가 계산됩니다.\n\n1. 빈 공간: 그리드 내에 남은 빈 공간이 많을수록 점수가 높아집니다.\n2. 이동 가능한 공간: 각 가구 주변에 자유롭게 이동할 수 있는 공간이 많을수록 점수가 올라갑니다.\n3. 중앙 배치: 그리드 중앙에 가까운 가구일수록 점수가 높습니다.\n4. 가구 간 거리: 가구들이 너무 가까이 배치되면 점수가 감소합니다.\n5. 조화로운 배치: 특정 가구들이 잘 어울리는 위치에 배치되면 추가 점수가 주어집니다.";
        helpText.alignment = TMPro.TextAlignmentOptions.Left; // 왼쪽 정렬
    }

    public void goStage()
    {
        SceneManager.LoadScene("Stage"); // 씬 변경
    }

    public void OpenHelpPopup()
    {
        helpPopup.SetActive(true); // 팝업창 열기
    }

    public void CloseHelpPopup()
    {
        helpPopup.SetActive(false); // 팝업창을 비활성화하여 숨김
    }
}
