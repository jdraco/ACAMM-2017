using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum WhichPeople
{
    WP_ACAMM,
    WP_ASMAM,
    WP_SPOUSE
}

public class ScheduleLast : MonoBehaviour
{
    public GameObject ACAMM, ASMAM, SPOUSE;
    public Button ACAMMButton, ASMAMButton, SPOUSEButton;
    public Image TopLeftLogo;

    public Sprite ACAMMLogo, ASMAMLogo, SPOUSELogo;

    void Start()
    {
        ACAMM.SetActive(true);
        ASMAM.SetActive(false);
        SPOUSE.SetActive(false);

        ACAMMButton.interactable = false;
        TopLeftLogo.sprite = ACAMMLogo;
    }


    public void ChangeSchedule(int WhichOne)
    {
        WhichPeople ThisPeople = (WhichPeople)WhichOne; 

        switch (ThisPeople)
        {
            case WhichPeople.WP_ACAMM:
                ACAMM.SetActive(true);
                ASMAM.SetActive(false);
                SPOUSE.SetActive(false);

                ACAMMButton.interactable = false;
                ASMAMButton.interactable = true;
                SPOUSEButton.interactable = true;

                TopLeftLogo.sprite = ACAMMLogo;
                break;
            case WhichPeople.WP_ASMAM:
                ACAMM.SetActive(false);
                ASMAM.SetActive(true);
                SPOUSE.SetActive(false);

                ACAMMButton.interactable = true;
                ASMAMButton.interactable = false;
                SPOUSEButton.interactable = true;

                TopLeftLogo.sprite = ASMAMLogo;
                break;
            case WhichPeople.WP_SPOUSE:
                ACAMM.SetActive(false);
                ASMAM.SetActive(false);
                SPOUSE.SetActive(true);

                ACAMMButton.interactable = true;
                ASMAMButton.interactable = true;
                SPOUSEButton.interactable = false;

                TopLeftLogo.sprite = SPOUSELogo;
                break;
        }

    }
}
