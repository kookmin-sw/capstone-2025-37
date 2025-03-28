using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DominoGames.Core.DataStructure
{
    public class OrderedLinkedList<T>
    {
        public OrderedLinkedListNode<T> First;
        public OrderedLinkedListNode<T> Last;

        public void Add(int orderId, T value)
        {
            var node = new OrderedLinkedListNode<T>(orderId, value);

            if (First == null)
            {
                First = node;
                Last = node;
            }
            else
            {
                var compNode = First;

                while(compNode != null)
                {
                    if(orderId < compNode.OrderId)
                    {
                        break;
                    }

                    compNode = compNode.Next;
                }

                // 마지막까지 순회하여 compNode = null 인 상태
                if(compNode == null)
                {
                    node.Prev = Last;
                    Last.Next = node;
                    Last = node;
                }
                else
                {
                    var prevNode = compNode.Prev;

                    if(prevNode == null)
                    {
                        First = node;
                    }
                    else
                    {
                        prevNode.Next = node;
                    }

                    compNode.Prev = node;

                    node.Prev = prevNode;
                    node.Next = compNode;
                }
            }
        }
    }

    public class OrderedLinkedListNode<T>
    {
        public int OrderId = 0;
        public T Value;


        public OrderedLinkedListNode<T> Prev, Next;

        public OrderedLinkedListNode(int orderId, T nodeValue){
            this.OrderId = orderId;
            this.Value = nodeValue;
        }
    }
}
