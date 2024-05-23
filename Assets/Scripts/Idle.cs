using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UISpriteAnimation : MonoBehaviour
{
    public Image m_Image;
    public Sprite[] m_SpriteArray;
    public float m_Speed = 0.02f;

    private int m_IndexSprite;
    private Coroutine m_CoroutineAnim;
    private bool isDone;

    // This method is called when the script instance is being loaded.
    void Start()
    {
        // Start the animation automatically when the game starts
        Func_PlayUIAnim();
    }

    public void Func_PlayUIAnim()
    {
        isDone = false;
        m_IndexSprite = 0 ;
        m_CoroutineAnim = StartCoroutine(Func_PlayAnimUI());
        //StartCoroutine(Func_PlayAnimUI());
    }

    public void Func_StopUIAnim()
    {
        isDone = true;
        if (m_CoroutineAnim != null)
        {
            StopCoroutine(m_CoroutineAnim);
        }
    }

    IEnumerator Func_PlayAnimUI()
    {
        while (!isDone)
        {
            yield return new WaitForSeconds(m_Speed);

            if (m_IndexSprite >= m_SpriteArray.Length)
            {
                m_IndexSprite = 0;
            }

            m_Image.sprite = m_SpriteArray[m_IndexSprite];
            m_IndexSprite += 1;
        }
    }
}
