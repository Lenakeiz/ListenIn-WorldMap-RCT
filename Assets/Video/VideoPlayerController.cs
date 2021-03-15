using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoPlayerController : MonoBehaviour
{
    public static VideoPlayerController Instance;

    [SerializeField] private CanvasGroup bgImage; 
    // ACT = 0, SART_PRACTICE = 2, SART_TEST = 3, BASKET = 4, CORE = 5, QUESTIONAIRE = 1 };
    [SerializeField] private List<VideoTherapy> listOfVideos;
    [SerializeField] private RawImage rawImage;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private CanvasGroup videoPlayerPanel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        bgImage.alpha = 0f;
        videoPlayer.loopPointReached += VideoPlayer_loopPointReached;
        videoPlayer.prepareCompleted += VideoPlayer_prepareCompleted;
    }

    private void VideoPlayer_loopPointReached(VideoPlayer source)
    {
        StopVideo();
    }

    private void VideoPlayer_prepareCompleted(VideoPlayer source)
    {
        videoPlayerPanel.alpha = 1;
        videoPlayerPanel.interactable = true;
        videoPlayerPanel.blocksRaycasts = true;
        bgImage.alpha = 1;
        rawImage.texture = source.texture;
        source.Play();
    }

    public void PlayVideo()
    {
        videoPlayer.aspectRatio = VideoAspectRatio.NoScaling;
        videoPlayer.Prepare();
    }

    public void StopVideo()
    {
        videoPlayer.Stop();
        videoPlayerPanel.alpha = 0;
        videoPlayerPanel.interactable = false;
        videoPlayerPanel.blocksRaycasts = false;
        bgImage.alpha = 0;
    }

    public void SetVideo(TherapyLadderStep therapyStep)
    {
        VideoTherapy videoTh = listOfVideos.Where(therapy => therapy.TherapyStep == therapyStep).SingleOrDefault();
        if (videoTh != null)
        {
            videoPlayer.clip = videoTh.VideoClip;
        }
        else
        {
            Debug.LogWarning("No video associated with " + therapyStep.ToString());
        }
    }
}

[Serializable]
public class VideoTherapy
{
    public TherapyLadderStep TherapyStep;
    public VideoClip VideoClip;
}