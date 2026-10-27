using UnityEngine;

public class ShootingGalleryTarget : MonoBehaviour
{
    public int m_Points = 10;
    bool m_Hit = false;

    public void Hit()
    {
        if (m_Hit)
        {
            return;
        }

        m_Hit = true;

        ShootingGallery l_Gallery = FindAnyObjectByType<ShootingGallery>();

        if (l_Gallery != null)
        {
            l_Gallery.AddPoints(m_Points);
        }

        gameObject.SetActive(false);

    }

    public void ResetTarget()
    {
        m_Hit = false;
    }
}
