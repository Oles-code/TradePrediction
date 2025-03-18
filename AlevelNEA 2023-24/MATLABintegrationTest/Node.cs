using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MATLABintegrationTest
{
    internal class Node<T> where T : IComparable
    {
        public T data;
        public Node<T> next;
        public Node<T> previous;

        public Node(T inp)
        {
            data = inp;
        }

        public T DisplayNode()
        {
            return data;
        }
    }
}
