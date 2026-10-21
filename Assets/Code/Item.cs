using UnityEngine;

abstract public class Item : MonoBehaviour
{
    public abstract bool CanPick();

    public virtual void Pick()
    {
        gameObject.SetActive(false);
    }
}
