using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MicrophoneManager : MonoBehaviour
{

    //private string _MICName;//마이크 이름
    private List<string> _allMicrophones = new List<string>();//마이크들 저장
    private AudioSource _aud;
    float[] voice;
    int minFreq, maxFreq;

    void GetMicCaps()
    {
        Microphone.GetDeviceCaps(null, out minFreq, out maxFreq);//Gets the frequency of the device

        if (maxFreq - minFreq < 1)
        {
            minFreq = 0;
            maxFreq = 44100;
        }
    }

    #region Record
    private void StartRecord()
    {
        GetMicCaps();
        _aud.clip = Microphone.Start(null, true, 1, maxFreq);
        while (!(Microphone.GetPosition(null) > 0)) { }
        _aud.Play();
        voice = new float[_aud.clip.samples];
        NetworkManager.instance.StartChat();
    }

    private void RecordVoice()
    {
        if (_aud.clip == null)
            return;
        _aud.clip.GetData(voice, 0);
        NetworkManager.instance.SendVoice(voice);
    }

    private void StopRecord()
    {
        Microphone.End(null);
        _aud.clip = null;
    }
    #endregion

    // Use this for initialization
    void Start()
    {
        if (gameObject.GetComponent<ControllManager>().isMine)
            return;

        _aud = GetComponent<AudioSource>();
        //foreach (string device in Microphone.devices)
        //{
        //    if (_MICName == null)//첫번째 마이크를 기본마이크로 설정
        //    {
        //        _MICName = device;
        //    }
        //    _allMicrophones.Add(device);
        //}

        if (Microphone.devices.Length > 0)
        {
            StartRecord();
        }
        else
        {
            _aud.clip = null;
        }
        StartCoroutine(DelayUpdate());
    }

    IEnumerator DelayUpdate()
    {
        while (true)
        {
            RecordVoice();
            yield return new WaitForSeconds(0.01f);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
