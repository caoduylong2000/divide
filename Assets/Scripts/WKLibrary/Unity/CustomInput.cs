using UnityEngine;

namespace WK { namespace Unity {
    public static class CustomInput {
        public static bool IsBackKeyUp()
        {
            return Input.GetKeyUp(KeyCode.Escape);
            /*
            if (Application.platform == RuntimePlatform.Android)
            {
                return Input.GetKeyUp(KeyCode.Escape);
            }
            return false;
            */
        }
    }
}}
