using System;
using System.Collections;
using System.Collections.Generic;

namespace TixFactory.Configuration
{
    /// <inheritdoc cref="ITrackedList{T}"/>
    public class TrackedList<T> : ITrackedList<T>
    {
        private readonly IManufacturedSetting<int> _Count;
        private readonly IList<T> _List;

        /// <inheritdoc cref="ICollection{T}.Count"/>
        public int Count => _Count.Value;

        /// <inheritdoc cref="ICollection{T}.IsReadOnly"/>
        public bool IsReadOnly => _List.IsReadOnly;

        /// <inheritdoc cref="IList{T}.this[int]"/>
        public T this[int index]
        {
            get => _List[index];
            set => _List[index] = value;
        }

        /// <inheritdoc cref="ITrackedList{T}.CountSetting"/>
        public IReadOnlySetting<int> CountSetting => _Count;

        /// <summary>
        /// Initailizes a new <see cref="TrackedList{T}"/>.
        /// </summary>
        public TrackedList()
            : this(new List<T>())
        {
        }

        /// <summary>
        /// Initailizes a new <see cref="TrackedList{T}"/>.
        /// </summary>
        /// <param name="list">The internal <see cref="List{T}"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// - <paramref name="list"/>
        /// </exception>
        public TrackedList(IList<T> list)
        {
            _List = list ?? throw new ArgumentNullException(nameof(list));
            _Count = new ManufacturedSetting<int>(() => list.Count, refreshOnRead: true);
        }

        /// <inheritdoc cref="ICollection{T}.Add"/>
        public void Add(T item)
        {
            _List.Add(item);
            _Count.Refresh();
        }

        /// <inheritdoc cref="ICollection{T}.Clear"/>
        public void Clear()
        {
            _List.Clear();
            _Count.Refresh();
        }

        /// <inheritdoc cref="ICollection{T}.Contains"/>
        public bool Contains(T item)
        {
            return _List.Contains(item);
        }

        /// <inheritdoc cref="ICollection{T}.CopyTo"/>
        public void CopyTo(T[] array, int arrayIndex)
        {
            _List.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc cref="ICollection{T}.Remove"/>
        public bool Remove(T item)
        {
            var removed = _List.Remove(item);
            _Count.Refresh();

            return removed;
        }

        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
        public IEnumerator<T> GetEnumerator()
        {
            return _List.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc cref="IList{T}.IndexOf"/>
        public int IndexOf(T item)
        {
            return _List.IndexOf(item);
        }

        /// <inheritdoc cref="IList{T}.Insert"/>
        public void Insert(int index, T item)
        {
            _List.Insert(index, item);
            _Count.Refresh();
        }

        /// <inheritdoc cref="IList{T}.RemoveAt"/>
        public void RemoveAt(int index)
        {
            _List.RemoveAt(index);
            _Count.Refresh();
        }
    }
}
