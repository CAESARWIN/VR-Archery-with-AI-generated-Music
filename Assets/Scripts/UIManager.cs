using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("用于显示命中点的UI Image或Panel")]
    public RectTransform uiTargetDisplay;

    [Tooltip("命中标记的UI预制体")]
    public Image point;

    [Header("Target Physical Dimensions")]
    [Tooltip("靶子的总物理半径（单位：米），用于UI映射")]
    public float totalTargetRadius = 2f; // 示例值：靶子总半径为2米

    [Header("Scoring Rings (in Meters)")]
    [Tooltip("从分数最高的（环最小）到分数最低的（环最大）排序")]
    public ScoreRing[] scoreRings;

    [Header("Score Text")]
    public TMP_Text scoreText;
    private int currentScore = 0;

    [Header("UI Update Settings")]
    [Tooltip("UI更新的冷却时间（秒）")]
    public float uiUpdateCooldown = 0.5f;
    private float lastUpdateTime = 0f;

    [Header("Arrows")]
    public TMP_Text arrowsLeftText;
    public ArrowManager arrowManager;

    [Header("Time Text")]
    public TMP_Text timeText; // 用于显示时间的文本（如果需要时）

    // 用于在Inspector中方便地设置得分环带
    [System.Serializable]
    public struct ScoreRing
    {
        public int score;
        [Tooltip("该分数环带的最大半径（单位：米）")]
        public float radiusInMeters;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scoreRings = new ScoreRing[]
        {
            new ScoreRing { score = 10, radiusInMeters = 0.2f }, // 中心环
            new ScoreRing { score = 9, radiusInMeters = 0.4f },  // 第一环
            new ScoreRing { score = 8, radiusInMeters = 0.6f },  // 第二环
            new ScoreRing { score = 7, radiusInMeters = 0.8f }, // 第三环
            new ScoreRing { score = 6, radiusInMeters = 1f }, // 第四环
            new ScoreRing { score = 5, radiusInMeters = 1.2f }, // 第五环
            new ScoreRing { score = 4, radiusInMeters = 1.4f },  // 第六环
            new ScoreRing { score = 3, radiusInMeters = 1.6f },  // 第七环
            new ScoreRing { score = 2, radiusInMeters = 1.8f },  // 第八环
            new ScoreRing { score = 1, radiusInMeters = totalTargetRadius } // 最外环
        };

        scoreText.text = currentScore.ToString();
        if (arrowManager != null)
        {
            arrowsLeftText.text = arrowManager.arrowsLeft.ToString();
        }
    }

    public void UpdateArrowsLeft()
    {
        if (arrowManager != null)
        {
            arrowsLeftText.text = arrowManager.arrowsLeft.ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 根据局部坐标计算得分
    /// </summary>
    private int CalculateScore(Vector3 localHitPoint)
    {
        // 计算命中点在靶子平面上离中心的物理距离
        // 注意：如果你的靶子是竖直放置的，它的平面可能是XZ平面，此时应该用:
        // float distanceInMeters = new Vector2(localHitPoint.x, localHitPoint.z).magnitude;
        float distanceInMeters = new Vector2(localHitPoint.x, localHitPoint.z).magnitude;

        // 遍历预设的得分环带（需要按半径从小到大排序）
        foreach (var ring in scoreRings)
        {
            if (distanceInMeters <= ring.radiusInMeters)
            {
                return ring.score;
            }

        }

        return 0; // 在所有环带之外
    }

    /// <summary>
    /// 根据局部坐标更新UI
    /// </summary>
    public void UpdateUI(Vector3 localHitPoint)
    {
        // 检查冷却时间
        if (Time.time - lastUpdateTime < uiUpdateCooldown)
        {
            return; // 还在冷却中，不执行更新
        }

        // 更新最后更新时间
        lastUpdateTime = Time.time;

        // 1. 将局部坐标归一化到 [-1, 1] 的范围
        //    用局部坐标分量除以总半径即可
        float normalizedX = Mathf.Clamp(localHitPoint.x / totalTargetRadius, -1f, 1f);
        float normalizedY = Mathf.Clamp(localHitPoint.z / totalTargetRadius, -1f, 1f);
        // 注意：同样，如果靶子是XZ平面，这里应该是 localHitPoint.z
        Debug.Log($"Normalized Hit Point: ({normalizedX}, {normalizedY})");

        // 2. 将归一化的坐标映射到UI尺寸
        float uiX = normalizedY * (uiTargetDisplay.rect.width / 2);
        float uiY = normalizedX * (uiTargetDisplay.rect.height / 2);
        Debug.Log($"UI Position: ({uiX}, {uiY})");

        Vector2 anchoredPosition = new Vector2(uiX, uiY);

        // 3. 更新UI元素的位置
        point.rectTransform.anchoredPosition = anchoredPosition;

        // 4. 计算得分
        int score = CalculateScore(localHitPoint);
        currentScore += score;
        scoreText.text = currentScore.ToString();


        // 5. 更新箭矢数量显示
        if (arrowManager != null)
        {
            arrowManager.MarkHit(score);
            arrowsLeftText.text = arrowManager.arrowsLeft.ToString();
        }
    }
}
