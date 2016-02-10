using GodsWill_ASCIIRPG.Model.Core;
using GodsWill_ASCIIRPG.View;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model
{
    public abstract class Animation : IEnumerable
    {
        public const int _InterFrameDelay = 70;

        public class FrameItem
        {
            public string Symbol { get; private set; }
            public Color Color { get; private set; }
            public Coord Position { get; private set; }

            public FrameItem(string symbol, Color color, Coord position)
            {
                this.Symbol = symbol;
                this.Color = color;
                this.Position = position;
            }
        }

        protected class FrameBuilder
        {
            List<FrameItem> frameItems;

            public FrameBuilder()
            {
                frameItems = new List<FrameItem>();
            }

            public void AddFrameItem(FrameItem fItem)
            {
                frameItems.Add(fItem);
            }

            public void AddFrameItem(string symbol, Color color, Coord pos)
            {
                frameItems.Add(new FrameItem(symbol, color, pos));
            }

            public void AddFrameItem(string symbol, Color color, List<Coord> positions)
            {
                positions.ForEach( pos => frameItems.Add(new FrameItem(symbol, color, pos)));
            }

            public Frame Build()
            {
                return new Frame(frameItems);
            }

            public void Clear()
            {
                frameItems.Clear();
            }
        }

        public class Frame
        {
            List<FrameItem> frameItems;

            public FrameItem this[int index]
            {
                get { return frameItems[index]; }
            }

            public Frame(List<FrameItem> fItems)
            {
                frameItems = new List<FrameItem>(fItems.Count);
                frameItems.AddRange(fItems);
            }

            public void ForEach(Action<FrameItem> action)
            {
                frameItems.ForEach(fI => action(fI));
            }
        }

        static List<IAnimationViewer> animationViewers = new List<IAnimationViewer>();
        List<Frame> frames;

        public Animation()
        {
            frames = new List<Frame>();
        }

        protected void AddFrame(Frame frame)
        {
            frames.Add(frame);
        }

        public void Play()
        {
            foreach (var viewer in animationViewers)
            {
                viewer.PlayAnimation(this);
            }
        }

        public static void RegisterAnimationViewer(IAnimationViewer view)
        {
            animationViewers.AddOnce(view);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        public AnimationEnum GetEnumerator()
        {
            return new AnimationEnum(frames.ToArray());
        }
    }

    public class AnimationEnum : IEnumerator
    {
        Animation.Frame[] frames;

        // Enumerators are positioned before the first element
        // until the first MoveNext() call.
        int position = -1;

        public AnimationEnum(Animation.Frame[] list)
        {
            frames = list;
        }

        public bool MoveNext()
        {
            position++;
            return (position < frames.Length);
        }

        public void Reset()
        {
            position = -1;
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public Animation.Frame Current
        {
            get
            {
                try
                {
                    return frames[position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }
}
