using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // �� ��ȯ�� ����

public class Stage : MonoBehaviour
{
    private bool isActive = true; // Ŭ���� ó������ ���� �����ϴ� �÷���

    private void OnMouseDown() // Mouse Ŭ�� �̺�Ʈ ó��
    {
        if (!isActive) return; // Ŭ���� ��Ȱ��ȭ�Ǿ� ������ �ƹ��͵� ���� ����

        if (gameObject.name == "Stage1") // �̸��� "Stage1"���� Ȯ��
        {
            goStage("Stage1"); // Ŭ�� �� �� ��ȯ �޼��� ȣ��
        }
        else if (gameObject.name == "Stage2") // �̸��� "Stage2"���� Ȯ��
        {
            goStage("Stage2"); // Ŭ�� �� �� ��ȯ �޼��� ȣ��
        }
        else if (gameObject.name == "Stage3") // �̸��� "Stage3"���� Ȯ��
        {
            goStage("Stage3"); // Ŭ�� �� �� ��ȯ �޼��� ȣ��
        }
    }

    // �� ��ȯ �޼���
    public void goStage(string sceneName)
    {
        SceneManager.LoadScene(sceneName); // �� ����
    }

    // Ŭ�� ��Ȱ��ȭ �޼���
    public void DisableClick()
    {
        isActive = false; // Ŭ�� ��Ȱ��ȭ
    }

    // Ŭ�� Ȱ��ȭ �޼���
    public void EnableClick()
    {
        isActive = true; // Ŭ�� Ȱ��ȭ
    }
}
