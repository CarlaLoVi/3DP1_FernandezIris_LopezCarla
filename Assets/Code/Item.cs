using UnityEngine;

abstract public class Item : MonoBehaviour
{
    public abstract bool CanPick();

    public virtual void Pick()
    {
        gameObject.SetActive(false);
    }
    protected FPSController GetPlayer()
    {
        GameObject l_Player = GameObject.FindGameObjectWithTag("Player");

        FPSController l_Controller = null;

        if (l_Player != null)
        {
            l_Controller = l_Player.GetComponent<FPSController>();

            if (l_Controller == null)
                l_Controller = l_Player.GetComponentInChildren<FPSController>();
        }

        if (l_Controller == null)
            l_Controller = FindAnyObjectByType<FPSController>();

        return l_Controller;
    }
}
