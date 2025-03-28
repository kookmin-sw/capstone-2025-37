using UnityEngine;

namespace RNGNeeds.Samples.RandomAudioClip
{
    public class UnitAudio : MonoBehaviour
    {
        public ProbabilityList<AudioClip> selectResponses;
        public ProbabilityList<AudioClip> moveResponses;
        public ProbabilityList<AudioClip> interactResponses;
    }
}