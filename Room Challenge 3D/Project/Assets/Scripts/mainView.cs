using System.Collections;
using System.Collections.Generic;
using TMPro; // TextMeshPro ���� ���ӽ����̽� �߰�
using UnityEngine;
using UnityEngine.EventSystems; // Ŭ�� �̺�Ʈ�� ���� ���ӽ����̽�
using UnityEngine.SceneManagement; // �� ������ ���� ���ӽ����̽�
using UnityEngine.UI; // UI ��Ҹ� ����ϱ� ���� ���ӽ����̽�

public class mainView : MonoBehaviour
{
    public GameObject cube_Start; // �˾�â ���� ������Ʈ
    public GameObject cube_Help; // Start Ŭ���� ���� 3D ������Ʈ
    public GameObject helpPopup; // �˾�â ���� ������Ʈ
    
    public Button closeButton; // �ݱ� ��ư
    public Button firstButton; // 1������ �̵� ��ư
    public Button secondButton; // 2������ �̵� ��ư

    public TextMeshProUGUI helpText;

    private float lastPressTime = 0f; // ���������� ���� �ð�
    private float doublePressTime = 1f; // �� �� ���� �������� �ð� ���� (1�ʷ� ����)

    void Start()
    {
        helpPopup.SetActive(false); // ������ �� �˾�â �����
        firstButton.gameObject.SetActive(false);
        closeButton.onClick.AddListener(CloseHelpPopup); // �ݱ� ��ư Ŭ�� ������ �߰�
        firstButton.onClick.AddListener(firstHelpText); // 1������ �̵� ��ư Ŭ�� ������ �߰�
        secondButton.onClick.AddListener(secondHelpText); // 2������ �̵� ��ư Ŭ�� ������ �߰�
    }

    void Update()
    {
        // �ȵ���̵忡�� �ϵ���� �ڷΰ��� ��ư (Escape Ű) ����
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // �� �� �������� ���ȴ��� Ȯ��
            if (Time.time - lastPressTime <= doublePressTime)
            {
                // �ڷΰ��� ��ư�� �� �� ������ �� ó��
                Debug.Log("�ڷΰ��� ��ư�� �� �� �������ϴ�.");

                // �� �� ������ ���ø����̼� ����
                Application.Quit();  // �� ���Ḧ ���ϸ� �� ���� Ȱ��ȭ
            }

            // ������ ���� �ð� ����
            lastPressTime = Time.time;
        }
    }

    public void firstHelpText()
    {
        firstButton.gameObject.SetActive(false);
        secondButton.gameObject.SetActive(true);
        helpText.text = "���� ��ǥ\n�־��� �� ������� ������ �̿���\n���� �ٹ̰�, ��ǥ ������ �޼��ϼ���.\n\n�ֿ� ���\n��: ���� ������ �� ������������ �ٸ��ϴ�.\n����: �پ��� ���� ���� �巡���Ͽ� �濡 ��ġ�ϼ���.\n\n���� �ý���\n���� ��ġ�� ���� ��ȭ�� ���� ������ �޽��ϴ�. ��ǥ ������ �����ؾ� ���� ���������� �Ѿ �� �ֽ��ϴ�.\n\n���� ���\n������ �����ϰ� �巡���Ͽ� ���ϴ� ��ġ�� ��������.";
        helpText.alignment = TMPro.TextAlignmentOptions.Center; // ��� ����
    }

    public void secondHelpText()
    {
        firstButton.gameObject.SetActive(true);
        secondButton.gameObject.SetActive(false);
        helpText.text = "���� ������\n���� ��ġ�� ȿ������ ��ȭ�� ���ϴ� �����Դϴ�.\n���� ��ҵ��� �������� ������ ���˴ϴ�.\n\n1. �� ����: �׸��� ���� ���� �� ������ �������� ������ �������ϴ�.\n2. �̵� ������ ����: �� ���� �ֺ��� �����Ӱ� �̵��� �� �ִ� ������ �������� ������ �ö󰩴ϴ�.\n3. �߾� ��ġ: �׸��� �߾ӿ� ����� �����ϼ��� ������ �����ϴ�.\n4. ���� �� �Ÿ�: �������� �ʹ� ������ ��ġ�Ǹ� ������ �����մϴ�.\n5. ��ȭ�ο� ��ġ: Ư�� �������� �� ��︮�� ��ġ�� ��ġ�Ǹ� �߰� ������ �־����ϴ�.";
        helpText.alignment = TMPro.TextAlignmentOptions.Left; // ���� ����
    }

    public void goStage()
    {
        SceneManager.LoadScene("Stage"); // �� ����
    }

    public void OpenHelpPopup()
    {
        helpPopup.SetActive(true); // �˾�â ����
    }

    public void CloseHelpPopup()
    {
        helpPopup.SetActive(false); // �˾�â�� ��Ȱ��ȭ�Ͽ� ����
    }
}
