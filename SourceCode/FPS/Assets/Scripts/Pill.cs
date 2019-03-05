using UnityEngine;
using System.Collections;

public class Pill : MonoBehaviour
{

    Transform m_transform;
    PillSpawn m_spawn;

    // Use this for initialization
    void Start()
    {
        m_transform = this.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.pause) return;
        m_transform.Rotate(new Vector3(0, Time.deltaTime * 150, 0));
    }

    void OnTriggerEnter(Collider col)
    {
        if (GameManager.instance.pause) return;
        if (col.gameObject.tag == "Player")
        {
            col.gameObject.SendMessage("CellPickUp");
            m_spawn.m_pillCount--;
            Destroy(this.gameObject);
        }
    }

    public void Init(PillSpawn spawn)
    {
        m_spawn = spawn;
        m_spawn.m_pillCount++;
    }
}
