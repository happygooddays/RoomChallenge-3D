using System.Collections;
using System.Collections.Generic;
using TMPro; // TextMeshPro 관련 네임스페이스 추가
using UnityEngine;
using UnityEngine.EventSystems; // 클릭 이벤트를 위한 네임스페이스
using UnityEngine.SceneManagement; // 씬 관리를 위한 네임스페이스
using UnityEngine.UI; // UI 요소를 사용하기 위한 네임스페이스

public class scorePopup : MonoBehaviour
{
    public GameObject scorePopupGO; // 팝업창 게임 오브젝트
    public TextMeshProUGUI scoreText;
    public Button closeButton; // 닫기 버튼

    void Start()
    {
        scorePopupGO.SetActive(false); // 시작할 때 팝업창 숨기기
        closeButton.onClick.AddListener(CloseScorePopup); // 닫기 버튼 클릭 리스너 추가
    }

    // 닫기 버튼 클릭 시 호출되는 메서드
    public void OpenScorePopup()
    {
        scorePopupGO.SetActive(true); // 팝업창을 활성화
    }

    // 닫기 버튼 클릭 시 호출되는 메서드
    public void CloseScorePopup()
    {
        scorePopupGO.SetActive(false); // 팝업창을 비활성화하여 숨김
        goStage("Stage");
    }

    public void goStage(string sceneName)
    {
        SceneManager.LoadScene(sceneName); // 씬 변경
    }
}
