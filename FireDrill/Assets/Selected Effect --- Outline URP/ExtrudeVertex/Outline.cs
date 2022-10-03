using UnityEngine;

namespace SelectedEffectOutline
{
	[RequireComponent(typeof(Renderer))]
	public class Outline : MonoBehaviour
	{
		public enum ETriggerMethod { MouseMove = 0, MouseRightPress, MouseLeftPress };
		[Header("Trigger Method")]
		public ETriggerMethod m_TriggerMethod = ETriggerMethod.MouseMove;
		public bool m_Persistent = false;
		[Header("Outline")]
		public Color m_OutlineColor = Color.green;
		[Range(1f, 10f)] public float m_OutlineWidth = 2f;
		[Range(0f, 1f)] public float m_OutlineFactor = 1f;
		public bool m_WriteZ = false;
		public bool m_BasedOnVertexColorR = false;
		public bool m_OutlineOnly = false;
		[Range(-16f, -1f)] public float m_DepthOffset = -8f;
		public int m_StencilRefValue = 1;
		[Header("Overlay Dash")]
		public bool m_Dash = false;
		public Vector2 m_DashSpeed = new Vector2(0f, 0f);
		public float m_DashFrequency = 0.5f;
		public float m_DashSpaceWidth = 0.2f;
		[Header("Overlay Flash")]
		public Color m_OverlayColor = Color.red;
		[Range(0f, 0.6f)] public float m_Overlay = 0f;
		public bool m_OverlayFlash = false;
		[Range(1f, 6f)] public float m_OverlayFlashSpeed = 3f;
		[Header("Shader")]
		public Shader m_SdrOutlineOnly;
		public Shader m_SdrOutlineStandard;
		Shader m_SdrOriginal;
		Renderer m_Rd;

		public bool onEnable;


		void Start()
		{
			m_Rd = GetComponent<Renderer>();
			m_SdrOriginal = m_Rd.material.shader;
		}
		void Update()
		{
			// material effect parameters
			if (m_OverlayFlash)
			{
				float curve = Mathf.Sin(Time.time * m_OverlayFlashSpeed) * 0.5f + 0.5f;
				m_Overlay = curve * 0.6f;
			}

			SetOutLine();
		}
		public void OutlineEnable()
		{
			Debug.Log("Outline Enabled");
			Material[] mats = m_Rd.materials;
			for (int i = 0; i < mats.Length; i++)
			{
				if (m_OutlineOnly)
					mats[i].shader = m_SdrOutlineOnly;
				else
					mats[i].shader = m_SdrOutlineStandard;
			}
		}
		public void OutlineDisable()
		{
			if (m_Persistent)
				return;
			Debug.Log("Outline Disabled");
			Material[] mats = m_Rd.materials;
			for (int i = 0; i < mats.Length; i++)
				mats[i].shader = m_SdrOriginal;
		}

		public void SetOutLine()
		{
			Material[] mats = m_Rd.materials;
			for (int i = 0; i < mats.Length; i++)
			{
				if (m_Dash)
				{
					mats[i].EnableKeyword("OUTLINE_DASH");
					mats[i].SetVector("_OutlineDashParams", new Vector4(m_DashSpeed.x, m_DashSpeed.y, m_DashFrequency, m_DashSpaceWidth));
				}
				else
				{
					mats[i].DisableKeyword("OUTLINE_DASH");
				}
				mats[i].SetFloat("_OutlineWidth", m_OutlineWidth);
				mats[i].SetColor("_OutlineColor", m_OutlineColor);
				mats[i].SetFloat("_OutlineFactor", m_OutlineFactor);
				mats[i].SetColor("_OverlayColor", m_OverlayColor);
				mats[i].SetFloat("_OutlineWriteZ", m_WriteZ ? 1f : 0f);
				mats[i].SetFloat("_OutlineBasedVertexColorR", m_BasedOnVertexColorR ? 0f : 1f);
				mats[i].SetFloat("_Overlay", m_Overlay);
				mats[i].SetFloat("_DepthOffset", m_DepthOffset);
				mats[i].SetFloat("_RefValue", m_StencilRefValue);
			}
		}
	}
}