using Assets.Scripts.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialEventTestScript : MonoBehaviour
{
    [SerializeField] private GameObject objectToChangeMaterialOf;
    [SerializeField] private Material[] materials;

    private int index = 0;
    Maid maid = new Maid();

    // Start is called before the first frame update
    void Start()
    {
        SetMaterial(this.index);
    }

    void OnEnable()
    {
        maid.GiveTask(DialogueSystem.BindToEvent("ChangeMaterial", ChangeMaterial));
    }

    void OnDisable()
    {
        maid.Cleanup();
    }

    void ChangeMaterial()
    {
        this.index++;
        if (this.index >= this.materials.Length)
        {
            this.index = 0;
        }

        SetMaterial(this.index);
    }

    private void SetMaterial(int index)
    {
        this.objectToChangeMaterialOf.GetComponent<SkinnedMeshRenderer>().material
            = materials[index];
    }
}
