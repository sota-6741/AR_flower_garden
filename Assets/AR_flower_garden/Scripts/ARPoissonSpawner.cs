using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using static PoissonDiskSampler2DCircle; // Namespace where PoissonDiskSampler2DCircle is defined

[RequireComponent(typeof(ARRaycastManager))]
public class ARPoissonSpawner : MonoBehaviour
{
    [Header("AR Raycast")]
    [SerializeField] private ARRaycastManager _raycastManager;

    [Header("Spawn Settings")]
    [Tooltip("配置するプレハブのリスト")] public GameObject[] prefabList;
    [Tooltip("配置領域の半径 (m)")] public float spawnRadius = 1f;
    [Tooltip("オブジェクト同士の最小距離 (m)")] public float minDistance = 0.2f;

    private static List<ARRaycastHit> _hits = new List<ARRaycastHit>();

    public void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            SpawnAtTouch(Input.GetTouch(0).position);
        }
    }

    public void SpawnAtTouch(Vector2 screenPosition)
    {
        if (_raycastManager.Raycast(screenPosition, _hits, TrackableType.PlaneWithinPolygon))
        {
            Vector3 center = _hits[0].pose.position;
            SpawnAtCenter(center);
        }
    }
    
    public void SpawnAtCenter(Vector3 center)
    {
        // サンプラーを生成 (円形領域専用)
        var sampler = new PoissonDiskSampler2DCircle(spawnRadius);

        foreach (Vector2 offset2D in sampler.Samples())
        {
            // XZ 平面にマッピング
            Vector3 spawnPos = center + new Vector3(offset2D.x, 0f, offset2D.y);

            // prefabListからランダムに選択
            var prefab = prefabList[Random.Range(0, prefabList.Length)];

            // インスタンス生成
            Instantiate(prefab, spawnPos, Quaternion.identity);
        }
    }
}
