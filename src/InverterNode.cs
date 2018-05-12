namespace AivoTree
{
    public class InverterNode<T> : ITreeNode<T>
    {
        private readonly ITreeNode<T> _node;

        public InverterNode(ITreeNode<T> node)
        {
            _node = node;
        }
        
        public AivoTreeStatus Tick(float timeTick, T context)
        {
            var status = _node.Tick(timeTick, context);

            switch (status)
            {
                case AivoTreeStatus.Success:
                    return AivoTreeStatus.Failure;
                case AivoTreeStatus.Failure:
                    return AivoTreeStatus.Success;
                default:
                    return status;
            }
        }
    }
}