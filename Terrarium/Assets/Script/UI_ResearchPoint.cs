using UnityEngine;
using UnityEngine.UI;

public class UI_ResearchPoint : MonoBehaviour
{
    [SerializeField] private Text researchPointText;
    private int lastResearchPoints = 0;
    
    void Start()
    {
        // 如果没有手动分配Text组件，尝试从子对象中找到
        if (researchPointText == null)
        {
            researchPointText = GetComponentInChildren<Text>();
        }
        
        // 初始化显示
        UpdateResearchPointDisplay();
    }
    
    void Update()
    {
        // 检查研究点数是否发生变化
        if (PlantItem.researchPoints != lastResearchPoints)
        {
            UpdateResearchPointDisplay();
            lastResearchPoints = PlantItem.researchPoints;
        }
    }
    
    void UpdateResearchPointDisplay()
    {
        if (researchPointText != null)
        {
            researchPointText.text = "研究点数: " + PlantItem.researchPoints;
        }
        else
        {
            Debug.LogWarning("UI_ResearchPoint: 未找到Text组件！");
        }
    }
}