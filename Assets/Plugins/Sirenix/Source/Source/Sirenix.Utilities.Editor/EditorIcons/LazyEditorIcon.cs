#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="LazyEditorIcon.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System.Globalization;

namespace Sirenix.Utilities.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Lazy loading Editor Icon.
    /// </summary>
    [InitializeOnLoad]
    public class LazyEditorIcon : EditorIcon
    {
        private static readonly string iconShader = @"
Shader ""Hidden/Sirenix/Editor/GUIIcon""
{
	Properties
	{
        _MainTex(""Texture"", 2D) = ""white"" {}
        _Color(""Color"", Color) = (1,1,1,1)
        _Rect(""Rect"", Vector) = (0,0,0,0)
        _TexelSize(""TexelSize"", Vector) = (0,0,0,0)
	}
    SubShader
	{
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
                " + /*Blame the old Unity compiler*/ "#" + @"pragma vertex vert
                " + "#" + @"pragma fragment frag
                " + "#" + @"include ""UnityCG.cginc""

                struct appdata
                {
                    float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

                struct v2f
                {
                    float2 uv : TEXCOORD0;
					float4 vertex : SV_POSITION;
				};

                sampler2D _MainTex;
                float4 _Rect;
                float4 _Color;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
				{
					fixed4 col = _Color;
                    float2 uv = i.uv;
                    uv *= _Rect.zw;
					uv += _Rect.xy;
					col.a *= tex2D(_MainTex, uv).a;
					return col;
				}
			ENDCG
		}
	}
}
";

        private static Color highlightedColor = new Color(0f, 0f, 0f, 0.8f);
        private static Color highlightedColorPro = new Color(1f, 1f, 1f, 1f);
        private static Color inactiveColor = new Color(0f, 0f, 0f, 0.57f);
        private static Color inactiveColorPro = new Color(0.6f, 0.6f, 0.6f, 1f);
        private static Color activeColor = new Color(0f, 0f, 0f, 0.6f);
        private static Color activeColorPro = new Color(0.6f, 0.6f, 0.6f, 1f);

        private static Material iconMat;

        private Sprite icon;
        private Texture highlighted;
        private Texture active;
        private Texture inactive;
        private string iconName;

        /// <summary>
        /// Loads an EditorIcon from the spritesheet.
        /// </summary>
        public LazyEditorIcon(string icon)
        {
            this.iconName = icon;
            ReloadIcon();
        }

        private void ReloadIcon()
        {
            this.icon = AssetDatabase.LoadAllAssetsAtPath("Assets/" + SirenixAssetPaths.SirenixAssetsPath + "Editor/Icons.png").OfType<Sprite>().First(x => x.name.ToLower() == this.iconName);
            this.highlighted = this.RenderIcon(EditorGUIUtility.isProSkin ? highlightedColorPro : highlightedColor);
            this.active = this.RenderIcon(EditorGUIUtility.isProSkin ? activeColorPro : activeColor);
            this.inactive = this.RenderIcon(EditorGUIUtility.isProSkin ? inactiveColorPro : inactiveColor);
        }

        /// <summary>
        /// Gets the icon's highlight texture.
        /// </summary>
        public override Texture Highlighted
        {
            get
            {
                if (this.highlighted == null)
                {
                    this.ReloadIcon();
                }
                return this.highlighted;
            }
        }

        /// <summary>
        /// Gets the icon's active texture.
        /// </summary>
        public override Texture Active
        {
            get
            {
                if (this.active == null)
                {
                    this.ReloadIcon();
                }
                return this.active;
            }
        }

        /// <summary>
        /// Gets the icon's inactive texture.
        /// </summary>
        public override Texture Inactive
        {
            get
            {
                if (this.inactive == null)
                {
                    this.ReloadIcon();
                }
                return this.inactive;
            }
        }

        private Texture RenderIcon(Color color)
        {
            var rect = this.icon.rect;

            if (iconMat == null || iconMat.shader == null)
            {
                iconMat = new Material(ShaderUtil.CreateShaderAsset(iconShader));
            }

            iconMat.SetColor("_Color", color);
            iconMat.SetVector("_TexelSize", new Vector2(1f / this.icon.texture.width, 1f / this.icon.texture.height));
            iconMat.SetVector("_Rect", new Vector4(
                rect.x / this.icon.texture.width,
                rect.y / this.icon.texture.height,
                rect.width / this.icon.texture.width,
                rect.height / this.icon.texture.height
            ));

            RenderTexture prev = RenderTexture.active;
            var rt = new RenderTexture((int)rect.width, (int)rect.height, 0);
            rt.Create();
            RenderTexture.active = rt;
            GL.Clear(false, true, new Color(0, 0, 0, 0));
            Graphics.Blit(this.icon.texture, rt, iconMat);
            RenderTexture.active = prev;
            return rt;
        }

        /// <summary>
        /// Gets the name of the icon.
        /// </summary>
        public override string ToString()
        {
            return this.iconName.ToString(CultureInfo.InvariantCulture);
        }
    }
}
#endif