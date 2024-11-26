using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // 씬 전환을 위해

public class Stage : MonoBehaviour
{
    private bool isActive = true; // 클릭을 처리할지 말지 결정하는 플래그

    private void OnMouseDown() // Mouse 클릭 이벤트 처리
    {
        if (!isActive) return; // 클릭이 비활성화되어 있으면 아무것도 하지 않음

        if (gameObject.name == "Stage1") // 이름이 "Stage1"인지 확인
        {
            goStage("Stage1"); // 클릭 시 씬 전환 메서드 호출
        }
        else if (gameObject.name == "Stage2") // 이름이 "Stage2"인지 확인
        {
            goStage("Stage2"); // 클릭 시 씬 전환 메서드 호출
        }
        else if (gameObject.name == "Stage3") // 이름이 "Stage3"인지 확인
        {
            goStage("Stage3"); // 클릭 시 씬 전환 메서드 호출
        }
    }

    // 씬 전환 메서드
    public void goStage(string sceneName)
    {
        SceneManager.LoadScene(sceneName); // 씬 변경
    }

    // 클릭 비활성화 메서드
    public void DisableClick()
    {
        isActive = false; // 클릭 비활성화
    }

    // 클릭 활성화 메서드
    public void EnableClick()
    {
        isActive = true; // 클릭 활성화
    }
}
