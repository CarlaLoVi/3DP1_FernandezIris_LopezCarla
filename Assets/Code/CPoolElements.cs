using System.Collections.Generic;
using UnityEngine;

public class CPoolElements
{
    List<GameObject> m_Elements;
    int m_CurrentElementId =- 1;

    public CPoolElements (int ElementsCount, GameObject ElementPrefab)
    {
        m_Elements = new List<GameObject>();
        for (int i =0; i <ElementsCount; i++)
        {
            GameObject l_GameObject = GameObject.Instantiate(ElementPrefab);
            l_GameObject.gameObject.SetActive(false);
            m_Elements.Add(l_GameObject);
        }
    }

    public GameObject GetNextElement()
    {
        m_CurrentElementId = (m_CurrentElementId + 1) % m_Elements.Count;
        return m_Elements[m_CurrentElementId];
    }
}
