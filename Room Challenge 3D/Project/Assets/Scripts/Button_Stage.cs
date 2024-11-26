using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Button_Stage : MonoBehaviour
{
    private void OnMouseDown() // Mouse Ŭ�� �̺�Ʈ ó��
    {
        // ��ư �̸��� Ư�� ��Ͽ� ���ԵǾ� �ִ��� Ȯ���ϰ�, ��ġ�ϸ� �� ��ȯ
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
        SceneManager.LoadScene(sceneName); // �� ����
    }
}
