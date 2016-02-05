using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trees.BinarySearchTree;

namespace TreesTests
{
	[TestClass]
	public class BinaryTreeTests
	{
		private BinarySearchTree<int> testTree;

		[TestInitialize]
		public void TestInit()
		{
			testTree = new BinarySearchTree<int>();
		}

		#region Constructor Tests

		[TestMethod]
		public void BinarySearchTree_InitWithValue_HasRoot()
		{
			testTree = new BinarySearchTree<int>(0);
			Assert.AreEqual(0, testTree.Root.Value);
		}

		[TestMethod]
		public void BinarySearchTree_InitWithValues_IsInOrder()
		{
			testTree = new BinarySearchTree<int>( new []{ 50, 60, 40 });
			Assert.AreEqual(50, testTree.Root.Value);
			Assert.AreEqual(40, testTree.Root.LeftChild.Value);
			Assert.AreEqual(60, testTree.Root.RightChild.Value);
		}

		#endregion


		#region Insert Node Tests

		[TestMethod]
		public void Insert_EmptyTree_AddsRootNode()
		{
			Assert.IsNull(testTree.Root);
			testTree.Insert(1);

			Assert.IsNotNull(testTree.Root);
			Assert.IsNull(testTree.Root.LeftChild);
			Assert.IsNull(testTree.Root.RightChild);
		}

		[TestMethod]
		public void Insert_NodeLessThanRoot_AddsToLeftSubtree()
		{
			testTree.Insert(10);
			testTree.Insert(5);

			Assert.IsNotNull(testTree.Root.LeftChild);
			Assert.IsNull(testTree.Root.RightChild);
		}

		[TestMethod]
		public void Insert_NodeGreaterThanRoot_AddsToRightSubtree()
		{
			testTree.Insert(10);
			testTree.Insert(15);

			Assert.IsNull(testTree.Root.LeftChild);
			Assert.IsNotNull(testTree.Root.RightChild);
		}

		[TestMethod]
		public void Insert_NodeLessThanNestedNode_AddsToLeftSubtree()
		{
			const int NEW_NODE_VAL = 12;
			testTree.Insert(10);
			testTree.Insert(5);
			testTree.Insert(15);

			testTree.Insert(NEW_NODE_VAL);

			var newNode = testTree.Root.RightChild.LeftChild;
			Assert.IsNotNull(newNode);
			Assert.AreEqual(newNode.Value, NEW_NODE_VAL);
		}

		[TestMethod]
		public void Insert_NodeGreaterThanNestedNode_AddsToRightSubtree()
		{
			const int NEW_NODE_VAL = 7;
			testTree.Insert(10);
			testTree.Insert(5);
			testTree.Insert(15);

			testTree.Insert(NEW_NODE_VAL);

			var newNode = testTree.Root.LeftChild.RightChild;
			Assert.IsNotNull(newNode);
			Assert.AreEqual(newNode.Value, NEW_NODE_VAL);
		}

		#endregion


		#region Search Tests

		[TestMethod]
		public void Search_NodeExistsOnRightSubtree_ReturnsTrue()
		{
			testTree.Insert(1);
			testTree.Insert(10);

			Assert.IsTrue(testTree.Search(10));
		}

		[TestMethod]
		public void Search_NodeExistsOnLeftSubtree_ReturnsTrue()
		{
			testTree.Insert(10);
			testTree.Insert(1);

			Assert.IsTrue(testTree.Search(1));
		}

		[TestMethod]
		public void Search_NodeDoesNotExist_ReturnsFalse()
		{
			testTree.Insert(10);
			testTree.Insert(5);
			testTree.Insert(15);

			Assert.IsFalse(testTree.Search(100));
		}

		[TestMethod]
		public void Search_EmptyTree_ReturnsFalse()
		{
			Assert.IsNull(testTree.Root);
			Assert.IsFalse(testTree.Search(1));
		}

		#endregion


		#region Delete Node Tests

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))] 
		public void Delete_NodeDoesNotExist_ThrowsException()
		{
			testTree.Insert(10);
			testTree.Delete(999);
		}

		[TestMethod]
		public void Delete_OnlyOneNode_DeletesRoot()
		{
			testTree.Insert(10);

			Assert.IsTrue(testTree.Search(10));
			testTree.Delete(10);
			Assert.IsFalse(testTree.Search(10));
			Assert.IsTrue(testTree.Root == null);
		}

		[TestMethod]
		public void Delete_NodeHasNoChildren_IsDeleted()
		{
			testTree.Insert(10);
			testTree.Insert(5);
			testTree.Insert(15);

			Assert.IsTrue(testTree.Search(15));
			testTree.Delete(15);
			Assert.IsFalse(testTree.Search(15));
		}

		[TestMethod]
		public void Delete_NodeHasLeftChild_IsDeleted()
		{
			testTree.Insert(10);
			testTree.Insert(5);
			testTree.Insert(15);
			testTree.Insert(1);

			Assert.IsTrue(testTree.Search(5));
			testTree.Delete(5);
			Assert.IsFalse(testTree.Search(5));
		}

		[TestMethod]
		public void Delete_NodeHasRightChild_IsDeleted()
		{
			testTree.Insert(10);
			testTree.Insert(5);
			testTree.Insert(15);
			testTree.Insert(20);

			Assert.IsTrue(testTree.Search(15));
			testTree.Delete(15);
			Assert.IsFalse(testTree.Search(15));
		}

		[TestMethod]
		public void Delete_NodeHasTwoChildren_IsDeleted()
		{
			testTree = new BinarySearchTree<int>(new[] { 5, 3, 2, 4, 7, 6, 8});

			Assert.IsTrue(testTree.Search(5));
			testTree.Delete(5);
			Assert.IsFalse(testTree.Search(5));
		}

		[TestMethod]
		public void Delete_NodeIsRootAndHasTwoChildren_HasCorrectOrder()
		{
			testTree = new BinarySearchTree<int>(new[] { 5, 3, 2, 4, 7, 6, 8 });

			testTree.Delete(5);
			Assert.IsTrue(IsCorrectOrder(testTree.PreOrder(), "632478"));
		}

		[TestMethod]
		public void Delete_NodeIsNotRootAndHasTwoChildren_HasCorrectOrder()
		{
			testTree = new BinarySearchTree<int>(new[] { 5, 3, 2, 4, 7, 6, 8 });

			testTree.Delete(7);
			Debug.WriteLine(testTree.PreOrder());
			Assert.IsTrue(IsCorrectOrder(testTree.PreOrder(), "532486"));
		}
		#endregion

		#region Print Order Tests

		[TestMethod]
		public void PreOrder_IsPreOrder()
		{
			CreateOrderedTree();

			string order = testTree.PreOrder();

			Debug.WriteLine("PreOrder Test: " + order);
			Assert.IsTrue(IsCorrectOrder(order,"621435798"));
		}

		[TestMethod]
		public void InOrder_IsInOrder()
		{
			CreateOrderedTree();

			string order = testTree.InOrder();

			Debug.WriteLine("InOrder Test: " + order);
			Assert.IsTrue(IsCorrectOrder(order,"123456789"));
		}

		[TestMethod]
		public void PostOrder_IsPostOrder()
		{
			CreateOrderedTree();
			string order = testTree.PostOrder();

			Debug.WriteLine("PostOrder Test: " + order);
			Assert.IsTrue(IsCorrectOrder(order, "135428976"));
		}

		private static bool IsCorrectOrder(string order, string expectedOrder)
		{
			return new string(order.Where(char.IsDigit).ToArray()).Equals(expectedOrder);
		}

		private void CreateOrderedTree()
		{
			testTree = new BinarySearchTree<int>(new []{6,2,1,4,3,5,7,9,8});
		}
		#endregion
	}
}
