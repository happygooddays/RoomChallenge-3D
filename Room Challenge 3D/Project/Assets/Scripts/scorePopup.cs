using System.Collections;
using System.Collections.Generic;
using TMPro; // TextMeshPro ���� ���ӽ����̽� �߰�
using UnityEngine;
using UnityEngine.EventSystems; // Ŭ�� �̺�Ʈ�� ���� ���ӽ����̽�
using UnityEngine.SceneManagement; // �� ������ ���� ���ӽ����̽�
using UnityEngine.UI; // UI ��Ҹ� ����ϱ� ���� ���ӽ����̽�

public class scorePopup : MonoBehaviour
{
    public GameObject scorePopupGO; // �˾�â ���� ������Ʈ
    public TextMeshProUGUI scoreText;
    public Button closeButton; // �ݱ� ��ư

    void Start()
    {
        scorePopupGO.SetActive(false); // ������ �� �˾�â �����
        closeButton.onClick.AddListener(CloseScorePopup); // �ݱ� ��ư Ŭ�� ������ �߰�
    }

    // �ݱ� ��ư Ŭ�� �� ȣ��Ǵ� �޼���
    public void OpenScorePopup()
    {
        scorePopupGO.SetActive(true); // �˾�â�� Ȱ��ȭ
    }

    // �ݱ� ��ư Ŭ�� �� ȣ��Ǵ� �޼���
    public void CloseScorePopup()
    {
        scorePopupGO.SetActive(false); // �˾�â�� ��Ȱ��ȭ�Ͽ� ����
        goStage("Stage");
    }

    public void goStage(string sceneName)
    {
        SceneManager.LoadScene(sceneName); // �� ����
    }
}
