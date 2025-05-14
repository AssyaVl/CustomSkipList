using System;

namespace CustomSkipList
{
    /// <summary>
    /// Класс, представляющий узел Skip List, содержащий ключ, значение и связи с соседними узлами.
    /// </summary>
    /// <typeparam name="TKey">Тип ключа.</typeparam>
    /// <typeparam name="TValue">Тип значения.</typeparam>
    internal class Node<TKey, TValue>
    {
        /// <summary>
        /// Ключ узла.
        /// </summary>
        public TKey Key { get; set; }

        /// <summary>
        /// Значение узла.
        /// </summary>
        public TValue Value { get; set; }

        /// <
        /// Ссылка на следующий узел на том же уровне.
        /// </summary>
        public Node<TKey, TValue> Right { get; set; }

        /// <summary>
        /// Ссылка на узел на уровне выше.
        /// </summary>
        public Node<TKey, TValue> Up { get; set; }

        /// <summary>
        /// Ссылка на узел на уровне ниже.
        /// </summary>
        public Node<TKey, TValue> Down { get; set; }

        /// <summary>
        /// Конструктор по умолчанию для создания заголовочных узлов.
        /// </summary>
        public Node()
        {
            Key = default;
            Value = default;
            Right = null;
            Up = null;
            Down = null;
        }

        /// <summary>
        /// Конструктор для создания узла с заданным ключом и значением.
        /// </summary>
        /// <param name="key">Ключ узла.</param>
        /// <param name="value">Значение узла.</param>
        public Node(TKey key, TValue value)
        {
            Key = key;
            Value = value;
            Right = null;
            Up = null;
            Down = null;
        }
    }
}