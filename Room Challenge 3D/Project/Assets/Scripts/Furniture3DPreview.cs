using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Furniture3DPreview : MonoBehaviour
{
    public GameObject furniturePrefab; // 클릭된 가구 프리팹
    private FurnitureManager furnitureManager; // FurnitureManager 참조

    void Start()
    {
        furnitureManager = FindObjectOfType<FurnitureManager>(); // FurnitureManager 찾기
    }

    void Update()
    {
        // 마우스 클릭 감지
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // 클릭한 객체가 현재 스크립트가 붙어 있는 가구인지 확인
                if (hit.transform.gameObject == gameObject)
                {
                    Debug.Log("가구 클릭됨: " + furniturePrefab.name);

                    // 현재 프리팹 인덱스를 변경 또는 추가
                    for (int i = 0; i < furnitureManager.furniturePrefabs.Count; i++)
                    {
                        if (furnitureManager.furniturePrefabs[i] == furniturePrefab)
                        {
                            furnitureManager.SetCurrentPrefabIndex(i); // 기존 프리팹의 인덱스 설정
                            return; // 함수 종료
                        }
                    }

                    // 배열에 프리팹이 없으면 추가
                    furnitureManager.AddPrefab(furniturePrefab);
                }
            }
        }
    }
}
