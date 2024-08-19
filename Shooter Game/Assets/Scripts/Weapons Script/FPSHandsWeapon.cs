using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class FPSHandsWeapon : MonoBehaviour
{
    public AudioClip shootClip, reloadClip;
    private AudioSource audioManager;
    private GameObject muzzleFlash;

    private Animator anim;

    private string shoot = "Shoot";
    private string reload = "Reload";
    void Awake()
    {
        muzzleFlash = transform.Find("MuzzleFlash").gameObject;
        muzzleFlash.SetActive(false);

        audioManager = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
    }

    public void Shoot()
    {
        if(audioManager.clip != shootClip)
        {
            audioManager.clip = shootClip;
        }
        audioManager.Play();

        StartCoroutine(TurnOnMuzzleFlash());

        anim.SetTrigger(shoot);
    }

    IEnumerator TurnOnMuzzleFlash ()
    {
        muzzleFlash.SetActive (true);
        yield return new WaitForSeconds(0.05f);
        muzzleFlash.SetActive(false);
    }

    public void Reload()
    {
        StartCoroutine(PlayReloadSound());
        anim.SetTrigger(reload);
    }
    IEnumerator PlayReloadSound()
    {
        yield return new WaitForSeconds(0.8f);
        if(audioManager.clip != reloadClip) {
            audioManager.clip = reloadClip;
        }
        audioManager.Play();
    }

}
