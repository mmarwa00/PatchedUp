using UnityEngine;
using UnityEngine.Audio;

public class XylophonePuzzleController : MonoBehaviour
{
    [SerializeField] private AudioClip keyC1;
    [SerializeField] private AudioClip keyD;
    [SerializeField] private AudioClip keyE;
    [SerializeField] private AudioClip keyF;
    [SerializeField] private AudioClip keyG;
    [SerializeField] private AudioClip keyA;
    [SerializeField] private AudioClip keyB;
    [SerializeField] private AudioClip keyC2;
    [SerializeField] private AudioMixerGroup audioMixerGroup;
    
    private int melodyKeyAmount;
    private int melodyIterationAmount;
}
