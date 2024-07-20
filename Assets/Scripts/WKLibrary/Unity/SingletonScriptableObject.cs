using UnityEngine;

namespace WK
{
namespace Unity
{
    public class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject
    {
        private static volatile T instance;

        public static T Instance
        {
            get
            {
#if UNITY_EDITOR
                if (instance == null)
                {
                    if(Application.isPlaying)
                    {
                        Debug.LogAssertionFormat(
                            "Warning: Database({0}) isn't loaded in hierarchy. This cannot work in out of UnityEditor",
                            typeof(T).Name);
                    }
                    instance = UnityEditor.AssetDatabase.LoadAssetAtPath<T>("Assets/ScriptableObjects/" + typeof(T).Name + ".asset");
                }
#endif
                Debug.Assert(instance!=null,"instance is null!!!!");
                return instance;
            }
        }

        void OnDestroy()
        {
            instance = null;
        }

        // コンストラクタをprotectedにすることでインスタンスを生成出来なくする
        protected SingletonScriptableObject()
        {
            //Debug.LogFormat("Databse:{0} ready", typeof(T).Name);
            instance = this as T;
        }
    }

}
}
