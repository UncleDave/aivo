using AivoTree;
using NUnit.Framework;

namespace Test
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void RunsActionNode()
        {
            var model = new MyModel();
            var action = new ActionNode<MyModel>((frame, ctx) =>
            {
                ctx.value = frame;
                return AivoTreeStatus.Success;
            });
            Assert.AreEqual(AivoTreeStatus.Success, action.Tick(1, model));
            Assert.AreEqual(1, model.value);
        }
        
        [Test]
        public void SequenceNodeFailsWithFirstFailure()
        {
            var model = new MyModel();
            var sequence = new SequenceNode<MyModel>(new SucceedingNode(), new FailingNode(), new ShouldNotRunNode());
            Assert.AreEqual(AivoTreeStatus.Failure, sequence.Tick(1, model));
        }
        
        [Test]
        public void SequenceNodeSucceedsIfAllNodesSucceed()
        {
            var model = new MyModel();
            var sequence = new SequenceNode<MyModel>(new SucceedingNode(), new SucceedingNode());
            Assert.AreEqual(AivoTreeStatus.Success, sequence.Tick(1, model));
        }
        
        [Test]
        public void SequenceNodeContinuesWithCurrentlyRunningNodeWithNextTick()
        {
            var model = new MyModel();
            var sequence = new SequenceNode<MyModel>(new SucceedOnceNode(), new RunOnceAndSucceedNextNode(), new FailingNode());
            Assert.AreEqual(AivoTreeStatus.Running, sequence.Tick(1, model));
            Assert.AreEqual(AivoTreeStatus.Failure, sequence.Tick(2, model));
        }
        
        [Test]
        public void SelectorNodeSucceedSWithFirstSucceedingNode()
        {
            var model = new MyModel();
            var sequence = new SelectorNode<MyModel>(new SucceedingNode(), new ShouldNotRunNode());
            Assert.AreEqual(AivoTreeStatus.Success, sequence.Tick(1, model));
        }
        
        [Test]
        public void SelectorNodeFailsIfAllNodesFails()
        {
            var model = new MyModel();
            var sequence = new SelectorNode<MyModel>(new FailingNode());
            Assert.AreEqual(AivoTreeStatus.Failure, sequence.Tick(1, model));
        }
        
        [Test]
        public void SelectorNodeContinuesWithCurrentlyRunningNodeWithNextTick()
        {
            var model = new MyModel();
            var sequence = new SelectorNode<MyModel>(new FailOnceNode(), new RunOnceAndSucceedNextNode());
            Assert.AreEqual(AivoTreeStatus.Running, sequence.Tick(1, model));
            Assert.AreEqual(AivoTreeStatus.Success, sequence.Tick(2, model));
        }

        [Test]
        public void InverterNodeInvertsFailSuccessStateTheResul()
        {
            Assert.AreEqual(AivoTreeStatus.Success, new InverterNode<MyModel>(new FailingNode()).Tick(1, new MyModel()));
            Assert.AreEqual(AivoTreeStatus.Failure, new InverterNode<MyModel>(new SucceedingNode()).Tick(1, new MyModel()));
            Assert.AreEqual(AivoTreeStatus.Running, new InverterNode<MyModel>(new RunOnceAndSucceedNextNode()).Tick(1, new MyModel()));
        }
    }

    public class MyModel
    {
        public float value;
    }

    public class FailingNode : ITreeNode<MyModel>
    {
        public AivoTreeStatus Tick(float timeTick, MyModel context)
        {
            return AivoTreeStatus.Failure;
        }
    }
    
    public class SucceedingNode : ITreeNode<MyModel>
    {
        public AivoTreeStatus Tick(float timeTick, MyModel context)
        {
            return AivoTreeStatus.Success;
        }
    }
    
    public class RunOnceAndSucceedNextNode : ITreeNode<MyModel>
    {
        private bool alreadyRun;
        
        public AivoTreeStatus Tick(float timeTick, MyModel context)
        {
            if (!alreadyRun)
            {
                alreadyRun = true;
                return AivoTreeStatus.Running;
            }
            return AivoTreeStatus.Success;
        }
    }
    
    public class FailOnceNode : ITreeNode<MyModel>
    {
        private bool alreadyRun;
        
        public AivoTreeStatus Tick(float timeTick, MyModel context)
        {
            if (!alreadyRun)
            {
                alreadyRun = true;
                return AivoTreeStatus.Failure;
            }
            throw new AssertionException("Should not be called");
        }
    }
    
    public class SucceedOnceNode : ITreeNode<MyModel>
    {
        private bool alreadyRun;
        
        public AivoTreeStatus Tick(float timeTick, MyModel context)
        {
            if (!alreadyRun)
            {
                alreadyRun = true;
                return AivoTreeStatus.Success;
            }
            throw new AssertionException("Should not be called");
        }
    }
    
    public class ShouldNotRunNode : ITreeNode<MyModel>
    {
        public AivoTreeStatus Tick(float timeTick, MyModel context)
        {
            throw new AssertionException("Should not be called");
        }
    }
}