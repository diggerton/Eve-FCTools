using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCTools.Utilities.Models
{
    public class LineQueue : IEnumerable<Line>
    {
        public LineQueue()
        {
            queue = new Queue<Line>();
        }

        public delegate void ChangedEventHandler(object sender, EventArgs e);
        public event ChangedEventHandler OnLineEnqueued;

        private Queue<Line> queue;
        
        public void Enqueue(Line l)
        {
            queue.Enqueue(l);
            OnAdded(EventArgs.Empty);
        }
        public Line Dequeue()
        {
            return queue.Dequeue();
        }
        public int Count()
        {
            return queue.Count;
        }
        public bool HasItems()
        {
            return queue.Count > 0;
        }

        protected virtual void OnAdded(EventArgs e)
        {
            if (OnLineEnqueued != null)
                OnLineEnqueued(this, e);
        }

        public IEnumerator<Line> GetEnumerator()
        {
            return queue.GetEnumerator();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return queue.GetEnumerator();
        }
    }
}