using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_ResearchPoint : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI researchPointText;
    private int lastResearchPoints = 0;
    
    // 添加研究点数变量，替代不存在的ResearchManager
    private static int researchPoints = 0;
    
    // 提供静态访问方法
    public static int ResearchPoints 
    { 
        get { return researchPoints; } 
        set { researchPoints = value; }
    }
    
    void Start()
    {
        // 如果没有手动分配Text组件，尝试从子对象中找到
        if (researchPointText == null)
        {
            researchPointText = GetComponentInChildren<TextMeshProUGUI>();
        }
        
        // 初始化显示
        UpdateResearchPointDisplay();
    }
    
    void Update()
    {
        // 检查研究点数是否发生变化
        if (ResearchPoints != lastResearchPoints)
        {
            UpdateResearchPointDisplay();
            lastResearchPoints = ResearchPoints;
        }
    }
    
    void UpdateResearchPointDisplay()
    {
        if (researchPointText != null)
        {
            researchPointText.text = "研究点数: " + ResearchPoints;
        }
        else
        {
            Debug.LogWarning("UI_ResearchPoint: 未找到TextMeshProUGUI组件！");
        }
    }
    
    // 添加增加研究点数的方法
    public static void AddResearchPoints(int amount)
    {
        ResearchPoints += amount;
    }
}


