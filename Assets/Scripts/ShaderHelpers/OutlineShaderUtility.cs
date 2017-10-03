using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineShaderUtility : MonoBehaviour
{
    public const string OutlineShaderName = "Custom/Outline";
    public const string OutlineColorPropertyName = "_OutlineColor";
    public const string OutlineThicknessPropertyName = "_OutlineThickness";

    //-------------------------------------------------------------------------------------

    public static Material CreateOutlineMaterial(float InitialThickness = 0, Color InitialColor = default(Color))
    {
        if (InitialColor == default(Color)) { InitialColor = Color.white; }

        Shader OutlineShader = Shader.Find(OutlineShaderName);
        if (OutlineShader == null) { return null; }

        Material Result = new Material(OutlineShader);
        if (Result != null)
        {
            SetOutlineColor(Result, InitialColor);
            SetOutlineThickness(Result, InitialThickness);
        }
        return Result;
    }

    #region Setters
    public static void SetOutlineThickness(Material Mat, float Value)
    {
        if (Mat == null) { return; }

        if (Mat.HasProperty(OutlineThicknessPropertyName)) { Mat.SetFloat(OutlineThicknessPropertyName, Value); }
    }
    public static void SetOutlineThickness(Renderer Rend, float Value)
    {
        if (Rend == null || Rend.material == null) { return; }

        SetOutlineThickness(Rend.material, Value);
    }

    public static void SetOutlineColor(Material Mat, Color Value)
    {
        if (Mat == null) { return; }

        if (Mat.HasProperty(OutlineColorPropertyName)) { Mat.SetVector(OutlineColorPropertyName, Value); }
    }
    public static void SetOutlineColor(Renderer Rend, Color Value)
    {
        if (Rend == null) { return; }

        SetOutlineColor(Rend.material, Value);
    }
    #endregion
}
