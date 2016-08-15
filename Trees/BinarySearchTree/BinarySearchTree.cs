using System;
using System.Collections.Generic;
using System.Linq;

namespace Trees.BinarySearchTree
{
	public class BinarySearchTree<T> where T : IComparable
	{
		/// <summary>
		/// The root node of the BST.
		/// </summary>
		public Node<T> Root { get; private set; }

		/// <summary>
		/// Create an empty BST.
		/// </summary>
		public BinarySearchTree() { }

		/// <summary>
		/// Creaate a new BST with a root node.
		/// </summary>
		/// <param name="rootValue">The value for the root node.</param>
		public BinarySearchTree(T rootValue)
		{
			this.Root = new Node<T>(rootValue);
		}

		/// <summary>
		/// Create a new BST populated with values.
		/// </summary>
		/// <param name="values">The values to insert into the new BST.</param>
		public BinarySearchTree(IEnumerable<T> values)
		{
			foreach(T val in values)
			{
				this.Insert(val);
			}
		}

		/// <summary>
		/// Create and insert a new value into the BST.
		/// </summary>
		/// <param name="value">The value to insert.</param>
		public void Insert(T value)
		{
			if (this.Root == null)
			{
				this.Root = new Node<T>(value);
			}
			else
			{
				Insert(this.Root, value);
			}
		}

		/// <summary>
		/// Deletes a value from the BST, if it exists.
		/// </summary>
		/// <param name="value">The value to delete.</param>
		public void Delete(T value)
		{
			Node<T> nodeToDelete = null;
			Node<T> currentParent = null;

			// Find the node to delete.
			var currentNode = this.Root;
			while (currentNode != null && nodeToDelete == null)
			{
				if (currentNode.Value.Equals(value))
				{
					nodeToDelete = currentNode;
				}
				else
				{
					currentParent = currentNode;
					currentNode = value.CompareTo(currentNode.Value) < 0 ? currentNode.LeftChild : currentNode.RightChild;
				}
			}

			if (nodeToDelete == null)
			{
				// Node wasn't found.
				throw new InvalidOperationException("Deletion failed. Could not find the node in the tree.");
			}

			// Now that the node has been found, do the deletion.
			if (nodeToDelete.LeftChild == null && nodeToDelete.RightChild == null)
			{
				DeleteLeafNode(nodeToDelete, currentParent);
			}
			else if (nodeToDelete.LeftChild == null || nodeToDelete.RightChild == null)
			{
				DeleteNodeWithOneChild(nodeToDelete, currentParent);
			}
			else
			{
				DeleteNodeWithTwoChildren(nodeToDelete, currentParent);
			}
		}

		/// <summary>
		/// Search the BST for the specified value.
		/// </summary>
		/// <param name="value">The value to search for.</param>
		/// <returns>A boolean indicated whether the value was found.</returns>
		public bool Search(T value)
		{
			var currentNode = this.Root;
			while (currentNode != null)
			{
				if(currentNode.Value.Equals(value))
				{
					return true;
				}

				currentNode = value.CompareTo(currentNode.Value) < 0 ? currentNode.LeftChild : currentNode.RightChild;
			}

			return false;
		}

		/// <summary>
		/// The pre-ordering of the tree.
		/// </summary>
		/// <returns>A comma separated pre-ordering of the tree.</returns>
		public string PreOrder()
		{
			return PreOrder(this.Root);
		}

		/// <summary>
		/// The in-ordering of the tree.
		/// </summary>
		/// <returns>A comma separated in-ordering of the tree.</returns>
		public string InOrder()
		{
			return InOrder(this.Root);
		}

		/// <summary>
		/// The post-ordering of the tree.
		/// </summary>
		/// <returns>A comma separated post-ordering of the tree.</returns>
		public string PostOrder()
		{
			return PostOrder(this.Root);
		}

		#region Private Helpers

		private static void Insert(Node<T> node, T value)
		{
			if (value.CompareTo(node.Value) == 0)
			{
				throw new InvalidOperationException("Cannot insert node into tree. Value already exists.");
			}

			if (value.CompareTo(node.Value) < 0)
			{
				// Add node to the left.
				if (node.LeftChild == null)
				{
					node.LeftChild = new Node<T>(value);
				}
				else
				{
					Insert(node.LeftChild, value);
				}
			}
			else
			{
				// Add node to the right.
				if (node.RightChild == null)
				{
					node.RightChild = new Node<T>(value);
				}
				else
				{
					Insert(node.RightChild, value);
				}
			}
		}

		private void DeleteLeafNode(Node<T> nodeToDelete, Node<T> nodeToDeleteParent)
		{
			if (nodeToDeleteParent == null)
			{
				// Deleting the root.
				this.Root = null;
			}
			else if (IsLeftChild(nodeToDeleteParent, nodeToDelete))
			{
				nodeToDeleteParent.LeftChild = null;
			}
			else
			{
				nodeToDeleteParent.RightChild = null;
			}
		}

		private void DeleteNodeWithOneChild(Node<T> nodeToDelete, Node<T> nodeToDeleteParent)
		{
			var childNode = nodeToDelete.LeftChild ?? nodeToDelete.RightChild;

			if (nodeToDeleteParent == null)
			{
				// Deleting the root.
				this.Root = childNode;
			}
			else if (IsLeftChild(nodeToDeleteParent, nodeToDelete))
			{
				nodeToDeleteParent.LeftChild = childNode;
			}
			else
			{
				nodeToDeleteParent.RightChild = childNode;
			}
		}

		private void DeleteNodeWithTwoChildren(Node<T> nodeToDelete, Node<T> nodeToDeleteParent)
		{
			var replacementNode = MinimumChild(nodeToDelete.RightChild) ?? nodeToDelete.RightChild;

			// Reassign children to replacement node, avoiding any circular references.
			replacementNode.RightChild = (replacementNode != nodeToDelete.RightChild) ? nodeToDelete.RightChild : null;
			replacementNode.LeftChild = (replacementNode != nodeToDelete.LeftChild) ? nodeToDelete.LeftChild : null;

			if (nodeToDeleteParent == null)
			{
				// Deleting the root.
				this.Root = replacementNode;
			}
			else if (IsLeftChild(nodeToDeleteParent, nodeToDelete))
			{
				nodeToDeleteParent.LeftChild = replacementNode;
			}
			else
			{
				nodeToDeleteParent.RightChild = replacementNode;
			}
		}

		// Returns minimum-valued node under the given node, and then removes the reference to that node. Useful for node deletion.
		private static Node<T> MinimumChild(Node<T> node)
		{
			Node<T> next = null;
			Node<T> parent = null;

			while (node != null)
			{
				if (node.LeftChild != null)
				{
					parent = node;
					next = node.LeftChild;
				}

				node = node.LeftChild;
			}

			if (parent != null)
			{
				parent.LeftChild = null;
			}

			return next;
		}

		private static bool IsLeftChild(Node<T> parent, Node<T> child)
		{
			return parent.LeftChild != null && parent.LeftChild.Value.Equals(child.Value);
		}

		private static string InOrder(Node<T> node)
		{
			return node == null ? 
				string.Empty :
				string.Join(", ", new[] {InOrder(node.LeftChild), node.ToString(), InOrder(node.RightChild)}.Where(s => s != string.Empty));
		}

		private static string PreOrder(Node<T> node)
		{
			return node == null ? 
				string.Empty : 
				string.Join(", ", new[] { node.ToString(), PreOrder(node.LeftChild), PreOrder(node.RightChild) }.Where(s => s != string.Empty));
		}

		private static string PostOrder(Node<T> node)
		{
			return node == null ? 
				string.Empty : 
				string.Join(", ", new[] { PostOrder(node.LeftChild), PostOrder(node.RightChild), node.ToString()}.Where(s => s != string.Empty));
		}
		#endregion
	}
}
