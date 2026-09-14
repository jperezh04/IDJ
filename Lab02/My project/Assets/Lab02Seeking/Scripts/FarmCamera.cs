using UnityEngine;

namespace Lab02Seeking
{
    [RequireComponent(typeof(Camera))]
    public class FarmCamera : MonoBehaviour
    {
        public enum ViewMode { General, Cenital, Seguir }
        public Transform target;
        public ViewMode view = ViewMode.General;
        private Camera lens;

        private void Awake() { lens = GetComponent<Camera>(); }
        private void Start() { SetView(view); }

        public void SetView(ViewMode value)
        {
            if (lens == null) lens = GetComponent<Camera>();
            view = value;
            lens.orthographic = view != ViewMode.Seguir;
            lens.orthographicSize = view == ViewMode.Cenital ? 14.5f : 17f;
            lens.fieldOfView = 55f;
            ApplyView(true);
        }

        private void LateUpdate() { ApplyView(false); }

        private void ApplyView(bool instant)
        {
            if (view == ViewMode.General)
            {
                transform.position = new Vector3(0, 23, 26);
                transform.LookAt(Vector3.zero);
            }
            else if (view == ViewMode.Cenital)
            {
                transform.position = new Vector3(0, 30, 0);
                transform.rotation = Quaternion.Euler(90, 0, 0);
            }
            else if (target != null)
            {
                Vector3 focus = target.position + Vector3.up;
                Vector3 position = focus + new Vector3(0, 8, 10);
                transform.position = instant ? position : Vector3.Lerp(transform.position, position,
                    1f - Mathf.Exp(-8f * Time.deltaTime));
                transform.LookAt(focus);
            }
        }
    }
}
