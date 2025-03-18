using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MATLABintegrationTest
{
    //custom generic linked list defined with data type T
    internal class DoubleLinkList<T> where T : IComparable
    {
        //defining head and temporary nodes for use in list
        private Node<T> head;
        private Node<T> temp;
        //variable that keeps track of length of list
        private int length = 0;

        //constructor making empty list
        public DoubleLinkList()
        {
            temp = head;
        }

        //methods

        //adds element to front of list
        public void AddFront(T inp)
        {
            Node<T> a = new Node<T>(inp);
            a.next = head;
            a.previous = null;
            head = a;
            temp = head;

            length++;
        }

        //adds element to specified position in list
        public void AddMid(T inp, int pos)
        {
            int count = 1;
            Node<T> node = new Node<T>(inp);
            if (pos == 1)
            {
                AddFront(inp);
            }
            else if (pos == 2)
            {
                node.next = head.next;
                node.previous = head;
                head.next = node;
                node.next.previous = node;
            }
            else
            {
                while (count < pos)
                {
                    temp = temp.next;
                    count++;
                }
                node.next = temp;
                node.previous = temp.previous;
                temp.previous.next = node;
                temp.previous = node;
                temp = head;
            }

            length++;

        }

        //adds element to the back of the list
        public void AddBack(T inp)
        {
            Node<T> newnode = new Node<T>(inp);
            if (temp != null)
            {
                while (temp.next != null)
                {
                    temp = temp.next;
                }
                temp.next = newnode;
                newnode.previous = temp;
                newnode.next = null;
                temp = head;
            }
            else
            {
                head = newnode;
                temp = head;
            }

            length++;

        }

        //removes element at specified position in list
        public Node<T> RemoveAt(int pos)
        {
            int x = 1;
            Node<T> temp1 = head;
            while (x < pos)
            {
                temp1 = temp1.next;
                x++;
            }
            temp1.previous.next = temp1.next;
            temp1.next.previous = temp1.previous;

            length--;

            return temp1;

        }

        //returns node at desired position
        public Node<T> GetNode(int pos)
        {
            int x = 0;
            Node<T> temp1 = head;
            while (x < pos)
            {
                temp1 = temp1.next;
                x++;
            }
            return temp1;
        }

        //returns length of list 
        public int GetLength()
        {
            return length;
        }

        //returns boolean indicating whether or not the passed element is in the list
        public bool Contains(T element)
        {
            bool contains = false;

            while (temp.next != null && contains == false)
            {
                if (element.CompareTo(temp.DisplayNode()) == 0)
                {
                    contains = true;
                }
                temp = temp.next;
            }

            temp = head;
            return contains;
        }

        //returns the index of the passed element in the list
        public int IndexOf(T element)
        {
            int index = -1;
            int counter = 0;

            while (temp.next != null && index == -1)
            {
                if (element.CompareTo(temp.DisplayNode()) == 0)
                {
                    index = counter;
                }
                temp = temp.next;
                counter++;
            }

            temp = head;
            return index;
        }
    }
}
