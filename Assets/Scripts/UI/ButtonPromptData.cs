using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI {
    [CreateAssetMenu(menuName = "Data/UI/Button Prompt")]
    public class ButtonPromptData : ScriptableObject {
        public List<string> textParts;
        public List<int> iconIDs; // Inserted between the text parts, from left to right. Those are then repeated for each input method
    }
}
