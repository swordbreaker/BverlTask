using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uebung12
{
    /**
     * Space bounded priority queue (thread-safe). The elements are ordered by the natural order of the elements.
     * @author Christoph Stamm
     *
     * @param <E> data type of elements
     */
    public class BoundedPQ<E> : IEnumerable<E>
    {
        private int _capacity;     // maximum number of elements in the priority queue
        private SortedSet<E> _pq;    // priority queue implemented by a tree set

        /**
         * Creates a bounded priority queue for a maximum of capacity elements
         * @param capacity
         */
        public BoundedPQ(int capacity)
        {
            _capacity = capacity;
            _pq = new SortedSet<E>();
        }

        public BoundedPQ(int capacity, IComparer<E> comparer)
        {
            _capacity = capacity;
            _pq = new SortedSet<E>(comparer);
        }

        /**
         * Adds a new element e to the priority queue
         * @param e
         */
        public void Add(E e)
        {
            lock(this)
            {
                _pq.Add(e);
                if (_pq.Count > _capacity)
                {
                    _pq.Remove(_pq.First());
                }
            }

        }

        /**
         * Returns an element with highest priority
         * @return
         */
        public E Last()
        {
            lock(this)
            {
                return _pq.Last();
            }
        }

        /**
         * Removes and returns an element with highest priority
         * @return
         */
        public E PollLast()
        {
            lock(this)
            {
                var last = _pq.Last();
                _pq.Remove(last);
                return last;
            }
        }

        /**
         * Returns the number of elements in the priority queue
         * @return
         */
        public int Count()
        {
            return _pq.Count;
        }

        public IEnumerator<E> GetEnumerator()
        {
            return _pq.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _pq.GetEnumerator();
        }
    }
}
