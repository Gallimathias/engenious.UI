﻿using System;
using engenious.Graphics;
using engenious.Input;

namespace engenious.UI.Controls
{
    public class ScrollContainer : ContentControl
    {
        private bool horizontalScrollbarEnabled;

        private bool verticalScrollbarEnabled;

        private bool? horizontalScrollbarVisible;

        private bool? verticalScrollbarVisible;

        private Brush verticalScrollbarBackground;

        private Brush verticalScrollbarForeground;

        private Brush horizontalScrollbarBackground;

        private Brush horizontalScrollbarForeground;

        private Point scrollPosition = Point.Zero;

        private int scrollbarWidth;

        private int scrollerMinSize;

        private Point virtualSize;

        private readonly PropertyEventArgs<bool> _horizontalScrollbarEnabledChangedEventArgs = new PropertyEventArgs<bool>();

        #region Properties 
        /// <summary>
        /// Legt die Scrollgeschwindigkeit fest
        /// </summary>
        public int ScrollSpeed { get; set; } = 20;

        /// <summary>
        /// Gibt an, ob es eine horizontale Scrollbar geben soll.
        /// </summary>
        public bool HorizontalScrollbarEnabled
        {
            get { return horizontalScrollbarEnabled; }
            set
            {
                if (horizontalScrollbarEnabled == value) return;

                _horizontalScrollbarEnabledChangedEventArgs.OldValue = horizontalScrollbarEnabled;
                _horizontalScrollbarEnabledChangedEventArgs.NewValue = value;
                _horizontalScrollbarEnabledChangedEventArgs.Handled = false;

                horizontalScrollbarEnabled = value;
                InvalidateDimensions();

                OnHorizontalScrollbarEnabledChanged(_horizontalScrollbarEnabledChangedEventArgs);
                HorizontalScrollbarEnabledChanged?.Invoke(this, _horizontalScrollbarEnabledChangedEventArgs);
            }
        }
        private readonly PropertyEventArgs<bool> _verticalScrollbarEnabledChangedEventArgs = new PropertyEventArgs<bool>();
        /// <summary>
        /// Gibt an, ob es eine vertikale Scrollbar geben soll.
        /// </summary>
        public bool VerticalScrollbarEnabled
        {
            get { return verticalScrollbarEnabled; }
            set
            {
                if (verticalScrollbarEnabled == value) return;

                _verticalScrollbarEnabledChangedEventArgs.OldValue = verticalScrollbarEnabled;
                _verticalScrollbarEnabledChangedEventArgs.NewValue = value;
                _verticalScrollbarEnabledChangedEventArgs.Handled = false;

                verticalScrollbarEnabled = value;
                InvalidateDimensions();

                OnVerticalScrollbarEnabledChanged(_verticalScrollbarEnabledChangedEventArgs);
                VerticalScrollbarEnabledChanged?.Invoke(this, _verticalScrollbarEnabledChangedEventArgs);
            }
        }

        private readonly PropertyEventArgs<bool?> _horizontalScrollbarVisibleChangedEventArgs = new PropertyEventArgs<bool?>();
        /// <summary>
        /// Gibt die Sichtbarkeit der Scrollbar auf horizontaler Achse an oder legt 
        /// diese fest. null steht hier für "auto", sonfern eine Scrollbar 
        /// notwendig ist.
        /// </summary>
        public bool? HorizontalScrollbarVisible
        {
            get { return horizontalScrollbarVisible; }
            set
            {
                if (horizontalScrollbarVisible == value) return;

                _horizontalScrollbarVisibleChangedEventArgs.OldValue = horizontalScrollbarVisible;
                _horizontalScrollbarVisibleChangedEventArgs.NewValue = value;
                _horizontalScrollbarVisibleChangedEventArgs.Handled = false;

                horizontalScrollbarVisible = value;
                InvalidateDimensions();

                OnHorizontalScrollbarVisibleChanged(_horizontalScrollbarVisibleChangedEventArgs);
                HorizontalScrollbarVisibleChanged?.Invoke(this, _horizontalScrollbarVisibleChangedEventArgs);
            }
        }

        private readonly PropertyEventArgs<bool?> _verticalScrollbarVisibleChangedEventArgs = new PropertyEventArgs<bool?>();
        /// <summary>
        /// Gibt die Sichtbarkeit der Scrollbar auf vertikaler Achse an oder legt
        /// diese fest. null steht hier für "auto", sofern eine Scrollbar notwendig
        /// ist.
        /// </summary>
        public bool? VerticalScrollbarVisible
        {
            get { return verticalScrollbarVisible; }
            set
            {
                if (verticalScrollbarVisible == value) return;

                _verticalScrollbarVisibleChangedEventArgs.OldValue = verticalScrollbarVisible;
                _verticalScrollbarVisibleChangedEventArgs.NewValue = value;
                _verticalScrollbarVisibleChangedEventArgs.Handled = false;
                
                verticalScrollbarVisible = value;
                InvalidateDimensions();

                OnVerticalScrollbarVisibleChanged(_verticalScrollbarVisibleChangedEventArgs);
                VerticalScrollbarVisibleChanged?.Invoke(this, _verticalScrollbarVisibleChangedEventArgs);
            }
        }

        private readonly PropertyEventArgs<Point> _virtualSizeChangedEventArgs = new PropertyEventArgs<Point>();
        /// <summary>
        /// Gibt die Größe des virtuellen Client-Bereichs an.
        /// </summary>
        public Point VirtualSize
        {
            get { return virtualSize; }
            private set
            {
                if (virtualSize == value) return;

                _virtualSizeChangedEventArgs.OldValue = virtualSize;
                _virtualSizeChangedEventArgs.NewValue = value;
                _virtualSizeChangedEventArgs.Handled = false;

                virtualSize = value;

                OnVirtualSizeChanged(_virtualSizeChangedEventArgs);
                VirtualSizeChanged?.Invoke(this, _virtualSizeChangedEventArgs);
            }
        }

        /// <summary>
        /// Hintergrund-Brush für die vertikale Scrollbar
        /// </summary>
        public Brush VerticalScrollbarBackground
        {
            get
            {
                return verticalScrollbarBackground;
            }
            set
            {
                if (verticalScrollbarBackground != value)
                {
                    verticalScrollbarBackground = value;
                    InvalidateDrawing();
                }
            }
        }

        /// <summary>
        /// Brush für den vertikalen Scroller.
        /// </summary>
        public Brush VerticalScrollbarForeground
        {
            get
            {
                return verticalScrollbarForeground;
            }
            set
            {
                if (verticalScrollbarForeground != value)
                {
                    verticalScrollbarForeground = value;
                    InvalidateDrawing();
                }
            }
        }

        /// <summary>
        /// Hintergrund-Brush für die horizontale Scrollbar.
        /// </summary>
        public Brush HorizontalScrollbarBackground
        {
            get
            {
                return horizontalScrollbarBackground;
            }
            set
            {
                if (horizontalScrollbarBackground != value)
                {
                    horizontalScrollbarBackground = value;
                    InvalidateDrawing();
                }
            }
        }

        /// <summary>
        /// Brush für den horizontalen Scroller.
        /// </summary>
        public Brush HorizontalScrollbarForeground
        {
            get
            {
                return horizontalScrollbarForeground;
            }
            set
            {
                if (horizontalScrollbarForeground != value)
                {
                    horizontalScrollbarForeground = value;
                    InvalidateDrawing();
                }
            }
        }

        private readonly PropertyEventArgs<int> _verticalScrollPositionChangedEventArgs = new PropertyEventArgs<int>();
        /// <summary>
        /// Gibt die Scroll-Position auf der virtuellen Achse an oder legt diese fest.
        /// </summary>
        public int VerticalScrollPosition
        {
            get
            {
                return scrollPosition.Y;
            }
            set
            {
                int scrollRange = VirtualSize.Y - ActualClientSize.Y;
                int newPosition = Math.Max(0, Math.Min(scrollRange, value));
                if (scrollPosition.Y == newPosition) return;

                _verticalScrollPositionChangedEventArgs.OldValue = scrollPosition.Y;
                _verticalScrollPositionChangedEventArgs.NewValue = newPosition;
                _verticalScrollPositionChangedEventArgs.Handled = false;

                scrollPosition.Y = newPosition;
                InvalidateDimensions();

                OnVerticalScrollPositionChanged(_verticalScrollPositionChangedEventArgs);
                VerticalScrollPositionChanged?.Invoke(this, _verticalScrollPositionChangedEventArgs);
            }
        }
        private readonly PropertyEventArgs<int> _horizontalScrollPositionChangedEventArgs = new PropertyEventArgs<int>();
        /// <summary>
        /// Gibt die Scroll-Position auf der horizontalen Achse an oder legt diese fest.
        /// </summary>
        public int HorizontalScrollPosition
        {
            get
            {
                return scrollPosition.X;
            }
            set
            {
                int scrollRange = VirtualSize.X - ActualClientSize.X;
                int newPosition = Math.Max(0, Math.Min(scrollRange, value));
                if (scrollPosition.X == newPosition) return;
                
                
                _horizontalScrollPositionChangedEventArgs.OldValue = scrollPosition.X;
                _horizontalScrollPositionChangedEventArgs.NewValue = newPosition;
                _horizontalScrollPositionChangedEventArgs.Handled = false;

                scrollPosition.X = newPosition;
                InvalidateDimensions();

                OnHorizontalScrollPositionChanged(_horizontalScrollPositionChangedEventArgs);
                HorizontalScrollPositionChanged?.Invoke(this, _horizontalScrollPositionChangedEventArgs);
            }
        }

        /// <summary>
        /// Gibt die Breite bzw. Höhe der Scrollbar an oder legt diese fest.
        /// </summary>
        public int ScrollbarWidth
        {
            get { return scrollbarWidth; }
            set
            {
                if (scrollbarWidth != value)
                {
                    scrollbarWidth = value;
                    InvalidateDimensions();
                }
            }
        }

        /// <summary>
        /// Gibt die Mindestgröße für den greifbaren Scroller an.
        /// </summary>
        public int ScrollerMinSize
        {
            get { return scrollerMinSize; }
            set
            {
                if (scrollerMinSize != value)
                {
                    scrollerMinSize = value;
                    InvalidateDimensions();
                }
            }
        }

        /// <summary>
        /// Gibt den lokalen Bereich für die horizontale Scrollbar zurück, sofern eine angezeigt wird.
        /// </summary>
        public Rectangle? HorizontalScrollbarArea
        {
            get
            {
                if (HorizontalScrollbarEnabled &&
                    HorizontalScrollbarVisible != false &&
                    (HorizontalScrollbarVisible == true || VirtualSize.X > ActualClientSize.X))
                {
                    Point backgroundSize = ActualSize -
                        new Point(Margin.Left + Margin.Right, Margin.Top + Margin.Bottom);

                    // Scrollbereich ermitteln
                    return new Rectangle(
                        0,
                        backgroundSize.Y - ScrollbarWidth,
                        backgroundSize.X - (VerticalScrollbarEnabled && VerticalScrollbarVisible != false ? ScrollbarWidth : 0),
                        ScrollbarWidth);
                }

                return null;
            }
        }

        /// <summary>
        /// Gibt den Bereich für den horizontalen Scroller zurück, sofern einer angezeigt wird. 
        /// </summary>
        public Rectangle? HorizontalScrollerArea
        {
            get
            {
                Rectangle? scrollbarArea = HorizontalScrollbarArea;
                if (scrollbarArea.HasValue)
                {
                    int nettoBarSize = scrollbarArea.Value.Width - ScrollerMinSize;
                    float ratio = Math.Max(0f, Math.Min(1f, ((float)ActualClientSize.X / VirtualSize.X)));
                    int nettoScrollerSize = (int)(nettoBarSize * ratio);

                    float clientRangePx = VirtualSize.X - ActualClientSize.X;
                    float barRangePx = nettoBarSize - nettoScrollerSize;
                    float barPerClientPx = barRangePx / clientRangePx;
                    int barPos = (int)(barPerClientPx * HorizontalScrollPosition);

                    return new Rectangle(
                        scrollbarArea.Value.X + barPos,
                        scrollbarArea.Value.Y,
                        nettoScrollerSize + ScrollerMinSize,
                        scrollbarArea.Value.Height);
                }

                return null;
            }
        }

        /// <summary>
        /// Gibt den lokalen Bereich für die vertikale Scrollbar zurück, sofern eine angezeigt wird.
        /// </summary>
        public Rectangle? VerticalScrollbarArea
        {
            get
            {
                if (VerticalScrollbarEnabled &&
                    VerticalScrollbarVisible != false &&
                    (verticalScrollbarVisible == true || VirtualSize.Y > ActualClientSize.Y))
                {
                    Point backgroundSize = ActualSize -
                        new Point(Margin.Left + Margin.Right, Margin.Top + Margin.Bottom);

                    // Scrollbereich ermitteln
                    return new Rectangle(
                            backgroundSize.X - ScrollbarWidth,
                            0,
                            ScrollbarWidth,
                            backgroundSize.Y - (HorizontalScrollbarEnabled && HorizontalScrollbarVisible != false ? ScrollbarWidth : 0));
                }

                return null;
            }
        }

        /// <summary>
        /// Gibt den Bereich für den vertikalen Scroller zurück, sofern einer angezeigt wird. 
        /// </summary>
        public Rectangle? VerticalScrollerArea
        {
            get
            {
                Rectangle? scrollbarArea = VerticalScrollbarArea;
                if (scrollbarArea.HasValue)
                {
                    // Scroller-Größe berechnen
                    int nettoBarSize = scrollbarArea.Value.Height - ScrollerMinSize;
                    float ratio = Math.Max(0f, Math.Min(1f, ((float)ActualClientSize.Y / VirtualSize.Y)));
                    int nettoScrollerSize = (int)(nettoBarSize * ratio);

                    // Scroll-Position ermitteln
                    float clientRangePx = VirtualSize.Y - ActualClientSize.Y;
                    float barRangePx = nettoBarSize - nettoScrollerSize;
                    float barPerClientPx = barRangePx / clientRangePx;
                    int barPos = (int)(barPerClientPx * VerticalScrollPosition);

                    return new Rectangle(
                        scrollbarArea.Value.X,
                        scrollbarArea.Value.Y + barPos,
                        scrollbarArea.Value.Width,
                        nettoScrollerSize + ScrollerMinSize);
                }

                return null;
            }
        }

        /// <summary>
        /// Gibt den aktuell sichtbaren Bereich zurück.
        /// </summary>
        public Rectangle VisibleArea
        {
            get
            {
                return new Rectangle(
                    HorizontalScrollPosition, 
                    VerticalScrollPosition, 
                    Math.Min(VirtualSize.X, ActualSize.X), 
                    Math.Min(VirtualSize.Y, ActualSize.Y));
            }
        }

        #endregion

        public ScrollContainer(BaseScreenComponent manager)
            : base(manager)
        {
            HorizontalScrollbarEnabled = false;
            VerticalScrollbarEnabled = true;
            HorizontalScrollbarVisible = false;
            VerticalScrollbarVisible = true;
            CanFocus = true;
            TabStop = true;

            ApplySkin(typeof(ScrollContainer));
        }

        public override Point GetExpectedSize(Point available)
        {
            // Bereich ermitteln, der für die Scrollbars verwendet wird
            Point scrollCut = new Point(VerticalScrollbarVisible ?? false ? ScrollbarWidth : 0, HorizontalScrollbarVisible ?? false ? ScrollbarWidth : 0);

            Point client = GetMaxClientSize(available) - scrollCut;
            Point result = GetMinClientSize(available);

            // Client-Bereich erweitern, wenn entsprechende Scrollbars aktiv sind
            if (HorizontalScrollbarEnabled)
                client.X = int.MaxValue;
            if (VerticalScrollbarEnabled)
                client.Y = int.MaxValue;

            if (Content != null)
            {
                Point expected = Content.GetExpectedSize(client);
                result.Y = Math.Max(result.Y, expected.Y);
                result.X = Math.Max(result.X, expected.X);
            }
            Content.SetActualSize(result);
            result += scrollCut + Borders;

            result = new Point(Math.Min(available.X, result.X), Math.Min(available.Y, result.Y));

            return result;
        }

        public override void SetActualSize(Point available)
        {
            Point scrollCut = new Point(
                VerticalScrollbarEnabled && VerticalScrollbarVisible != false ? ScrollbarWidth : 0,
                HorizontalScrollbarEnabled && HorizontalScrollbarVisible != false ? ScrollbarWidth : 0);

            Point minSize = GetExpectedSize(available);
            Point controlSize = new Point(Math.Min(minSize.X, available.X), Math.Min(minSize.Y, available.Y));
            SetDimension(controlSize, available);

            Point client = ActualClientSize - scrollCut;
            if (HorizontalScrollbarEnabled) client.X = minSize.X;
            if (VerticalScrollbarEnabled) client.Y = minSize.Y;

            // Placement
            if (Content != null)
            {
                Content.SetActualSize(new Point(Math.Max(Content.ActualSize.X, client.X), Math.Max(Content.ActualSize.Y, client.Y)));
                VirtualSize = new Point(Math.Max(VirtualSize.X, Content.ActualSize.X), Math.Max(VirtualSize.Y, Content.ActualSize.Y));
                Content.ActualPosition -= new Point(HorizontalScrollPosition, VerticalScrollPosition);
            }
            else
            {
                VirtualSize = Point.Zero;
            }
        }

        protected override void OnDrawBackground(SpriteBatch batch, Rectangle area, GameTime gameTime, float alpha)
        {
            base.OnDrawBackground(batch, area, gameTime, alpha);

            // Vertikaler Scrollbar-Background
            Rectangle? verticalScrollbarArea = VerticalScrollbarArea;
            if (verticalScrollbarArea.HasValue && VerticalScrollbarBackground != null)
            {
                Rectangle absoluteArea = new Rectangle(
                    verticalScrollbarArea.Value.X + area.X,
                    verticalScrollbarArea.Value.Y + area.Y,
                    verticalScrollbarArea.Value.Width,
                    verticalScrollbarArea.Value.Height);
                VerticalScrollbarBackground.Draw(batch, absoluteArea, alpha);
            }

            // Vertikaler Scroller
            Rectangle? verticalScrollerArea = VerticalScrollerArea;
            if (verticalScrollerArea.HasValue && VerticalScrollbarForeground != null)
            {
                Rectangle absoluteArea = new Rectangle(
                    verticalScrollerArea.Value.X + area.X,
                    verticalScrollerArea.Value.Y + area.Y,
                    verticalScrollerArea.Value.Width,
                    verticalScrollerArea.Value.Height);
                VerticalScrollbarForeground.Draw(batch, absoluteArea, alpha);
            }

            // Horizontaler Scrollbar-Background
            Rectangle? horizontalScrollbarArea = HorizontalScrollbarArea;
            if (horizontalScrollbarArea.HasValue && HorizontalScrollbarBackground != null)
            {
                Rectangle absoluteArea = new Rectangle(
                    horizontalScrollbarArea.Value.X + area.X,
                    horizontalScrollbarArea.Value.Y + area.Y,
                    horizontalScrollbarArea.Value.Width,
                    horizontalScrollbarArea.Value.Height);
                HorizontalScrollbarBackground.Draw(batch, absoluteArea, alpha);
            }

            // Horizontaler Scroller
            Rectangle? horizontalScrollerArea = HorizontalScrollerArea;
            if (horizontalScrollerArea.HasValue && HorizontalScrollbarForeground != null)
            {
                Rectangle absoluteArea = new Rectangle(
                    horizontalScrollerArea.Value.X + area.X,
                    horizontalScrollerArea.Value.Y + area.Y,
                    horizontalScrollerArea.Value.Width,
                    horizontalScrollerArea.Value.Height);
                HorizontalScrollbarForeground.Draw(batch, absoluteArea, alpha);
            }
        }

        protected override void OnDrawFocusFrame(SpriteBatch batch, Rectangle contentArea, GameTime gameTime, float alpha)
        {
            if (Skin.Current.FocusFrameBrush != null)
            {
                // Rahmen um die vertikale Scrollbar
                Rectangle? vArea = VerticalScrollbarArea;
                if (vArea.HasValue)
                {
                    Rectangle area = new Rectangle(
                        vArea.Value.X + AbsolutePosition.X, 
                        vArea.Value.Y + AbsolutePosition.Y, 
                        vArea.Value.Width, vArea.Value.Height);
                    Skin.Current.FocusFrameBrush.Draw(batch, area, alpha);
                }

                // Rahmen um die horizontale Scrollbar
                Rectangle? hArea = HorizontalScrollbarArea;
                if (hArea.HasValue)
                {
                    Rectangle area = new Rectangle(
                        hArea.Value.X + AbsolutePosition.X,
                        hArea.Value.Y + AbsolutePosition.Y,
                        hArea.Value.Width, hArea.Value.Height);
                    Skin.Current.FocusFrameBrush.Draw(batch, area, alpha);
                }
            }
        }

        private int? verticalDragOffset = null;
        private int? horizontalDragOffset = null;

        protected override void OnMouseScroll(MouseScrollEventArgs args)
        {
            VerticalScrollPosition -= args.Steps * ScrollSpeed;
            args.Handled = true;

            base.OnMouseScroll(args);
        }

        protected override void OnLeftMouseDown(MouseEventArgs args)
        {
            // Dragging in vertikaler Achse starten
            Rectangle? verticalScrollerArea = VerticalScrollerArea;
            if (verticalScrollerArea.HasValue && verticalScrollerArea.Value.Intersects(args.LocalPosition))
            {
                verticalDragOffset = args.LocalPosition.Y - verticalScrollerArea.Value.Y;
                args.Handled = true;
            }

            // Dragging in horizontaler Achse starten
            Rectangle? horizontalScrollerArea = HorizontalScrollerArea;
            if (horizontalScrollerArea.HasValue && horizontalScrollerArea.Value.Intersects(args.LocalPosition))
            {
                horizontalDragOffset = args.LocalPosition.X - horizontalScrollerArea.Value.X;
                args.Handled = true;
            }

            base.OnLeftMouseDown(args);
        }

        protected override void OnLeftMouseUp(MouseEventArgs args)
        {
            // Dragging auf beiden Achsen beenden
            if (verticalDragOffset.HasValue || horizontalDragOffset.HasValue)
            {
                verticalDragOffset = null;
                horizontalDragOffset = null;
                args.Handled = true;
            }

            base.OnLeftMouseUp(args);
        }

        protected override void OnMouseMove(MouseEventArgs args)
        {
            // Laufendes Dragging verarbeiten (Vertikal)
            Rectangle? verticalScrollbarArea = VerticalScrollbarArea;
            Rectangle? verticalScrollerArea = VerticalScrollerArea;
            if (verticalDragOffset.HasValue && verticalScrollbarArea.HasValue && verticalScrollerArea.HasValue)
            {
                int scrollRangePx = verticalScrollbarArea.Value.Height - verticalScrollerArea.Value.Height;
                int scrollRange = VirtualSize.Y - ActualClientSize.Y;
                int diffPx = args.LocalPosition.Y - verticalDragOffset.Value;
                VerticalScrollPosition = (int)(((float)diffPx / scrollRangePx) * scrollRange);
                args.Handled = true;
            }

            // Laufendes Dragging verarbeiten (Vertikal)
            Rectangle? horizontalScrollbarArea = HorizontalScrollbarArea;
            Rectangle? horizontalScrollerArea = HorizontalScrollerArea;
            if (horizontalDragOffset.HasValue && horizontalScrollbarArea.HasValue && horizontalScrollerArea.HasValue)
            {
                int scrollRangePx = horizontalScrollbarArea.Value.Width - horizontalScrollerArea.Value.Width;
                int scrollRange = VirtualSize.Y - ActualClientSize.Y;
                int diffPx = args.LocalPosition.Y - horizontalDragOffset.Value;
                VerticalScrollPosition = (int)(((float)diffPx / scrollRangePx) * scrollRange);
                args.Handled = true;
            }

            base.OnMouseMove(args);
        }

        protected override void OnLeftMouseClick(MouseEventArgs args)
        {
            // Klick auf die Scrollbar (nicht auf den Scroller)
            Rectangle? verticalScrollbarArea = VerticalScrollbarArea;
            Rectangle? verticalScrollerArea = VerticalScrollerArea;
            if (verticalScrollbarArea.HasValue && 
                verticalScrollerArea.HasValue && 
                verticalScrollbarArea.Value.Intersects(args.LocalPosition))
            {
                // Klick über den Scroller -> PageUp
                if (args.LocalPosition.Y < verticalScrollerArea.Value.Y)
                    VerticalScrollPageUp();

                // Klick unter den Scroller -> PageDown
                if (args.LocalPosition.Y > verticalScrollerArea.Value.Bottom)
                    VerticalScrollPageDown();

                args.Handled = true;
            }

            // Klick auf die Scrollbar (nicht auf den Scroller)
            Rectangle? horizontalScrollbarArea = HorizontalScrollbarArea;
            Rectangle? horizontalScrollerArea = HorizontalScrollerArea;
            if (horizontalScrollerArea.HasValue && 
                horizontalScrollbarArea.HasValue &&
                horizontalScrollbarArea.Value.Intersects(args.LocalPosition))
            {
                // Klick über den Scroller -> PageUp
                if (args.LocalPosition.X < horizontalScrollerArea.Value.X)
                    HorizontalScrollPageUp();

                // Klick unter den Scroller -> PageDown
                if (args.LocalPosition.X > horizontalScrollerArea.Value.Right)
                    HorizontalScrollPageDown();

                args.Handled = true;
            }

            base.OnLeftMouseClick(args);
        }

        protected override void OnKeyPress(KeyEventArgs args)
        {
            if (Focused != TreeState.None)
            {
                switch (args.Key)
                {
                    case Keys.Left: if (HorizontalScrollbarEnabled) HorizontalScrollUp(); args.Handled = true; break;
                    case Keys.Right: if (HorizontalScrollbarEnabled) HorizontalScrollDown(); args.Handled = true; break;
                    case Keys.Up: if (VerticalScrollbarEnabled) VerticalScrollUp(); args.Handled = true; break;
                    case Keys.Down: if (VerticalScrollbarEnabled) VerticalScrollDown(); args.Handled = true; break;
                    case Keys.PageUp: if (VerticalScrollbarEnabled) VerticalScrollPageUp(); args.Handled = true; break;
                    case Keys.PageDown: if (VerticalScrollbarEnabled) VerticalScrollPageDown(); args.Handled = true; break;
                }
            }

            base.OnKeyPress(args);
        }

        public void VerticalScrollUp()
        {
            VerticalScrollPosition -= 20;
        }

        public void VerticalScrollPageUp()
        {
            VerticalScrollPosition -= (int)(ActualClientSize.Y * 0.7f);
        }

        public void VerticalScrollDown()
        {
            VerticalScrollPosition += 20;
        }

        public void VerticalScrollPageDown()
        {
            VerticalScrollPosition += (int)(ActualClientSize.Y * 0.7f);
        }

        public void HorizontalScrollUp()
        {
            HorizontalScrollPosition -= 20;
        }

        public void HorizontalScrollPageUp()
        {
            HorizontalScrollPosition -= (int)(ActualClientSize.X * 0.7f);
        }

        public void HorizontalScrollDown()
        {
            HorizontalScrollPosition += 20;
        }

        public void HorizontalScrollPageDown()
        {
            HorizontalScrollPosition += (int)(ActualClientSize.X * 0.7f);
        }

        protected virtual void OnHorizontalScrollbarEnabledChanged(PropertyEventArgs<bool> args) { }
        protected virtual void OnVerticalScrollbarEnabledChanged(PropertyEventArgs<bool> args) { }
        protected virtual void OnHorizontalScrollbarVisibleChanged(PropertyEventArgs<bool?> args) { }
        protected virtual void OnVerticalScrollbarVisibleChanged(PropertyEventArgs<bool?> args) { }

        protected virtual void OnHorizontalScrollPositionChanged(PropertyEventArgs<int> args) { }

        protected virtual void OnVerticalScrollPositionChanged(PropertyEventArgs<int> args) { }

        protected virtual void OnVirtualSizeChanged(PropertyEventArgs<Point> args) { }

        public event PropertyChangedDelegate<bool> HorizontalScrollbarEnabledChanged;

        public event PropertyChangedDelegate<bool> VerticalScrollbarEnabledChanged;

        public event PropertyChangedDelegate<bool?> HorizontalScrollbarVisibleChanged;

        public event PropertyChangedDelegate<bool?> VerticalScrollbarVisibleChanged;

        public event PropertyChangedDelegate<int> HorizontalScrollPositionChanged;
        public event PropertyChangedDelegate<int> VerticalScrollPositionChanged;

        public event PropertyChangedDelegate<Point> VirtualSizeChanged;
    }
}
