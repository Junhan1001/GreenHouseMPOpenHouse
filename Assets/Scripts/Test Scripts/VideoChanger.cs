using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoChanger : MonoBehaviour
{

    public VideoPlayer video;

    public VideoClip clip1;
    public VideoClip clip2;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(VideoChange());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator VideoChange() 
    {
        video.clip = clip1;
        yield return new WaitForSeconds(2f);
        video.clip = clip2;
    }
}
