using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Heap<T> where T : IHeapItem<T>
{

	T[] items; //List of items in heap
	int currentItemCount; //Current count of items in heap list
	private int threadIndex;
	public Heap(int maxHeapSize, int _threadIndex) //Give the max possible size of the heap list to create the array
	{
		items = new T[maxHeapSize];
		threadIndex = _threadIndex;

	}

	public void Add(T item) //Add new item to the heap list
	{
		item.HeapIndex[threadIndex] = currentItemCount; //Set the heap index of the item
		items[currentItemCount] = item; //Put the item in the list
		SortUp(item); //Sort the item up
		currentItemCount++; //+1 to item count
	}

	public T RemoveFirst() //Remove the first item on the list
	{
		T firstItem = items[0]; //Get the first item
		currentItemCount--; //-1 to item count
		items[0] = items[currentItemCount]; //Add the last item in the list as the first item
		items[0].HeapIndex[threadIndex] = 0; //Change the index of the new first item as 0
		SortDown(items[0]); //Sort down the list
		return firstItem;
	}

	public void UpdateItem(T item)
	{
		SortUp(item);
	}

	public int Count //Return the active item count in the list. Not the total items but those currently in use.
	{
		get
		{
			return currentItemCount;
		}
	}

	public bool Contains(T item) //Is this item in the heap list?
	{
		return Equals(items[item.HeapIndex[threadIndex]], item);
	}

	void SortUp(T item)
	{
		int parentIndex = (item.HeapIndex[threadIndex] - 1) / 2; //Get the parent's heap index with this formula

		while (true) //This loop with iterate until there are no items left to swap
		{
			T parentItem = items[parentIndex]; //Get the parent object with index

			if (item.CompareTo(parentItem, threadIndex) > 0) //Check if left is Lower = 1, Equal = 0, Higher = -1 than the right side
			{
				Swap(item, parentItem); //Swap the item if it is lower than the parent
			}
			else
			{
				break; //Leave the loop if the item index is not lower than the parent's
			}

			parentIndex = (item.HeapIndex[threadIndex] - 1) / 2; //Get the parent index for next iteration
		}
	}

	void SortDown(T item)
	{
		while (true)
		{
			int childIndexLeft = item.HeapIndex[threadIndex] * 2 + 1; //Left child formula
			int childIndexRight = item.HeapIndex[threadIndex] * 2 + 2; //Right child formula
			int swapIndex = 0; //Index to swap with

			if (childIndexLeft < currentItemCount) //If left child is in the boundaries of the list 
			{
				swapIndex = childIndexLeft; //Left is the one to swap first

				if (childIndexRight < currentItemCount) //Check if right is in the boundaries
				{
					if (items[childIndexLeft].CompareTo(items[childIndexRight], threadIndex) < 0) //If left is higher than the right child
					{
						swapIndex = childIndexRight; //Then take the right child to swap
					}
				}

				if (item.CompareTo(items[swapIndex], threadIndex) < 0) //Check if the chosen child is lower than it's parent
				{
					Swap(item, items[swapIndex]); //Swap the items if it is true
				}
				else
				{
					return;
				}

			}
			else
			{
				return;
			}

		}
	}


	void Swap(T itemA, T itemB) //Swap the place of two items in the list
	{
		items[itemA.HeapIndex[threadIndex]] = itemB;
		items[itemB.HeapIndex[threadIndex]] = itemA;
		int itemAIndex = itemA.HeapIndex[threadIndex];
		itemA.HeapIndex[threadIndex] = itemB.HeapIndex[threadIndex];
		itemB.HeapIndex[threadIndex] = itemAIndex;
	}
}

public interface IHeapItem<T> : IComparable<T>
{
	int[] HeapIndex
	{
		get;
		set;
	}
}

public interface IComparable<T>
{

	int CompareTo(T otherCell, int _threadIndex);

}
