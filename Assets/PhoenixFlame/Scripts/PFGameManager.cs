using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PFGameManager : MonoBehaviour
{
    [SerializeField] private Animator m_animator;
    [SerializeField] private Toggle m_fireToggle;

    [SerializeField] private string m_startClip;
    [SerializeField] private string m_stopClip;

    [SerializeField] private Button m_back;
    void Start()
    {
        m_back.onClick.AddListener(() => { SceneManager.LoadScene(0); });

        m_fireToggle.onValueChanged.AddListener(delegate { FireToggleChecked(); });
    }

    void FireToggleChecked() 
    {
        if (m_fireToggle.isOn)
            m_animator.Play(m_startClip);
        else
            m_animator.Play(m_stopClip);
    }
}
