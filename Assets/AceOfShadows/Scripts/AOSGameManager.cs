using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AOSGameManager : MonoBehaviour
{
    [SerializeField] private Card m_cardPrefab;
    [SerializeField] private Canvas m_instanceContent;
    [SerializeField] private Sprite[] m_cardImgs;

    [SerializeField] private int cardCount = 144;
    [SerializeField] private float moveDuration = 1.0f;
    [SerializeField] private float transferTime = 2.0f;

    [SerializeField] private Button m_back;

    private List<Card> cards;
    private List<Vector3> cardsPos;
    private List<Quaternion> cardsRot;
    private float nTime;
    private int index;
    private int nextDeckIndex;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_back.onClick.AddListener(() => { SceneManager.LoadScene(0); });

        cards = new List<Card>();
        cardsPos = new List<Vector3>();
        cardsRot = new List<Quaternion>();
        nTime = 0.0f;
        index = cardCount - 1;
        nextDeckIndex = 0;

        for (int i = 0; i < cardCount; i++) 
        {
            Card Instance = Instantiate(m_cardPrefab.gameObject, m_instanceContent.transform).GetComponent<Card>();
            Instance.m_transferTime = transferTime;
            Instance.InitCardImg(m_cardImgs[i % m_cardImgs.Length]);
            Instance.transform.localRotation = Quaternion.Euler(0, 0, 15 - (i + 1) * 0.25f);

            cards.Add(Instance);

            cardsPos.Add(Instance.m_cardImg.transform.position);
            cardsRot.Add(Instance.m_cardImg.transform.rotation);
        }

        Destroy(m_cardPrefab.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (index == -1) return;

        nTime += Time.deltaTime;

        if (nTime > moveDuration) 
        {
            cards[index].Initialize(cardsPos[nextDeckIndex], cardsRot[nextDeckIndex], nextDeckIndex);
            cards[index].m_startAnim = true;

            index--;
            nextDeckIndex++;
            nTime = 0.0f;
        }
    }


}
