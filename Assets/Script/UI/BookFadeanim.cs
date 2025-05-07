using System.Collections;
using UnityEngine;

public class BookFadeanim : MonoBehaviour
{
    Animator animator;
    public GameObject Pannel_1;
    public GameObject Pannel_2;
    public GameObject Pannel_2_1;
    public GameObject Pannel_2_2;

    void Start()
    {
        animator = GetComponent<Animator>();
       // animator.SetTrigger("Fade");
        Pannel_1.SetActive(true);
        Pannel_2.SetActive(false);
        Pannel_2_1.SetActive(false);
        Pannel_2_2.SetActive(false);
    }

    public void BookFadeThenNext1()
    {
        StartCoroutine(FadeThenCallNext1());
    }

    public void BookFadeThenNext2_2()
    {
        StartCoroutine(FadeThenCallNext2_2());
    }

    private IEnumerator FadeThenCallNext1()
    {
        animator.SetTrigger("Fade");
        yield return new WaitForSeconds(0.7f);
        Next_1();
    }

    private IEnumerator FadeThenCallNext2_2()
    {
        animator.SetTrigger("Fade");
        yield return new WaitForSeconds(0.7f);
        Next_2_2();
    }

    public void Next_1()
    {
        Pannel_1.SetActive(false);
        Pannel_2.SetActive(true);
        Pannel_2_1.SetActive(true);
        Pannel_2_2.SetActive(false);
    }

    public void Next_2_2()
    {
        Pannel_1.SetActive(false);
        Pannel_2.SetActive(true);
        Pannel_2_1.SetActive(false);
        Pannel_2_2.SetActive(true);
    }
}