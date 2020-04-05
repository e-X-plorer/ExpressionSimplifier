using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CalculatorSimplifier.Tests
{
    public class ExpressionBlockTests
    {
        [Theory]
        [InlineData(-1)]
        [InlineData(Settings.MaximumPriority + 1)]
        public void Init_PriorityOutOfRange_Exception(int priority)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ExpressionBlock(priority));
        }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(1, 1)]
        public void Init_InvalidIEnumerableCount_Exception(int nodesCount, int operationsCount)
        {
            Assert.Throws<ArgumentException>(() =>
                new ExpressionBlock(0, new List<IExpression>(nodesCount), new List<Operation>(operationsCount)));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void Init_WithValidPriority_CheckState(int priority)
        {
            var expressionBlock = new ExpressionBlock(priority);

            Assert.Equal(priority, expressionBlock.Priority);
            Assert.False(expressionBlock.IsAtomic);
            Assert.False(expressionBlock.IsVariable);
            Assert.False(expressionBlock.IsFilled);
            Assert.Empty(expressionBlock.Nodes);
            Assert.Single(expressionBlock.Operations);
        }

        [Fact]
        public void AppendNode_AppendToSelf_Exception()
        {
            var expressionBlock = new ExpressionBlock(0);

            Assert.Throws<InvalidOperationException>(() => expressionBlock.AppendNode(expressionBlock));
        }

        [Fact]
        public void AppendNode_AppendToFilled_Exception()
        {
            var parentExpressionBlock = new ExpressionBlock(0);
            var firstChild = new Number(1);
            var secondChild = new Number(2);
            parentExpressionBlock.AppendNode(firstChild);

            Assert.Throws<InvalidOperationException>(() => parentExpressionBlock.AppendNode(secondChild));
        }

        [Fact]
        public void AppendNode_AppendNodeWithParent_Exception()
        {
            var parentExpressionBlock = new ExpressionBlock(0);
            var child = new Number(1);
            parentExpressionBlock.AppendNode(child);

            Assert.Throws<InvalidOperationException>(() => parentExpressionBlock.AppendNode(child));
        }

        [Fact]
        public void AppendNode_AppendNext_AppendedAndAddedParent()
        {
            var parentExpressionBlock = new ExpressionBlock(0);
            var childExpressionBlock = new ExpressionBlock(1);
            childExpressionBlock.AppendNode(new Number("a"));
            parentExpressionBlock.AppendNode(childExpressionBlock);

            Assert.Single(parentExpressionBlock.Nodes);
            Assert.True(childExpressionBlock == parentExpressionBlock.Nodes.ElementAt(0));
            Assert.True(childExpressionBlock.Parent == parentExpressionBlock);
            Assert.True(childExpressionBlock.IsVariable && parentExpressionBlock.IsVariable);
        }

        [Fact]
        public void AppendOperation_AppendToNotFilled_Exception()
        {
            var expressionBlock = new ExpressionBlock(0);

            Assert.Throws<InvalidOperationException>(() => expressionBlock.AppendOperation(new Addition()));
        }

        [Fact]
        public void AppendOperation_AppendSeveral_Appended()
        {
            var parentExpressionBlock = new ExpressionBlock(0);
            
            parentExpressionBlock.AppendNode(new ExpressionBlock(1));
            parentExpressionBlock.AppendOperation(new Addition());
            parentExpressionBlock.AppendNode(new ExpressionBlock(1));
            parentExpressionBlock.AppendOperation(new Subtraction());
            parentExpressionBlock.AppendNode(new ExpressionBlock(1));
            parentExpressionBlock.AppendOperation(new Multiplication());
            parentExpressionBlock.AppendNode(new ExpressionBlock(1));
            
            Assert.False(parentExpressionBlock.ContainsDivision);

            parentExpressionBlock.AppendOperation(new Division());

            Assert.True(parentExpressionBlock.ContainsDivision);
            Assert.Equal(5, parentExpressionBlock.Operations.Count());
        }

        [Fact]
        public void GetNumericValue_SingleNode_Value()
        {
            var parent = new ExpressionBlock(1);
            parent.AppendNode(new Number(5));

            Assert.Equal(5, parent.GetNumericValue(true));
            Assert.Equal(5, parent.GetNumericValue(false));
        }

        [Fact]
        public void GetNumericValue_MultipleNodesVariable_Value()
        {
            var parent = new ExpressionBlock(1);
            parent.AppendNode(new Number(10));
            parent.AppendOperation(new Division());
            parent.AppendNode(new Number(5));
            parent.AppendOperation(new Multiplication());
            parent.AppendNode(new Number(4));
            parent.AppendOperation(new Multiplication());
            parent.AppendNode(new Number("a"));

            Assert.Equal(8, parent.GetNumericValue(true));
            Assert.Equal(40, parent.GetNumericValue(false));
        }

        [Fact]
        public void GetNumericValue_MultipleNodesHierarchy_Value()
        {
            var parent = new ExpressionBlock(1);
            parent.AppendNode(new Number(10));
            parent.AppendOperation(new Division());
            parent.AppendNode(new Number(5));
            parent.AppendOperation(new Multiplication());
            parent.AppendNode(new Number(4));
            parent.AppendOperation(new Multiplication());
            parent.AppendNode(new Number("a"));

            var firstChild = new ExpressionBlock(2);
            firstChild.AppendNode(new Number(2));
            firstChild.AppendOperation(new Addition());
            firstChild.AppendNode(new Number(2));

            var secondChild = new ExpressionBlock(3);
            secondChild.AppendNode(new Number(10));
            secondChild.AppendOperation(new Division());
            secondChild.AppendNode(new Number("b"));

            parent.AppendOperation(new Division());
            parent.AppendNode(firstChild);
            parent.AppendOperation(new Multiplication());
            parent.AppendNode(secondChild);

            Assert.Equal(2, parent.GetNumericValue(true));
            Assert.Equal(40, parent.GetNumericValue(false));
        }

        [Fact]
        public void MergeChildren_SingleNode_Merge()
        {
            var parent = new ExpressionBlock(0);
            var child = new ExpressionBlock(2);
            var leaf = new Number(1);

            child.AppendNode(leaf);
            parent.AppendNode(child);

            parent.MergeChildren();

            Assert.Equal(leaf, parent.Nodes.ElementAt(0));
            Assert.Equal(parent, leaf.Parent);
        }

        [Fact]
        public void MergeChildren_SeveralNodes_NoMerge()
        {
            var parent = new ExpressionBlock(0);
            var child = new ExpressionBlock(2);
            var firstLeaf = new Number(1);
            var secondLeaf = new Number(2);

            child.AppendNode(firstLeaf);
            child.AppendOperation(new Addition());
            child.AppendNode(secondLeaf);
            parent.AppendNode(child);

            parent.MergeChildren();

            Assert.Equal(parent, child.Parent);
            Assert.Single(parent.Nodes);
        }

        [Fact]
        public void SimplifiedRepresentation_NoVariable_CorrectString()
        {
            var expressionBlock = new ExpressionBlock(1);
            expressionBlock.AppendNode(new Number(1));
            expressionBlock.AppendOperation(new Division());
            expressionBlock.AppendNode(new Number(2));
            expressionBlock.AppendOperation(new Multiplication());
            expressionBlock.AppendNode(new Number(3));
            expressionBlock.AppendOperation(new Multiplication());
            expressionBlock.AppendNode(new Number(4));

            Assert.Equal("6", expressionBlock.SimplifiedRepresentation(true, true));
            Assert.Equal("12 / 2", expressionBlock.SimplifiedRepresentation(false, true));
        }

        [Fact]
        public void SimplifiedRepresentation_WithVariable_CorrectString()
        {
            var expressionBlock = new ExpressionBlock(1);
            expressionBlock.AppendNode(new Number(1));
            expressionBlock.AppendOperation(new Division());
            expressionBlock.AppendNode(new Number("a"));
            expressionBlock.AppendOperation(new Multiplication());
            expressionBlock.AppendNode(new Number(3));
            expressionBlock.AppendOperation(new Multiplication());
            expressionBlock.AppendNode(new Number(4));

            Assert.Equal("12 / a", expressionBlock.SimplifiedRepresentation(true, true));
            Assert.Equal("12 / a", expressionBlock.SimplifiedRepresentation(false, true));
        }

        [Fact]
        public void SimplifiedRepresentation_Hierarchy_CorrectString()
        {
            var expressionBlock = new ExpressionBlock(1);
            expressionBlock.AppendNode(new Number(1));
            expressionBlock.AppendOperation(new Division());

            var child1 = new ExpressionBlock(2);
            child1.AppendNode(new Number(2));
            child1.AppendOperation(new Subtraction());
            child1.AppendNode(new Number("b"));

            expressionBlock.AppendNode(child1);
            expressionBlock.AppendOperation(new Multiplication());

            var child2 = new ExpressionBlock(3);
            child2.AppendNode(new Number(3));
            child2.AppendOperation(new Multiplication());
            child2.AppendNode(new Number(4));

            expressionBlock.AppendNode(child2);
            expressionBlock.AppendOperation(new Division());
            expressionBlock.AppendNode(new Number(2));

            Assert.Equal("6 / (2 - b)", expressionBlock.SimplifiedRepresentation(true, true));
            Assert.Equal("12 / (2 - b) / 2", expressionBlock.SimplifiedRepresentation(false, true));
        }
    }
}
