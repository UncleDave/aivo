﻿using System;

namespace AivoTree
{
    public class ActionNode<T> : ITreeNode<T>
    {
        private readonly Func<float, T, AivoTreeStatus> _fn;

        public ActionNode(Func<float, T, AivoTreeStatus> fn)
        {
            _fn = fn;
        }
        
        public AivoTreeStatus Tick(float timeTick, T context)
        {
            return _fn(timeTick, context);
        }
    }
}