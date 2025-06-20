using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class FireworkTest : MonoBehaviour
{
    [SerializeField] private VisualEffect explosionVFX;
    [SerializeField] private int maxSimultaneousExplosions = 4;
   
   private Queue<int> availableSpawnIndices = new Queue<int>();
   private Dictionary<int, float> spawnUsageTime = new Dictionary<int, float>();
   private float spawnCooldownTime = 3f; // エフェクト継続時間

   void Start()
   {
        Debug.Log($"maxSimultaneousExplosions = {maxSimultaneousExplosions}"); 
       if (explosionVFX == null)
            explosionVFX = GetComponent<VisualEffect>();

        // VFXウォームアップ
        if (explosionVFX != null)
        {
            explosionVFX.Stop();
            explosionVFX.Reinit();
        }

        // 利用可能なSpawnインデックスを初期化
        for (int i = 0; i < maxSimultaneousExplosions; i++)
        {
            availableSpawnIndices.Enqueue(i);
            Debug.Log($"Enqueued spawn index: {i}"); // エンキューされる値を確認
        }
   }

   void Update()
   {
       // 使用済みSpawnの解放チェック
       CheckAndReleaseExpiredSpawns();
   }

   public void TriggerExplosion(Vector3 position, int particleCount = 10000)
   {
       int spawnIndex = GetAvailableSpawnIndex();
       Debug.Log($"Using spawn index: {spawnIndex}, maxSimultaneous: {maxSimultaneousExplosions}"); // この値を確認
       if (spawnIndex == -1)
        {
            Debug.LogWarning("No available spawn slots. Max simultaneous explosions reached.");
            return;
        }

        //    // Event Attributeの作成と設定
        //    VFXEventAttribute eventAttr = explosionVFX.CreateVFXEventAttribute();
        //    if (eventAttr.HasVector3("pos"))
        //        eventAttr.SetVector3("pos", position);
        //    else
        //        Debug.LogWarning("VFX Event Attribute does not have 'pos'");

        //    if (eventAttr.HasUint("count"))
        //        eventAttr.SetUint("count", (uint)particleCount);
        //    else
        //        Debug.LogWarning("VFX Event Attribute does not have 'count'");
        //    // 対応するEventを発火
        //    string eventName = $"Fireworks{spawnIndex}";
        //    explosionVFX.SendEvent(eventName, eventAttr);
    
        explosionVFX.SetVector3($"pos{spawnIndex}", position);
        explosionVFX.SetUInt($"count{spawnIndex}", (uint)particleCount);
        explosionVFX.SendEvent($"Fireworks{spawnIndex}");
       

       
       // 使用開始時刻を記録
        spawnUsageTime[spawnIndex] = Time.time;
       
       Debug.Log($"FireworkTest : Firework triggered at position: {position}, with particle count: {particleCount}, using spawn index {spawnIndex}");
   }

   private int GetAvailableSpawnIndex()
   {
       if (availableSpawnIndices.Count > 0)
       {
           return availableSpawnIndices.Dequeue();
       }
       return -1; // 利用可能なスロットなし
   }

   private void CheckAndReleaseExpiredSpawns()
   {
       List<int> expiredSpawns = new List<int>();
       
       foreach (var kvp in spawnUsageTime)
       {
           if (Time.time - kvp.Value >= spawnCooldownTime)
           {
               expiredSpawns.Add(kvp.Key);
           }
       }
       
       foreach (int spawnIndex in expiredSpawns)
       {
           availableSpawnIndices.Enqueue(spawnIndex);
           spawnUsageTime.Remove(spawnIndex);
           Debug.Log($"Spawn index {spawnIndex} released and available for reuse");
       }
   }
}