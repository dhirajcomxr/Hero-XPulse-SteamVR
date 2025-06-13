using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MatDetails{
    public string matName;
    public Material mat;
}
[System.Serializable]
public class BikeVariantColors{
    public string veriantName;
    public MatDetails[] matDerails;

}
public class BikeMatTexChange : MonoBehaviour
{
    public BikeVariantColors[] bikeVariants;
    public MeshRenderer[] Meshes;
    //public MaterialContainer materialContainer;
    
    
    MatDetails[] materials;
    /*
        public BikeVariant[] colorVariants;
        BikeVariant SelectedColorVariants;
        MaterialContainer mat;
        private void Start() {

            mat = FindObjectOfType<MaterialContainer>();
            if (mat) {
                colorVariants = mat.colorVariants;
            }
        }

        public void changeBikeColor(string colorName)
        {

            foreach (BikeVariant bikeVariant in colorVariants)
            {
                if (bikeVariant.colorName == colorName)
                {
                    SelectedColorVariants = bikeVariant;
                }
            }

            for (int i = 0; i < SelectedColorVariants.matsAndTex.Length; i++)
            {
                SelectedColorVariants.matsAndTex[i].mat.mainTexture = SelectedColorVariants.matsAndTex[i].tex;
            }
            for (int i = 0; i < SelectedColorVariants.materialsForColorChange.Length; i++)
            {
                SelectedColorVariants.materialsForColorChange[i].color = SelectedColorVariants.colorToApply;
            }
        }*/

    private void Start() {
       /* materialContainer = FindObjectOfType<MaterialContainer>();
        bikeVariants = materialContainer.bikeVariants;
        changeMaterials("Matte Nexus Blue");*/
    }
    public void changeMaterials(string veriantName) {
        foreach (BikeVariantColors item in bikeVariants) {
            if(item.veriantName == veriantName) {
                materials = item.matDerails;
            }
        }

        foreach (MeshRenderer item in Meshes) {
            Material[] renderMaterial = item.materials;
            for (int i = 0; i < renderMaterial.Length; i++) {
                for (int j = 0; j < materials.Length; j++) {
                    if (renderMaterial[i].name.Contains(materials[j].matName)) {
                        renderMaterial[i] = materials[j].mat;
                        break;
                    }
                }
            }
            item.materials = renderMaterial;
        }


    
    }






}
