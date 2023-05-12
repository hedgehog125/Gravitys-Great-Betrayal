using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI { 
	public class HealthBar : MonoBehaviour {
		[SerializeField] private GameObject m_heartPrefab;
		[SerializeField] private Sprite heartSprite;
		[SerializeField] private Sprite emptyHeartSprite;
		[SerializeField] private float m_heartSpacing;

		private int maxHearts;
		private int lastHearts = -1;
		private Image[] heartRenderers;


		public void Init(int maxHearts) {
			this.maxHearts = maxHearts;

			heartRenderers = new Image[maxHearts];
			for (int i = 0; i < maxHearts; i++) {
				GameObject heart = Instantiate(m_heartPrefab, transform);
				RectTransform rect = heart.GetComponent<RectTransform>();

				heart.transform.Translate(i * m_heartSpacing, 0, 0);

				heartRenderers[i] = heart.GetComponent<Image>();
			}
			SetHearts(0);
		}

		public void SetHearts(int value) {
			if (lastHearts != value) {
				RenderHearts(value);
				lastHearts = value;
			}
		}

		private void RenderHearts(int value) {
			for (int i = 0; i < maxHearts; i++) {
				heartRenderers[i].sprite = i < value? heartSprite : emptyHeartSprite;
			}
			
		}
	}
}
