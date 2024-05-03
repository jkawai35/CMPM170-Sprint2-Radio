using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimumBinaryHeap<V>
{
	private KVPair[] array;
	private int maxCount;
	public int Count;
	
	public MinimumBinaryHeap(int size) {
		array = new KVPair[size + 1];
		maxCount = size;
		Count = 0;
	}

	public void Add(int key, V value) {
		if(Count < maxCount) {
			Count++;
			array[Count] = new KVPair(key, value);
			int currentI = Count;
			while(currentI > 1) {
				int parentI = currentI / 2;
				if(array[currentI].key < array[parentI].key) {
					Swap(currentI, parentI);
					currentI = parentI;
				} else {
					break;
				}
			}
		}
	}

	public V ExtractMin() {
		if(Count > 0) {
			V min = array[1].value;

			array[1] = array[Count];
			Count--;
			int currentI = 1;
			while(currentI < Count) {
				int smallestI = currentI;
				int l = currentI * 2;
				int r = currentI * 2 + 1;
				if(l <= Count && array[l].key < array[smallestI].key) {
					smallestI = l;
				}
				if(r <= Count && array[r].key < array[smallestI].key) {
					smallestI = r;
				}
				if(smallestI != currentI) {
					Swap(currentI, smallestI);
					currentI = smallestI;
				} else {
					break;
				}
			}
			return min;
		} else {
			throw new System.Exception("Cannot extract min of empty heap");
		}
	}

	private void Swap(int a, int b) {
		KVPair temp = array[a];
		array[a] = array[b];
		array[b] = temp;
	}

	struct KVPair {
		public readonly int key;
		public readonly V value;

		public KVPair(int key, V value) {
			this.key = key;
			this.value = value;
		}
	}

}
