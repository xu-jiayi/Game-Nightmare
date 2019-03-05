using UnityEngine;
using System.Collections;

public class PassToBoss : MonoBehaviour {

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            Application.LoadLevel("BossLevel");
        }
    }
}
