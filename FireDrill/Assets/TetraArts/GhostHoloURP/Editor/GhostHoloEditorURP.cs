using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GhostHoloEditorURP : ShaderGUI
{


    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {


        Diffuse(materialEditor, properties);
        GhostSettings(materialEditor, properties);
        HoloSettings(materialEditor, properties);
        

    }
    void Diffuse(MaterialEditor materialEditor, MaterialProperty[] properties)
    {

        MaterialProperty MainTexture = ShaderGUI.FindProperty("_MainTexture", properties);
        MaterialProperty BaseColor = ShaderGUI.FindProperty("_BaseColor", properties);
        
        GUILayout.Label("MAIN COLOR AND TEXTURE", EditorStyles.boldLabel);

        materialEditor.ShaderProperty(BaseColor, BaseColor.displayName);
        materialEditor.ShaderProperty(MainTexture, MainTexture.displayName);
        
        GUILayout.Label("_____________________________________________________________", EditorStyles.boldLabel);
    }

    void GhostSettings(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        MaterialProperty Power = ShaderGUI.FindProperty("_Power", properties);
        MaterialProperty Bias = ShaderGUI.FindProperty("_Bias", properties);
        MaterialProperty Scale = ShaderGUI.FindProperty("_Scale", properties);
        MaterialProperty GradientPos = ShaderGUI.FindProperty("_GradientPos", properties);
        MaterialProperty GradientSize = ShaderGUI.FindProperty("_GradientSize", properties);
        MaterialProperty GradientRotation = ShaderGUI.FindProperty("_GradientRotation", properties);

        GUILayout.Label("GHOST SETTINGS", EditorStyles.boldLabel);

        materialEditor.ShaderProperty(Power, Power.displayName);
        materialEditor.ShaderProperty(Bias, Bias.displayName);
        materialEditor.ShaderProperty(Scale, Scale.displayName);
        materialEditor.ShaderProperty(GradientPos, GradientPos.displayName);
        materialEditor.ShaderProperty(GradientSize, GradientSize.displayName);
        materialEditor.ShaderProperty(GradientRotation, GradientRotation.displayName);

        GUILayout.Label("_____________________________________________________________", EditorStyles.boldLabel);
    }

    void HoloSettings(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        MaterialProperty HoloEffect = ShaderGUI.FindProperty("_HoloEffect", properties);
        MaterialProperty HoloTexture = ShaderGUI.FindProperty("_HoloTexture", properties);
        MaterialProperty HoloScale = ShaderGUI.FindProperty("_HoloScale", properties);
        MaterialProperty HoloSpeedX = ShaderGUI.FindProperty("_HoloSpeedX", properties);
        MaterialProperty HoloSpeedY = ShaderGUI.FindProperty("_HoloSpeedY", properties);
        MaterialProperty Glitch = ShaderGUI.FindProperty("_Glitch", properties);
        MaterialProperty GlitchStrength = ShaderGUI.FindProperty("_GlitchStrength", properties);
        MaterialProperty GlitchSeed = ShaderGUI.FindProperty("_GlitchSeed", properties);
        

        GUILayout.Label("HOLO SETTINGS", EditorStyles.boldLabel);

        materialEditor.ShaderProperty(HoloEffect, HoloEffect.displayName);

        if (HoloEffect.floatValue == 1)
        {

            materialEditor.ShaderProperty(HoloTexture, HoloTexture.displayName);
            materialEditor.ShaderProperty(HoloScale, HoloScale.displayName);
            materialEditor.ShaderProperty(HoloSpeedX, HoloSpeedX.displayName);
            materialEditor.ShaderProperty(HoloSpeedY, HoloSpeedY.displayName);
            materialEditor.ShaderProperty(Glitch, Glitch.displayName);
            if (Glitch.floatValue == 1)
            {
                materialEditor.ShaderProperty(GlitchStrength, GlitchStrength.displayName);
                materialEditor.ShaderProperty(GlitchSeed, GlitchSeed.displayName);
                
            }
        }
        GUILayout.Label("_____________________________________________________________", EditorStyles.boldLabel);
    }

   

    public static class FxStyles
    {
        public static GUIStyle header;
        public static GUIStyle headerCheckbox;
        public static GUIStyle headerFoldout;
        public static GUIStyle headerTab;
        public static GUIStyle labelStyle;
        public static GUIStyle HeaderTexture;
        public static GUIContent textureLabel;
        public static GUIContent textureLabel2;
        public static GUIStyle colorPicker;
        public static GUIStyle topIMG;


        static FxStyles()
        {
            // Tab header
            header = new GUIStyle("ShurikenModuleTitle");
            header.font = (new GUIStyle("Label")).font;
            header.border = new RectOffset(15, 7, 4, 4);
            header.fixedHeight = 24;
            header.contentOffset = new Vector2(20f, -2f);
            header.alignment = TextAnchor.MiddleCenter;
            header.fontSize = 12;
            header.fontStyle = FontStyle.Bold;

            // Tab header checkbox
            headerCheckbox = new GUIStyle("ShurikenCheckMark");
            headerFoldout = new GUIStyle("Foldout");

            labelStyle = new GUIStyle(EditorStyles.label);
            //labelStyle.fontStyle = FontStyle.Bold;
            labelStyle.alignment = TextAnchor.MiddleLeft;
            labelStyle.fontSize = 11;
            labelStyle.normal.textColor = new Color32(0, 0, 0, 255);
            //labelStyle.stretchWidth = 10;

            HeaderTexture = new GUIStyle(EditorStyles.label);
            HeaderTexture.alignment = TextAnchor.MiddleCenter;



            textureLabel = new GUIContent();
            textureLabel2 = new GUIContent();

            colorPicker = new GUIStyle(EditorStyles.colorField);
            colorPicker.fixedWidth = 85;

            topIMG = new GUIStyle();
            topIMG.alignment = TextAnchor.MiddleCenter;
        }

        public static bool Header(string title, bool foldout, Color color)
        {
            var rect = GUILayoutUtility.GetRect(16f, 22f, FxStyles.header);
            var auxColor = GUI.color;
            GUI.color = color;
            UnityEngine.GUI.Box(rect, title, FxStyles.header);
            GUI.color = auxColor;

            var foldoutRect = new Rect(rect.x + 4f, rect.y + 2f, 13f, 13f);
            var e = Event.current;



            return foldout;
        }

        public static bool Header(string title, bool foldout, SerializedProperty enabledField, Color color)
        {
            var enabled = enabledField.boolValue;

            var rect = GUILayoutUtility.GetRect(16f, 22f, FxStyles.header);
            var auxColor = GUI.color;
            GUI.color = color;
            UnityEngine.GUI.Box(rect, title, FxStyles.header);
            GUI.color = auxColor;

            var toggleRect = new Rect(rect.x + 4f, rect.y + 4f, 13f, 13f);
            var e = Event.current;

            if (e.type == EventType.Repaint) FxStyles.headerCheckbox.Draw(toggleRect, false, false, enabled, false);

            if (e.type == EventType.MouseDown)
            {
                const float kOffset = 2f;
                toggleRect.x -= kOffset;
                toggleRect.y -= kOffset;
                toggleRect.width += kOffset * 2f;
                toggleRect.height += kOffset * 2f;

                if (toggleRect.Contains(e.mousePosition))
                {
                    enabledField.boolValue = !enabledField.boolValue;
                    e.Use();
                }
                else if (rect.Contains(e.mousePosition))
                {
                    foldout = !foldout;
                    e.Use();
                }
            }

            return foldout;
        }

        public static bool Header(string title, bool foldout, MaterialProperty enabledField, Color color)
        {
            var enabled = (enabledField.floatValue == 1);

            var rect = GUILayoutUtility.GetRect(16f, 22f, FxStyles.header);
            var auxColor = GUI.color;
            GUI.color = color;
            UnityEngine.GUI.Box(rect, title, FxStyles.header);
            GUI.color = auxColor;

            var toggleRect = new Rect(rect.x + 4f, rect.y + 4f, 13f, 13f);
            var e = Event.current;

            if (e.type == EventType.Repaint) FxStyles.headerCheckbox.Draw(toggleRect, false, false, enabled, false);

            if (e.type == EventType.MouseDown)
            {
                const float kOffset = 2f;
                toggleRect.x -= kOffset;
                toggleRect.y -= kOffset;
                toggleRect.width += kOffset * 2f;
                toggleRect.height += kOffset * 2f;

                if (toggleRect.Contains(e.mousePosition))
                {
                    enabledField.floatValue = (enabledField.floatValue == 0) ? 1 : 0;
                    e.Use();
                }

            }

            return foldout;
        }
    }
}


