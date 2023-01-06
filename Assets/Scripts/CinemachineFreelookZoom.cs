using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace Cinemachine
{

    [RequireComponent(typeof(CinemachineFreeLook))]
public class CinemachineFreelookZoom : MonoBehaviour
{
        private CinemachineFreeLook freelook;
        private CinemachineFreeLook.Orbit[] originalOrbits;
        [Tooltip("The minimum scale for the orbits")]
        [Range(0.01f, 1f)]
        public float minScale = 0.5f;

        [Tooltip("The maximum scale for the orbits")]
        [Range(1F, 5f)]
        public float maxScale = 1;

        [Tooltip("The zoom axis.  Value is 0..1.  How much to scale the orbits")]
        [AxisStateProperty]
        public AxisState zAxis = new AxisState(0, 1, false, true, 50f, 0.1f, 0.1f, "Mouse ScrollWheel", false);

        void OnValidate()
        {
            minScale = Mathf.Max(0.01f, minScale);
            maxScale = Mathf.Max(minScale, maxScale);
        }

        private void Start()
        {
            CinemachineCore.GetInputAxis = GetAxisCustom;
        }

        public float GetAxisCustom(string axisName)
        {
            if (axisName == "Horizontal")
            {
                if (Input.GetMouseButton(2))
                {
                    return UnityEngine.Input.GetAxis("Mouse X");
                }
                else
                {
                    return UnityEngine.Input.GetAxis("Horizontal");
                }
            }
            else if (axisName == "Vertical")
            {
                if (Input.GetMouseButton(2))
                {
                    return UnityEngine.Input.GetAxis("Mouse Y");
                }
                else
                {
                    return UnityEngine.Input.GetAxis("Vertical");
                }
            }
            return UnityEngine.Input.GetAxis(axisName);
        }

        void Awake()
        {
            freelook = GetComponentInChildren<CinemachineFreeLook>();
            if (freelook != null)
            {
                originalOrbits = new CinemachineFreeLook.Orbit[freelook.m_Orbits.Length];
                for (int i = 0; i < originalOrbits.Length; i++)
                {
                    originalOrbits[i].m_Height = freelook.m_Orbits[i].m_Height;
                    originalOrbits[i].m_Radius = freelook.m_Orbits[i].m_Radius;
                }
#if UNITY_EDITOR
                SaveDuringPlay.SaveDuringPlay.OnHotSave -= RestoreOriginalOrbits;
                SaveDuringPlay.SaveDuringPlay.OnHotSave += RestoreOriginalOrbits;
#endif
            }
        }

#if UNITY_EDITOR
        private void OnDestroy()
        {
            SaveDuringPlay.SaveDuringPlay.OnHotSave -= RestoreOriginalOrbits;
        }

        private void RestoreOriginalOrbits()
        {
            if (originalOrbits != null)
            {
                for (int i = 0; i < originalOrbits.Length; i++)
                {
                    freelook.m_Orbits[i].m_Height = originalOrbits[i].m_Height;
                    freelook.m_Orbits[i].m_Radius = originalOrbits[i].m_Radius;
                }
            }
        }
#endif

        void Update()
        {
            if (originalOrbits != null)
            {
                zAxis.Update(Time.deltaTime);
                float scale = Mathf.Lerp(minScale, maxScale, zAxis.Value);
                for (int i = 0; i < originalOrbits.Length; i++)
                {
                    freelook.m_Orbits[i].m_Height = originalOrbits[i].m_Height * scale;
                    freelook.m_Orbits[i].m_Radius = originalOrbits[i].m_Radius * scale;
                }
            }
        }
    }
}
