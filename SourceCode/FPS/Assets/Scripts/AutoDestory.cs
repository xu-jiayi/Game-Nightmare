using UnityEngine;
using System.Collections;

public class AutoDestory : MonoBehaviour
{

    float m_timer = 1.0f;
    // Update is called once per frame
    void Update()
    {
        m_timer -= Time.deltaTime;
        if (m_timer <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
