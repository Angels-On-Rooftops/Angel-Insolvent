using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSysCaster : MonoBehaviour
{
    LayerMask mask;

    SongEnum last = SongEnum.None;

    private void Start()
    {
        mask = (1 << 7);
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Debug.DrawLine(transform.position, transform.position + Vector3.up * 2000);
        if (Physics.Raycast(transform.position, Vector3.up, out hit, Mathf.Infinity, mask))
        {
            SongIDHolder s = hit.collider.gameObject.GetComponent<SongIDHolder>();
            if (s != null && s.ID != last && AudioSystem.Instance.HasSong(s.ID))
            {
                Debug.Log(s.ID);
                Debug.Log(hit.collider.gameObject.name);
                AudioSystem.SwitchToSong(s.ID);
                last = s.ID;
            }
        }
    }
}
