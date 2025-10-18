using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// XRIDefaultInputActions 的单例管理器
/// 提供全局访问点，无需手动实例化
/// </summary>
public class InputActionsManager : MonoBehaviour
{
    // 单例实例
    private static InputActionsManager _instance;

    // 静态 XRIDefaultInputActions 实例
    private static XRIDefaultInputActions _inputActions;

    /// <summary>
    /// 获取单例实例
    /// </summary>
    public static InputActionsManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<InputActionsManager>();

                if (_instance == null)
                {
                    GameObject managerObject = new GameObject("InputActionsManager");
                    _instance = managerObject.AddComponent<InputActionsManager>();
                }
            }

            return _instance;
        }
    }

    /// <summary>
    /// 获取 XRIDefaultInputActions 实例
    /// </summary>
    public static XRIDefaultInputActions Actions
    {
        get
        {
            if (_inputActions == null)
            {
                _inputActions = new XRIDefaultInputActions();
            }
            return _inputActions;
        }
    }

    /// <summary>
    /// 获取底层的 InputActionAsset
    /// </summary>
    public static InputActionAsset Asset => Actions.asset;

    // 确保只有一个实例存在
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        // 初始化 InputActions（如果静态实例还未创建）
        if (_inputActions == null)
        {
            _inputActions = new XRIDefaultInputActions();
        }
    }

    private void OnEnable()
    {
        _inputActions?.Enable();
    }

    private void OnDisable()
    {
        _inputActions?.Disable();
    }

    private void OnDestroy()
    {
        // 如果是单例实例被销毁，清理资源
        if (_instance == this)
        {
            _inputActions?.Dispose();
            _inputActions = null;
            _instance = null;
        }
    }

    /// <summary>
    /// 查找指定名称的 InputAction
    /// </summary>
    /// <param name="actionName">Action 名称</param>
    /// <param name="throwIfNotFound">如果找不到是否抛出异常</param>
    /// <returns>找到的 InputAction，如果找不到且 throwIfNotFound 为 false 则返回 null</returns>
    public static InputAction FindAction(string actionName, bool throwIfNotFound = false)
    {
        return Asset.FindAction(actionName, throwIfNotFound);
    }

    /// <summary>
    /// 启用所有 InputActions
    /// </summary>
    public static void EnableAll()
    {
        Actions.Enable();
    }

    /// <summary>
    /// 禁用所有 InputActions
    /// </summary>
    public static void DisableAll()
    {
        Actions.Disable();
    }
}

