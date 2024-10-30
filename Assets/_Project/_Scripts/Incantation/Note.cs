using System;
using UnityEngine;
namespace IncantationSystem {
    [Serializable]
    public class Note {

        [field: SerializeField] public float Timing { get; private set; }
        [field: SerializeField] public NoteType Type { get; private set; }

        public Note(float timing, NoteType type) {
            Timing = timing;
            Type = type;
        }

        public Note() {
            Timing = 0;
            Type = NoteType.Cast;
        }
    }

    public enum NoteType {
        Cast,
        Attack,
    }
}
