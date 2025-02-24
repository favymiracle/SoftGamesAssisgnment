using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public Image m_cardImg;
    public int siblingIndex;
    public float m_transferTime;
    public bool m_startAnim;

    [HideInInspector] public Vector3 m_endPosition;
    [HideInInspector] public Quaternion m_endRotation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_startAnim = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_startAnim) 
        {
            StartCoroutine(MoveToTransform(m_endPosition, m_endRotation, m_transferTime));
        }
    }

    public void Initialize(Vector3 pos, Quaternion rot, int siblingIndex) 
    {
        m_endPosition = pos - new Vector3(0, Screen.height / 2, 0);
        m_endRotation = rot;
        this.siblingIndex = siblingIndex;
    }

    public void InitCardImg(Sprite Img) 
    {
        m_cardImg.sprite = Img;
    }

    private IEnumerator MoveToTransform(Vector3 targetPosition, Quaternion targetRotation, float time)
    {
        Vector3 startPosition = m_cardImg.transform.position;
        Quaternion startRotation = m_cardImg.transform.rotation;
        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            // Interpolate between the start and target positions
            m_cardImg.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / time);
            m_cardImg.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsedTime / time);
            elapsedTime += Time.deltaTime;

            if (elapsedTime > 1.0f) 
            {
                transform.SetSiblingIndex(siblingIndex + 1);
            }

            yield return null; // Wait for the next frame
        }

        // Ensure the object reaches the exact target position
        m_cardImg.transform.position = targetPosition;
    }
}
