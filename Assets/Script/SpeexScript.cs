using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NSpeex;
using System;

public class SpeexScript : MonoBehaviour {
    SpeexDecoder speexDecoder;
    SpeexEncoder speexEncoder;
    BandMode mode;
    byte[] outBuffer = new byte[100000];

    short[] outBufferShort = new short[100000];
    float[] outBufferFloat = new float[100000];

    int minFreq, maxFreq;

    void SetMode()
    {
        Microphone.GetDeviceCaps(null, out minFreq, out maxFreq);//Gets the frequency of the device

        if (maxFreq - minFreq < 1)
        {
            minFreq = 0;
            maxFreq = 44100;
        }
        mode = GetBandMode(maxFreq);
    }

    public void CreateEncoder()
    {
        SetMode();
        speexEncoder = new SpeexEncoder(mode);
        speexEncoder.Quality = 3;
    }
    public void CreateDecoder()
    {
        SetMode();
        speexDecoder = new SpeexDecoder(mode);
    }

    public byte[] AudioEncode(float[] data, int len)
    {
        ToShortArray(data, outBufferShort, 0, len);
        int resLen = speexEncoder.Encode(outBufferShort, 0, len, outBuffer, 0, len);
        byte[] resBuff = new byte[resLen];
        Array.Copy(outBuffer, 0, resBuff, 0, resLen);
        return resBuff;
    }
    public float[] AudioDecode(byte[] data, int len)
    {
        int resLen = speexDecoder.Decode(data, 0, len, outBufferShort, 0, false);
        ToFloatArray(outBufferShort, outBufferFloat, resLen);
        float[] resBuff = new float[resLen];
        Array.Copy(outBufferFloat, 0, resBuff, 0, resLen);
        return resBuff;
    }

    static void ToShortArray(float[] input, short[] output, int offset, int len)
    {
        for (int i = 0; i < len; ++i)
        {
            output[i] = (short)Mathf.Clamp((int)(input[i + offset] * 32767.0f), short.MinValue, short.MaxValue);
        }

    }
    static void ToFloatArray(short[] input, float[] output, int length)
    {
        for (int i = 0; i < length; ++i)
        {
            output[i] = input[i] / (float)short.MaxValue;
        }
    }
    private static BandMode GetBandMode(int sampleRate)
    {

        if (sampleRate <= 8000)

            return BandMode.Narrow;

        if (sampleRate <= 16000)

            return BandMode.Wide;

        return BandMode.UltraWide;

    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
