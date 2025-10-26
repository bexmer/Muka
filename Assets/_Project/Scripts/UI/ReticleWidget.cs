using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    public class ReticleWidget : MonoBehaviour
    {
        public Image image;
        public Color normalColor = new Color(1, 1, 1, 0.7f);
        public Color hoverColor = new Color(1, 1, 1, 1f);
        public float scaleNormal = 1f;
        public float scaleHover = 1.2f;
        public float lerp = 15f;

        bool _isHover;

        void Awake()
        {
            if (!image) image = GetComponent<Image>();
            SetImmediate(false);
        }

        void Update()
        {
            if (!image) return;
            image.color = Color.Lerp(image.color, _isHover ? hoverColor : normalColor, Time.deltaTime * lerp);
            float target = _isHover ? scaleHover : scaleNormal;
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * target, Time.deltaTime * lerp);
        }

        public void SetHover(bool v) { _isHover = v; }
        public void SetImmediate(bool v)
        {
            _isHover = v;
            if (image)
            {
                image.color = v ? hoverColor : normalColor;
                transform.localScale = Vector3.one * (v ? scaleHover : scaleNormal);
            }
        }
    }
}
