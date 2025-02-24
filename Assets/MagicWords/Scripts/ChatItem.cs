using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ChatItem : MonoBehaviour
{
    [SerializeField] private Image m_leftProfile;
    [SerializeField] private Image m_rightProfile;
    [SerializeField] private TMP_Text m_text;
    [SerializeField] private bool m_isleft;

    public void Initialize(bool isLeft, string text, Sprite avatar) 
    {
        m_isleft = isLeft;
        m_text.text = text;

        if (m_isleft)
        {
            m_leftProfile.sprite = avatar == null ? m_leftProfile.sprite : avatar;
            m_rightProfile.gameObject.SetActive(false);
        }
        else
        {
            m_rightProfile.sprite = avatar == null ? m_leftProfile.sprite : avatar;
            m_leftProfile.gameObject.SetActive(false);
        }

        m_text.alignment = m_leftProfile.gameObject.activeSelf ? TextAlignmentOptions.Left : TextAlignmentOptions.Right;
    }

    public void InitializeSpriteAssets(TMP_SpriteAsset spriteAssets) 
    {
        m_text.spriteAsset = spriteAssets;
    }
}
