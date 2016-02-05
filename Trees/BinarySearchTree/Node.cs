using System;

namespace Trees.BinarySearchTree
{
	public class Node<T> where T : IComparable
	{
		public T Value { get; private set; }
		public Node<T> LeftChild { get; set; }
		public Node<T> RightChild { get; set; }

		public Node(T value)
		{
			this.Value = value;
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}
	}
}
