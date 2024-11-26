using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Furniture3DPreview : MonoBehaviour
{
    public GameObject furniturePrefab; // Ŭ���� ���� ������
    private FurnitureManager furnitureManager; // FurnitureManager ����

    void Start()
    {
        furnitureManager = FindObjectOfType<FurnitureManager>(); // FurnitureManager ã��
    }

    void Update()
    {
        // ���콺 Ŭ�� ����
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Ŭ���� ��ü�� ���� ��ũ��Ʈ�� �پ� �ִ� �������� Ȯ��
                if (hit.transform.gameObject == gameObject)
                {
                    Debug.Log("���� Ŭ����: " + furniturePrefab.name);

                    // ���� ������ �ε����� ���� �Ǵ� �߰�
                    for (int i = 0; i < furnitureManager.furniturePrefabs.Count; i++)
                    {
                        if (furnitureManager.furniturePrefabs[i] == furniturePrefab)
                        {
                            furnitureManager.SetCurrentPrefabIndex(i); // ���� �������� �ε��� ����
                            return; // �Լ� ����
                        }
                    }

                    // �迭�� �������� ������ �߰�
                    furnitureManager.AddPrefab(furniturePrefab);
                }
            }
        }
    }
}
