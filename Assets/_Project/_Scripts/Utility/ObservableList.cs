using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Utility {
    [Serializable]
    public class ObservableList<T> : IList<T> {
        [SerializeField] List<T> list = new();
        static List<T> empty = new();
        public event Action<ChangeArgs> Changed;
        
        public IEnumerator<T> GetEnumerator() => list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public bool Contains(T item) => list.Contains(item);
        public int Count => list.Count;
        public bool IsReadOnly => false;
        public int IndexOf(T item) => list.IndexOf(item);
        public void CopyTo(T[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);
        public void Add(T item) {
            list.Add(item);
            Changed?.Invoke(new(empty, new(){item}));
        }
        public void Clear() {
            Changed?.Invoke(new(list, empty));
            list.Clear();
        }
        public bool Remove(T item) {
            if (!list.Remove(item)) return false;
            Changed?.Invoke(new(new(){item}, empty));
            return true;
        }
        public void Insert(int index, T item) {
            list.Insert(index, item);
            Changed?.Invoke(new(empty, new(){item}));
        }
        public void RemoveAt(int index) {
            T item = list[index];
            list.RemoveAt(index);
            Changed?.Invoke(new(new(){item}, empty));
        }
        public T this[int index] {
            get => list[index];
            set {
                var old = list[index];
                list[index] = value;
                Changed?.Invoke(new(new(){old}, new(){value}));
            }
        }
        
        public struct ChangeArgs {
            public List<T> ItemsToRemove { get; }
            public List<T> ItemsToAdd { get; }
            public ChangeArgs(List<T> itemsToRemove, List<T> itemsToAdd) {
                ItemsToRemove = itemsToRemove;
                ItemsToAdd = itemsToAdd;
            }
        }
    }
}