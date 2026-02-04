using UnityEngine;

public class MonoBehaviourHelper : MonoBehaviour
{
    private static MonoBehaviourHelper _instance;

    public static MonoBehaviourHelper Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("MonoBehaviourHelper");
                _instance = go.AddComponent<MonoBehaviourHelper>();
                Object.DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }
}
