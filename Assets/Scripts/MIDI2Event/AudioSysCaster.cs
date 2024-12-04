using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSysCaster : MonoBehaviour
{
    LayerMask mask;

    SongEnum last;

    private void Start()
    {
        mask = (1 << 7);
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.up, out hit, Mathf.Infinity, mask))
        {
            SongIDHolder s = hit.collider.gameObject.GetComponent<SongIDHolder>();
            if (s is not null && s.ID != last)
            {
                Debug.Log(s.ID);
                Debug.Log(hit.collider.gameObject.name);
                AudioSystem.SwitchToSong(s.ID);
                last = s.ID;
            }
        }
    }
}
