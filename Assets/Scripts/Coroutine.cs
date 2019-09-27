using UnityEngine;

public class Coroutine : MonoBehaviour
{
    public const string NAME = "Coroutine";

    private static Coroutine instance_ = null;

    public static Coroutine Instance
    {
        get
        {
            if(instance_ == null)
                instance_ = new GameObject(NAME).AddComponent<Coroutine>();
            return instance_;
        }
    }
}
