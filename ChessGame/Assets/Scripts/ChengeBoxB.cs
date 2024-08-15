using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ChengeBoxB : MonoBehaviour
{
    private static ChengeBoxB insance;

    public GameObject Template;
    public Sprite Qsprite;
    public Sprite Bsprite;
    public Sprite Rsprite;
    public Sprite Ksprite;
    // Start is called before the first frame update
    void Awake()
    {
        insance = this;
    }

    public static void SelectFigur(GameObject go, int dopchislo)
    {
        GameObject change_box = Instantiate(insance.Template);
        Transform panel = change_box.transform.Find("Panel");



        Button Q = panel.Find("Q").GetComponent<Button>();
        Button B = panel.Find("B").GetComponent<Button>();
        Button K = panel.Find("K").GetComponent<Button>();
        Button R = panel.Find("R").GetComponent<Button>();

        Q.onClick.AddListener(() =>
        {
            go.transform.name = "b6_"+ dopchislo;
            go.GetComponent<SpriteRenderer>().sprite = insance.Qsprite;
            Destroy(change_box);
        });
        B.onClick.AddListener(() =>
        {
            go.transform.name = "b2_"+ dopchislo;
            go.GetComponent<SpriteRenderer>().sprite = insance.Bsprite;
            Destroy(change_box);
        });
        K.onClick.AddListener(() =>
        {
            go.transform.name = "b3_" + dopchislo;
            go.GetComponent<SpriteRenderer>().sprite = insance.Ksprite;
            Destroy(change_box);
        });
        R.onClick.AddListener(() =>
        {
            go.transform.name = "b1_" + dopchislo;
            go.GetComponent<SpriteRenderer>().sprite = insance.Rsprite;
            Destroy(change_box);
        });

    }
}

