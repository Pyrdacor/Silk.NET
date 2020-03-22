using System;
using System.Collections.Generic;

namespace Silk.NET.UI.Renderer.OpenGL
{
    internal class IndexPool
    {
        readonly List<int> releasedIndices = new List<int>();
        int firstFree = 0;

        public int AssignNextFreeIndex(out bool reused)
        {
            if (releasedIndices.Count > 0)
            {
                reused = true;

                int index = releasedIndices[0];

                releasedIndices.RemoveAt(0);

                return index;
            }

            reused = false;

            if (firstFree == int.MaxValue)
            {
                throw new Exceptions.InsufficientResourcesException("No free index available.");
            }

            return firstFree++;
        }

        public void UnassignIndex(int index)
        {
            releasedIndices.Add(index);
        }

        public bool AssignIndex(int index)
        {
            if (releasedIndices.Contains(index))
            {
                releasedIndices.Remove(index);
                return true;
            }

            if (index == firstFree)
                ++firstFree;

            return false;
        }
    }
}
