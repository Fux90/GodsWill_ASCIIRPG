using GodsWill_ASCIIRPG.View;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodsWill_ASCIIRPG.Model
{
    public abstract class Animation
    {
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

            public Frame(List<FrameItem> fItems)
            {
                frameItems = fItems;
            }
        }

        List<IAnimationViewer> animationViewers;
        List<Frame> frames;

        public Animation()
        {
            animationViewers = new List<IAnimationViewer>();
            frames = new List<Frame>();
        }

        protected void AddFrame(Frame frame)
        {
            frames.Add(frame);
        }

        public void Play()
        {
            foreach (var frame in frames)
            {
                foreach (var viewer in animationViewers)
                {
                    viewer.PlayFrame(frame);
                }
            }
        }
    }
}
