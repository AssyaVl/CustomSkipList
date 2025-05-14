using System;
using System.Collections;
using System.Collections.Generic;

namespace CustomSkipList
{
    /// <summary>
    /// Класс, реализующий Skip List — вероятностную структуру данных для хранения пар ключ-значение.
    /// Поддерживает операции добавления, поиска, удаления и перебора элементов с логарифмической сложностью в среднем.
    /// </summary>
    /// <typeparam name="TKey">Тип ключа, должен реализовывать IComparable&lt;TKey&gt;.</typeparam>
    /// <typeparam name="TValue">Тип значения.</typeparam>
    public class SkipList<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>> where TKey : IComparable<TKey>
    {
        /// <summary>
        /// Массив заголовочных узлов для каждого уровня Skip List.
        /// </summary>
        private Node<TKey, TValue>[] _head;

        /// <summary>
        /// Вероятность добавления узла на следующий уровень (по умолчанию 0.5).
        /// </summary>
        private readonly double _probability;

        /// <summary>
        /// Максимальное количество уровней в Skip List (по умолчанию 10).
        /// </summary>
        private readonly int _maxLevel;

        /// <summary>
        /// Текущий максимальный занятый уровень.
        /// </summary>
        private int _curLevel;

        /// <summary>
        /// Генератор случайных чисел для определения уровня нового узла.
        /// </summary>
        private readonly Random _rd;

        /// <summary>
        /// Количество элементов в Skip List.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Конструктор Skip List.
        /// </summary>
        /// <param name="maxLevel">Максимальное количество уровней (по умолчанию 10).</param>
        /// <param name="probability">Вероятность добавления узла на следующий уровень (по умолчанию 0.5).</param>
        /// <exception cref="ArgumentOutOfRangeException">Если maxLevel меньше 1 или probability не в диапазоне (0, 1).</exception>
        public SkipList(int maxLevel = 10, double probability = 0.5)
        {
            if (maxLevel < 1)
                throw new ArgumentOutOfRangeException(nameof(maxLevel), "Максимальный уровень должен быть больше 0.");
            if (probability <= 0 || probability >= 1)
                throw new ArgumentOutOfRangeException(nameof(probability), "Вероятность должна быть в диапазоне (0, 1).");

            _maxLevel = maxLevel;
            _probability = probability;
            _head = new Node<TKey, TValue>[_maxLevel];
            for (int i = 0; i < maxLevel; i++)
            {
                _head[i] = new Node<TKey, TValue>();
                if (i == 0) continue;
                _head[i - 1].Up = _head[i];
                _head[i].Down = _head[i - 1];
            }

            _curLevel = 0;
            _rd = new Random(DateTime.Now.Millisecond);
        }

        /// <summary>
        /// Добавляет новую пару ключ-значение в Skip List.
        /// </summary>
        /// <param name="key">Ключ элемента.</param>
        /// <param name="value">Значение элемента.</param>
        /// <exception cref="ArgumentNullException">Если ключ равен null.</exception>
        /// <exception cref="ArgumentException">Если ключ уже существует в Skip List.</exception>
        public void Add(TKey key, TValue value)
        {
            ValidateKey(key);
            var prevNodes = FindPreviousNodes(key);
            int level = DetermineNodeLevel();
            EnsureLevelCapacity(level, prevNodes);
            InsertNode(key, value, prevNodes, level);
            Count++;
        }

        /// <summary>
        /// Проверяет ключ на null и уникальность.
        /// </summary>
        /// <param name="key">Ключ для проверки.</param>
        /// <exception cref="ArgumentNullException">Если ключ равен null.</exception>
        /// <exception cref="ArgumentException">Если ключ уже существует.</exception>
        private void ValidateKey(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key), "Ключ не может быть null.");

            if (Contains(key))
                throw new ArgumentException("Элемент с таким ключом уже существует.", nameof(key));
        }

        /// <summary>
        /// Определяет уровень нового узла на основе вероятности.
        /// </summary>
        /// <returns>Уровень нового узла (от 0 до maxLevel-1).</returns>
        private int DetermineNodeLevel()
        {
            int level = 0;
            while (_rd.NextDouble() < _probability && level < _maxLevel - 1)
            {
                level++;
            }
            return level;
        }

        /// <summary>
        /// Увеличивает текущий уровень, если новый узел требует большего уровня.
        /// </summary>
        /// <param name="level">Требуемый уровень.</param>
        /// <param name="prevNodes">Массив узлов, предшествующих вставляемому.</param>
        private void EnsureLevelCapacity(int level, Node<TKey, TValue>[] prevNodes)
        {
            while (_curLevel < level)
            {
                _curLevel++;
                prevNodes[_curLevel] = _head[_curLevel];
            }
        }

        /// <summary>
        /// Вставляет новый узел на заданных уровнях.
        /// </summary>
        /// <param name="key">Ключ узла.</param>
        /// <param name="value">Значение узла.</param>
        /// <param name="prevNodes">Массив узлов, предшествующих вставляемому.</param>
        /// <param name="level">Максимальный уровень для вставки.</param>
        private void InsertNode(TKey key, TValue value, Node<TKey, TValue>[] prevNodes, int level)
        {
            Node<TKey, TValue> downNode = null;
            for (int i = 0; i <= level; i++)
            {
                var node = new Node<TKey, TValue>(key, value) { Right = prevNodes[i].Right };
                prevNodes[i].Right = node;

                if (downNode != null)
                {
                    node.Down = downNode;
                    downNode.Up = node;
                }

                downNode = node;
            }
        }

        /// <summary>
        /// Проверяет, содержится ли ключ в Skip List.
        /// </summary>
        /// <param name="key">Ключ для поиска.</param>
        /// <returns>True, если ключ найден, иначе false.</returns>
        /// <exception cref="ArgumentNullException">Если ключ равен null.</exception>
        public bool Contains(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key), "Ключ не может быть null.");

            var currentNode = _head[_curLevel];
            for (int i = _curLevel; i >= 0; i--)
            {
                while (currentNode.Right != null && currentNode.Right.Key.CompareTo(key) < 0)
                {
                    currentNode = currentNode.Right;
                }
                if (currentNode.Right != null && currentNode.Right.Key.CompareTo(key) == 0)
                {
                    return true;
                }
                if (currentNode.Down == null)
                    break;
                currentNode = currentNode.Down;
            }
            return false;
        }

        /// <summary>
        /// Удаляет узел с указанным ключом из Skip List.
        /// </summary>
        /// <param name="key">Ключ узла для удаления.</param>
        /// <exception cref="ArgumentNullException">Если ключ равен null.</exception>
        /// <exception cref="ArgumentException">Если список пуст или ключ не найден.</exception>
        public void Remove(TKey key)
        {
            if (Count == 0)
                throw new ArgumentException("Список пуст.", nameof(key));

            if (key == null)
                throw new ArgumentNullException(nameof(key), "Ключ не может быть null.");

            var prevNodes = FindPreviousNodes(key);
            ValidateKeyExists(prevNodes, key);
            RemoveNodeAndUpdateLinks(prevNodes, key);
            Count--;
        }

        /// <summary>
        /// Проверяет, существует ли узел с указанным ключом.
        /// </summary>
        /// <param name="prevNodes">Массив узлов, предшествующих удаляемому.</param>
        /// <param name="key">Ключ для проверки.</param>
        /// <exception cref="ArgumentException">Если ключ не найден.</exception>
        private void ValidateKeyExists(Node<TKey, TValue>[] prevNodes, TKey key)
        {
            if (prevNodes[0] == null || prevNodes[0].Right == null || !prevNodes[0].Right.Key.Equals(key))
            {
                throw new ArgumentException("Узел с указанным ключом не найден.", nameof(key));
            }
        }

        /// <summary>
        /// Удаляет узел и обновляет связи на всех уровнях.
        /// </summary>
        /// <param name="prevNodes">Массив узлов, предшествующих удаляемому.</param>
        /// <param name="key">Ключ удаляемого узла.</param>
        private void RemoveNodeAndUpdateLinks(Node<TKey, TValue>[] prevNodes, TKey key)
        {
            for (int i = 0; i < prevNodes.Length && prevNodes[i] != null; i++)
            {
                if (prevNodes[i].Right != null && prevNodes[i].Right.Key.Equals(key))
                {
                    prevNodes[i].Right = prevNodes[i].Right.Right;
                }
            }
        }

        /// <summary>
        /// Находит узлы, предшествующие заданному ключу на каждом уровне.
        /// </summary>
        /// <param name="key">Ключ для поиска.</param>
        /// <returns>Массив узлов, предшествующих ключу на каждом уровне.</returns>
        private Node<TKey, TValue>[] FindPreviousNodes(TKey key)
        {
            var prevNodes = new Node<TKey, TValue>[_maxLevel];
            var currentNode = _head[_curLevel];

            for (int i = _curLevel; i >= 0; i--)
            {
                while (currentNode.Right != null && currentNode.Right.Key.CompareTo(key) < 0)
                {
                    currentNode = currentNode.Right;
                }
                prevNodes[i] = currentNode;

                if (currentNode.Down == null)
                    break;
                currentNode = currentNode.Down;
            }
            return prevNodes;
        }

        /// <summary>
        /// Возвращает перечислитель для элементов Skip List (на нижнем уровне).
        /// </summary>
        /// <returns>Перечислитель пар ключ-значение.</returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            for (var node = _head[0].Right; node != null; node = node.Right)
            {
                yield return new KeyValuePair<TKey, TValue>(node.Key, node.Value);
            }
        }

        /// <summary>
        /// Возвращает необобщённый перечислитель для Skip List.
        /// </summary>
        /// <returns>Необобщённый перечислитель.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}