using UnityEngine;

namespace Mirror
{
    /// <summary>
    /// Component that will display the clients ping in milliseconds
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Network/NetworkPingDisplay")]
    [HelpURL("https://mirror-networking.com/docs/Articles/Components/NetworkPingDisplay.html")]
    public class NetworkPingDisplay : MonoBehaviour
    {
        public bool showPing = true;
        public bool calculatePing = true;
        [Tooltip("True shows the round trip time, from origin to destination, then back agian. Set to false for time to server only.")]
        public bool showRoundTripTime = true;
        private int rttMultiplier = 1;
        public Vector2 position = new Vector2(200, 0);
        public int fontSize = 24;
        public Color textColor = new Color32(255, 255, 255, 80);
        public string format = "{0}ms";
        public string actualPingFormat = "0ms";
        public int actualPing = 0;

        GUIStyle style;

        void Awake()
        {
            style = new GUIStyle();
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = fontSize;
            style.normal.textColor = textColor;
        }

        void OnGUI()
        {
            if (!calculatePing) { return; }
            if (showRoundTripTime) { rttMultiplier = 1; } else { rttMultiplier = 2; }

            actualPingFormat = string.Format(format,(int)((NetworkTime.rtt / rttMultiplier) * 1000));
            actualPing = (int)((NetworkTime.rtt / rttMultiplier) * 1000);

            // leave here or create special method to update fontSize and textColor
            style.fontSize = fontSize;
            style.normal.textColor = textColor;

            int width = Screen.width;
            int height = Screen.height;
            Rect rect = new Rect(position.x, position.y, width - 200, height * 2 / 100);

            if (showPing) { GUI.Label(rect, actualPingFormat, style); }
        }
    }
}
