using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Button_Stage : MonoBehaviour
{
    private void OnMouseDown() // Mouse 클릭 이벤트 처리
    {
        // 버튼 이름이 특정 목록에 포함되어 있는지 확인하고, 일치하면 씬 전환
        switch (gameObject.name)
        {
            case "Close":
            case "Button_Stage1":
            case "Button_Stage2":
            case "Button_Stage3":
                goStage("Stage");
                break;
        }
    }

    public void goStage(string sceneName)
    {
        SceneManager.LoadScene(sceneName); // 씬 변경
    }
}
