namespace AivoTree
{
    public interface ITreeNode<T>
    {
        AivoTreeStatus Tick(float timeTick, T context);
    }
}