using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI { 
	public class HealthBar : MonoBehaviour {
		[SerializeField] private GameObject m_heartParent;
		[SerializeField] private GameObject m_heartPrefab;
		[SerializeField] private Sprite heartSprite;
		[SerializeField] private Sprite emptyHeartSprite;
		[SerializeField] private float m_heartSpacing;

		private int lastHearts = -1;
		private Image[] heartRenderers;


		private void Start() {
			int count = 3;
			heartRenderers = new Image[count];
			for (int i = 0; i < count; i++) {
				GameObject heart = Instantiate(m_heartPrefab);
				heart.transform.parent = m_heartParent.transform;
				heart.transform.localPosition = new(i * m_heartSpacing, 0, 0);

				heartRenderers[i] = heart.GetComponent<Image>();
			}
		}
		private void FixedUpdate() {
			RenderHearts();
		}

		private void RenderHearts() {
			if (lastHearts != 3) {
				for (int i = 0; i < 3; i++) {
					heartRenderers[i].sprite = i < 2? heartSprite : emptyHeartSprite;
				}
				lastHearts = 3;
			}
		}
	}
}
