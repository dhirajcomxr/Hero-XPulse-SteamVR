using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// [System.Serializable]
// public class MatDetails
// {
//     public string matName;
//     public Material mat;
// }
// [System.Serializable]
// public class BikeVariantColors
// {
//     public string veriantName;
//     public MatDetails[] matDerails;

// }
public class MainBikeColorVariant : MonoBehaviour
{
    public BikeVariantColors[] bikeVariants;
    public MeshRenderer[] Meshes;

    [SerializeField] MatDetails[] materials;
    string veriantName = "polestar blue";

    private void OnEnable()
    {
       /* materialContainer = FindObjectOfType<MaterialContainer>();
        bikeVariants = materialContainer.bikeVariants;
        veriantName = materialContainer.veriantName;*/
        GetMaterialData(veriantName);
        SetMaterialData();
    }

    public void GetMaterialData(string veriantName)
    {
        foreach (BikeVariantColors item in bikeVariants)
        {
            if (item.veriantName == veriantName)
            {
                materials = item.matDerails;
            }
        }
    }
    public void SetMaterialData()
    {
        foreach (MeshRenderer item in Meshes)
        {
            Material[] renderMaterial = item.materials;
            for (int i = 0; i < renderMaterial.Length; i++)
            {
                for (int j = 0; j < materials.Length; j++)
                {
                    if (renderMaterial[i].name.Contains(materials[j].matName))
                    {
                        renderMaterial[i] = materials[j].mat;
                        break;
                    }
                }
            }
            item.materials = renderMaterial;
        }
    }
}
