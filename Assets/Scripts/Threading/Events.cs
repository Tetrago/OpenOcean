using System.Threading;
using UnityEngine;

namespace Threading
{
    public sealed partial class Events : MonoBehaviour
    {
        public const string NAME = "Threading.Events";

        private static Events instance_;

        public static Events Instance
        {
            get
            {
                if(instance_ == null)
                    instance_ = new GameObject(NAME).AddComponent<Events>();
                return instance_;
            }
        }
    }
}
