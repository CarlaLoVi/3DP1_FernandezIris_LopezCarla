using UnityEngine;

public class TCDebugTimeScaler
{
public KeyCode m_AcceleratorKeyCode = KeyCode.RightControl;
public KeyCode m_DeceleratorKeyCode = KeyCode.RightShift;
#if UNITY_EDITOR
private void Update()
{
    if (Input.GetKey(m_AcceleratorKeyCode))
    {
        if (Time.timeScale == 1.0f)
        {
            Time.timeScale = 10.0f;
        }
        else
        {
            Time.timeScale = 1.0f;
        }

    }
    else if (Input.GetKeyDown(m_DeceleratorKeyCode))
    {
        if (Time.timeScale == 1.0f)
        {
            Time.timeScale = 0.1f;
        }
        else
        {
            Time.timeScale = 1.0f;
        }
    }
}
#endif
}