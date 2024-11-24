using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class rtSceneNum : MonoBehaviour
{
    // 현재 씬의 이름과 스테이지번호
    private string sceneName;
    private int sceneNum;

    void Start()
    {
        sceneName = SceneManager.GetActiveScene().name;

        // 마지막 문자를 숫자로 변환
        sceneNum = int.Parse(sceneName.Substring(sceneName.Length - 1));
    }

    void Update()
    {
        // 안드로이드에서 하드웨어 뒤로가기 버튼 (Escape 키) 감지
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 첫 번째 버튼을 눌렀을 때
            goStage("Stage");
        }
    }

    public void goStage(string sceneName)
    {
        SceneManager.LoadScene(sceneName); // 씬 변경
    }

    public string referencePath()
    {
        return "apartment_data/" + GetApartmentReference(sceneNum);
    }

    public int apartNum()
    {
        return sceneNum;
    }

    string GetApartmentReference(int sceneNum)
    {
        return sceneNum switch
        {
            1 => "9",
            2 => "789",
            3 => "13370",
            _ => "0"  // 예기치 않은 경우 기본값 "0"
        };
    }
}
