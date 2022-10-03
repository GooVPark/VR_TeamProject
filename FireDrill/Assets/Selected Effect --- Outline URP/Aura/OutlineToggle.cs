using UnityEngine;

namespace SelectedEffectOutline
{
	[RequireComponent(typeof(Renderer))]
	public class OutlineToggle : MonoBehaviour
	{
		public enum ETriggerMethod { MouseMove = 0, MouseRightPress, MouseLeftPress };
		[Header("Trigger Method")]
		public ETriggerMethod m_TriggerMethod = ETriggerMethod.MouseMove;
		public bool m_Persistent = false;
		Renderer m_Rd;
		bool m_IsMouseOn = false;

		public bool isOn;

		void Start()
		{
			m_Rd = GetComponent<Renderer>();
			Material[] mats = m_Rd.materials;
			for (int i = 0; i < mats.Length; i++)
				mats[i].SetShaderPassEnabled("LightweightForward", false);
		}


        private void Update()
        {
			if (isOn) OutlineEnable();
			else OutlineDisable();
        }

        [ContextMenu("OutLine Enable")]
		public void OutlineEnable()
		{
			Debug.Log("Enabled");
			Material[] mats = m_Rd.materials;
			for (int i = 0; i < mats.Length; i++)
				mats[i].SetShaderPassEnabled("LightweightForward", true);
		}
		public void OutlineDisable()
		{
			Material[] mats = m_Rd.materials;
			for (int i = 0; i < mats.Length; i++)
				mats[i].SetShaderPassEnabled("LightweightForward", false);
		}
		void OnMouseEnter()
		{
			m_IsMouseOn = true;
			if (m_TriggerMethod == ETriggerMethod.MouseMove)
				OutlineEnable();
		}
		void OnMouseExit()
		{
			m_IsMouseOn = false;
			OutlineDisable();
		}
	}
}
