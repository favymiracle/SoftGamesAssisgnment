using UnityEngine;

public class AnimationEventListener : MonoBehaviour
{
    [SerializeField] private bool isClearMode = false;

    private ParticleSystem[] m_particleSystems;

    private void Start()
    {
        m_particleSystems = GetComponentsInChildren<ParticleSystem>();
    }

    public void OnAnimationEvent(string message)
    {
        Debug.Log("Animation Event Triggered: " + message);

        if (message == "restart") 
        {
            RestartParticleSystem();
        }
    }

    public void RestartParticleSystem()
    {
        for (int i = 0; i < m_particleSystems.Length; i++) 
        {
            if (isClearMode)
                m_particleSystems[i].Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            else
                m_particleSystems[i].Stop();
            m_particleSystems[i].Play();
        }
    }
}
