#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("2R6sJ2XF8u6LNevZpBaNhEeW4Nb4pWHVcLUp/x/Fr2YbLUU3K30FNb/FUETWbLfRnXQK2l6P6lRy3YArAaLwJuKspJpTdyIYdkwtBnvUjcOWFRsUJJYVHhaWFRUUsFfgEu+S9nKYsx1s4ozzv+jac0jhVi+2MnJQC9ajwOSfjAivc0Xm2xny3qvhbyptWm6qopRYUZZUFvGLC7jj+UHP9iSWFTYkGRIdPpJckuMZFRUVERQXc5M9zuJVnzlhxMmk6Q/HWTURXpHMzL4KrNwK/qKjjAAq1Bs91K4lQgQ8yq+TBlLZBQKQQ7j7ZNSjQD8hwvBA0pxOs36YF/sJPkNYvOWD0yuPon1XhudamUTm2XRK+CEFgSUpCYgAoIz9qDBXURYXFRQV");
        private static int[] order = new int[] { 4,11,8,8,11,12,12,8,11,13,13,11,12,13,14 };
        private static int key = 20;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
