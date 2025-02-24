using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExampleAssignment : MonoBehaviour
{
    [SerializeField] private Button m_demo_1;
    [SerializeField] private Button m_demo_2;
    [SerializeField] private Button m_demo_3;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_demo_1.onClick.AddListener(() => { SceneManager.LoadScene(1); });
        m_demo_2.onClick.AddListener(() => { SceneManager.LoadScene(2); });
        m_demo_3.onClick.AddListener(() => { SceneManager.LoadScene(3); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
