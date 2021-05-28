using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Metalogic.DataUtil
{
    public class SynchronizedQueue<T> 
    {
        private readonly Queue<T> _queue = new Queue<T>();

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Enqueue(T obj)
        {
            _queue.Enqueue(obj);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool Any()
        {
            return _queue.Count > 0;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public int Count()
        {
            return _queue.Count;
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        public T Dequeue()
        {
            return _queue.Dequeue();
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        public List<T> DequeueAll()
        {
            var lst = _queue.ToList();
            _queue.Clear();
            return lst;
        }
    }
}
