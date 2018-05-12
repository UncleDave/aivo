using System.Linq;

namespace AivoTree
{
    public class SelectorNode<T> : ITreeNode<T>
    {
        private readonly ITreeNode<T>[] _nodes;
        private ITreeNode<T> _runningNode;

        public SelectorNode(params ITreeNode<T>[] nodes)
        {
            _nodes = nodes;
        }
        
        public AivoTreeStatus Tick(float timeTick, T context)
        {
            var nodesToSearch = _runningNode == null
                ? _nodes
                : _nodes.SkipWhile(node => node != _runningNode);

            return nodesToSearch.Aggregate(AivoTreeStatus.Failure, (acc, curr) =>
            {
              if (acc == AivoTreeStatus.Success)
                return acc;

              var result = curr.Tick(timeTick, context);

              _runningNode = result == AivoTreeStatus.Running ? curr : null;

              return result;
            });
        }
    }
}