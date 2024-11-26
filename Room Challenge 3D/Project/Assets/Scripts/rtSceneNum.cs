using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class rtSceneNum : MonoBehaviour
{
    // ���� ���� �̸��� ����������ȣ
    private string sceneName;
    private int sceneNum;

    void Start()
    {
        sceneName = SceneManager.GetActiveScene().name;

        // ������ ���ڸ� ���ڷ� ��ȯ
        sceneNum = int.Parse(sceneName.Substring(sceneName.Length - 1));
    }

    void Update()
    {
        // �ȵ���̵忡�� �ϵ���� �ڷΰ��� ��ư (Escape Ű) ����
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // ù ��° ��ư�� ������ ��
            goStage("Stage");
        }
    }

    public void goStage(string sceneName)
    {
        SceneManager.LoadScene(sceneName); // �� ����
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
            _ => "0"  // ����ġ ���� ��� �⺻�� "0"
        };
    }
}
