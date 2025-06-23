using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class MainMenu : MonoBehaviour
{
    [SerializeField] protected Animator anim; //componente animador del menu (0->intro, 1->idle, 2->conf, 3->levelselection)
    [SerializeField] protected Toggle tooltipToggle, ostToggle;

    [SerializeField] protected AudioSource ostPlayer; //reproductor de musica

    [SerializeField] protected GameObject videoPlayerObject;
    [SerializeField] protected VideoPlayer videoPlayer;

    private void Awake()
    {
        Application.targetFrameRate = 60; //para que el apk corra fluido
        QualitySettings.vSyncCount = 1; //prevenir screen-tearing
        videoPlayer.loopPointReached += OnVideoFinished;
        ostPlayer.enabled = SettingsManager.GetEnableOst();
    }

    public void SetShowTooltip(bool show) { SettingsManager.SetShowTooltip(show); }
    public void SetOstEnabled(bool enable) 
    { 
        SettingsManager.SetEnableOst(enable); 
        ostPlayer.enabled = enable;
    }

    public void SetMenuState(int state) 
    {
        anim.SetInteger("state", state);
    }

    public void SetUpToogles() 
    {
        tooltipToggle.SetIsOnWithoutNotify(SettingsManager.GetShowTooltip());
        tooltipToggle.onValueChanged.AddListener(SetShowTooltip);

        ostToggle.SetIsOnWithoutNotify(SettingsManager.GetEnableOst());
        ostToggle.onValueChanged.AddListener(SetOstEnabled);
    }

    public void GoToLevel(int levelIndex) 
    {
        SceneManager.LoadScene(levelIndex);
    }

    public void ExitGame() 
    {
        Application.Quit();
    }

    public void ShowVideo() 
    {
        if (SettingsManager.GetEnableOst()) ostPlayer.enabled = false;
        videoPlayerObject.SetActive(true);
        videoPlayer.Play();
    }

    public void ExitVideo() 
    {
        if(SettingsManager.GetEnableOst()) ostPlayer.enabled = true;
        videoPlayer.Stop();
        videoPlayer.SetDirectAudioMute(0, true);
        videoPlayerObject.SetActive(false);
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        ExitVideo();
    }
}
