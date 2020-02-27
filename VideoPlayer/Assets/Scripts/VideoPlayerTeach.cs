using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoPlayerTeach : MonoBehaviour
{
    //图像
    public RawImage image;
    //播放器
    public VideoPlayer vPlayer;
    //public string urlNetWork = "http://www.quirksmode.org/html5/videos/big_buck_bunny.mp4";//网络视频路径
    //播放
    public Button btn_Play;
    //暂停
    public Button btn_Pause;
    //前进
    public Button btn_Fornt;
    //后退
    public Button btn_Back;
    //下一个
    public Button btn_Next;
    //静音
    public Button btn_Mute;
    //视频控制器
    public Slider sliderVideo;
    //音量控制器
    public Slider sliderSource;
    //音量大小
    public Text text;
    //当前视频时间
    public Text text_Time;
    //视频总时长
    public Text text_Count;
    //音频组件
    public AudioSource source;
    //需要添加播放器的物体
    public GameObject obj;
    //前进后退的大小
    public float numBer = 20f;
    //时 分的转换
    private int hour, mint;
    private float time;
    private float time_Count;
    private float time_Current;
    //视频是否播放完成
    private bool isVideo;
    private bool mute;
    private bool show;

    // Use this for initialization
    void Start()
    {
        //唤醒时就播放关闭
        vPlayer.playOnAwake = false;
        source.playOnAwake = false;

        //初始化
        Init();

        btn_Play.onClick.AddListener(delegate { OnClick(0); });
        btn_Pause.onClick.AddListener(delegate { OnClick(1); });
        btn_Fornt.onClick.AddListener(delegate { OnClick(2); });
        btn_Back.onClick.AddListener(delegate { OnClick(3); });
        btn_Next.onClick.AddListener(delegate { OnClick(4); });
        btn_Mute.onClick.AddListener(delegate { OnClick(5); });

        sliderSource.value = source.volume;
        text.text = string.Format("音量{0:0}%", source.volume * 100);
        sliderSource.onValueChanged.AddListener(delegate { ChangeSource(sliderSource.value); });
    }

    /// <summary>
    /// 初始化VideoPlayer
    /// </summary>
    /// <param name="url"></param>
    private void Init()
    {
        show = true;
        isVideo = true;
        mute = false;
        time_Count = 0;
        time_Current = 0;
        sliderVideo.value = 0;
        //设置RenderTexture或RawImage上渲染VideoPlayer
        vPlayer.renderMode = VideoRenderMode.RenderTexture;
        //设置为URL模式
        vPlayer.source = VideoSource.VideoClip;
        //设置播放路径
        //vPlayer.url = url;
        //在视频中嵌入的音频类型
        vPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        //当VideoPlayer全部设置好的时候调用，帧立即可用
        vPlayer.prepareCompleted += OnPrepareFinished;
        //准备播放所需要的资源，预加载部分或者全部要播放的视频
        vPlayer.Prepare();
    }

    /// <summary>
    /// 改变音量大小
    /// </summary>
    /// <param name="value"></param>
    public void ChangeSource(float value)
    {
        source.volume = value;
        text.text = string.Format("音量{0:0}%", value * 100);
    }

    /// <summary>
    /// 改变视频进度
    /// </summary>
    /// <param name="value"></param>
    public void ChangeVideo(float value)
    {
        if (vPlayer.isPrepared)
        {
            vPlayer.time = (long)value;
            Debug.Log("VideoPlayer Time:" + vPlayer.time);
            time = (float)vPlayer.time;
            hour = (int)time / 60;
            mint = (int)time % 60;
            text_Time.text = string.Format("{0:D2}:{1:D2}", hour, mint);
        }
    }

    private void OnClick(int num)
    {
        switch (num)
        {
            case 0:
                vPlayer.Play();
                Time.timeScale = 1;
                break;
            case 1:
                vPlayer.Pause();
                //Time.timeScale = 0;
                break;
            case 2:
                sliderVideo.value = sliderVideo.value + numBer;
                break;
            case 3:
                sliderVideo.value = sliderVideo.value - numBer;
                break;
            case 4:
                vPlayer.Stop();
                //Init(Application.streamingAssetsPath + "/EasyMovieTexture.mp4");
                break;
            case 5:
                if (mute)
                {
                    source.mute = false;
                    mute = false;
                }
                else
                {
                    source.mute = true;
                    mute = true;
                }
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs((int)vPlayer.time - (int)sliderVideo.maxValue) == 0)
        {
            vPlayer.frame = (long)vPlayer.frameCount;
            vPlayer.Stop();
            Debug.Log("播放完成！");
            isVideo = false;
            return;
        }
        else if (isVideo && vPlayer.isPlaying)
        {
            time_Count += Time.deltaTime;
            if ((time_Count - time_Current) >= 1)
            {
                sliderVideo.value += 1;
                Debug.Log("value:" + sliderVideo.value);
                time_Current = time_Count;
            }
        }
    }

    void OnPrepareFinished(VideoPlayer player)
    {
        //把图像赋给RawImage
        image.texture = vPlayer.texture;
        //帧数/帧速率=总时长    如果是本地直接赋值的视频，我们可以通过VideoClip.length获取总时长
        sliderVideo.maxValue = vPlayer.frameCount / vPlayer.frameRate;

        time = sliderVideo.maxValue;
        hour = (int)time / 60;
        mint = (int)time % 60;
        text_Count.text = string.Format("/  {0:D2}:{1:D2}", hour, mint);

        sliderVideo.onValueChanged.AddListener(delegate { ChangeVideo(sliderVideo.value); });

        player.Play();
    }
}
