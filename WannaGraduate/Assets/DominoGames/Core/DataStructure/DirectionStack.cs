using UnityEngine;
using System.Collections;

public class StackItem<T>
{
    T myValue;
    StackItem<T> prevItem, nextItem;

    public StackItem(T _myValue, StackItem<T> _prevItem, StackItem<T> _nextItem)
    {
        myValue = _myValue;
        prevItem = _prevItem;
        nextItem = _nextItem;
    }

    public void SetPrevItem(StackItem<T> _prevItem)
    {
        prevItem = _prevItem;
    }

    public void SetNextItem(StackItem<T> _nextItem)
    {
        nextItem = _nextItem;
    }

    public StackItem<T> GetPrevItem()
    {
        return prevItem;
    }

    public StackItem<T> GetNextItem()
    {
        return nextItem;
    }

    public T GetMyValue()
    {
        return myValue;
    }
}

public class DirectionStack<T>:IEnumerable
{
    StackItem<T> startStack, endStack;
    int length;

    StackItem<T> currentItem;
    T currentTItem;

    public T GetContentByIndex(int index){
        StackItem<T> tempStack = startStack;

        for (int i = 0; i < index - 1; i++)
        {
            tempStack = tempStack.GetNextItem();
        }

        return tempStack.GetMyValue();
    }

    public StackItem<T> GetFirstItem(){
        return startStack;
    }

    public StackItem<T> GetLastItem(){
        return endStack;
    }

    public StackItem<T> GetNextItem(StackItem<T> currentStack){
        return currentStack.GetNextItem();
    }

    public StackItem<T> GetPrevItem(StackItem<T> currentStack){
        return currentStack.GetPrevItem();
    }

    public int Count()
    {
        return length;
    }

    public void PushFront(T getValue)
    {
        if (startStack == null)
        {
            startStack = new StackItem<T>(getValue, null, null);

            if(endStack == null)
            {
                endStack = startStack;
            }
        }
        else
        {
            StackItem<T> temp = new StackItem<T>(getValue, null, startStack);
            startStack.SetPrevItem(temp);
            startStack = temp;
        }

        length++;
    }

    public void PushBack(T getValue)
    {
        if (endStack == null)
        {
            endStack = new StackItem<T>(getValue, null, null);

            if(startStack == null)
            {
                startStack = endStack;
            }
        }
        else
        {
            StackItem<T> temp = new StackItem<T>(getValue, endStack, null);
            endStack.SetNextItem(temp);
            endStack = temp;
        }

        length++;
    }

    public T PopFront()
    {
        if (length > 0)
        {
            StackItem<T> temp = startStack;
            startStack = temp.GetNextItem();
            if (startStack != null)
            {
                startStack.SetPrevItem(null);
            }
            length--;

            return temp.GetMyValue();
        }
        else
        {
            throw new System.Exception("The structure is already empty");
        }
    }

    public T PopBack()
    {
        if (length > 0)
        {
            StackItem<T> temp = endStack;
            endStack = temp.GetPrevItem();
            if (endStack != null)
            {
                endStack.SetNextItem(null);
            }
            length--;

            return temp.GetMyValue();
        }
        else
        {
            throw new System.Exception("The structure is already empty");
        }
    }
    
    IEnumerator IEnumerable.GetEnumerator()
    {
        StackItem<T> tempStack = startStack;

        for (int i = 0; i < length; i++)
        {
            yield return tempStack.GetMyValue();
            tempStack = tempStack.GetNextItem();
        }
    }
}

// Copyright, jysa000@naver.com - 댄싱돌핀