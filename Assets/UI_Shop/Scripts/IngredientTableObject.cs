using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ingredient", menuName = "IngredientsData", order = 1)]
public class IngredientTableObject : ScriptableObject
{
    public string nameIngredient
    {
        get
        {
            string[] splitName =  iconIngredient.name.Split('_');
            string name = splitName[1].ToUpper();
            return name;
        }
    }

    public Sprite iconIngredient;
}
