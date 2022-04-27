using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Heap<T> where T:IHeapItem<T>
{
    private int currentItemCount;
    public T[] heap;

    public int Count { get { return currentItemCount; } }
    public Heap(int maxHeapSize)
    { 
        heap=new T[maxHeapSize];
    }
    public void Add(T item)
    {
        item.heapIndex = currentItemCount;  
        heap[currentItemCount] = item;
        Sortup(item);
        currentItemCount++;
    }
    public T RemoveFirst()
    {
        T firstItem=heap[0];
        currentItemCount--;
        heap[0]=heap[currentItemCount];
        heap[0].heapIndex = 0;
        SortDown(heap[0]);
        return firstItem;
    }
    public void Update(T item)
    {
        Sortup(item);
    }
    public bool Contains(T item)
    {

            return Equals(heap[item.heapIndex], item)&& item.heapIndex < currentItemCount;

    }
    private void SortDown(T item)
    {
        int childIndexRight;
        int childIndexLeft;
        int swapIndex;
        while (true)
        {
            childIndexLeft = item.heapIndex * 2 + 1;
            childIndexRight = item.heapIndex * 2 + 2;

            if (childIndexLeft<currentItemCount)    //left child exists
            {
                swapIndex = childIndexLeft;
                if (childIndexRight<currentItemCount)  //right child exists
                {
                    if (heap[childIndexLeft].CompareTo(heap[childIndexRight])<0)
                        {
                        swapIndex = childIndexRight;
                    }
                }
                if (item.CompareTo(heap[swapIndex])<0)
                    {
                    Swap(heap[swapIndex],item); 
                }
                else  //already sorted
                {
                    return;
                }
            }
            else   //no childs
            {
                return;
            }
           
        }

    }
    private void Sortup(T item)
    {
        int parentIndex = (item.heapIndex-1)/2;
        while (true)
        {
            T parentItem=heap[parentIndex];
            if (item.CompareTo(parentItem)>0)
            {
                Swap(item, parentItem);
            }
            else
            {
                break;
            }
            parentIndex = (item.heapIndex - 1) / 2;
        }
    }
    private void Swap(T itemA,T itemB)
    {
        heap[itemA.heapIndex] = itemB;
        heap[itemB.heapIndex] = itemA;
        int itemAheapIndex=itemA.heapIndex;
        itemA.heapIndex=itemB.heapIndex;
        itemB.heapIndex=itemAheapIndex;

    }
    public override string ToString()
    {
        string res = "";
        foreach (T n in heap)
        {
            if (n != null)
            {
                res += n+ "\n";
            }
        }
        return res;
    }
    public void Clear()
    {
        currentItemCount = 0;
    }
}
public interface IHeapItem<T>: IComparable<T>
{
    public int heapIndex { get; set; } // position in the heap
}
