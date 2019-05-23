﻿using System;
using System.Linq;
using engenious.Graphics;
using engenious.Input;

namespace engenious.UI.Controls
{
    /// <summary>
    /// Control für Texteingabe
    /// </summary>
    public class Textbox : ContentControl, ITextControl
    {
        private int cursorPosition;

        private int selectionStart;

        private int renderOffsetX;

        private Label label;

        private ScrollContainer scrollContainer;

        /// <summary>
        /// Gibt die aktuelle Cursor-Position an oder legt diese fest.
        /// </summary>
        public int CursorPosition
        {
            get { return cursorPosition; }
            set
            {
                if (cursorPosition != value)
                {
                    cursorPosition = Math.Min(label.Text.Length, value);
                    var textSize = (int)Font.MeasureString(Text.Substring(0, CursorPosition)).X;
                    scrollContainer.HorizontalScrollPosition = Math.Max(0, textSize - ActualClientArea.Width);
                    InvalidateDrawing();
                }
            }
        }

        /// <summary>
        /// Gibt den Beginn des Selektionsbereichs an oder legt diesen fest.
        /// </summary>
        public int SelectionStart
        {
            get { return selectionStart; }
            set
            {
                if (selectionStart != value)
                {
                    selectionStart = value;
                    InvalidateDrawing();
                }
            }
        }

        public string Text { get => label.Text; set => label.Text = value; }
        public SpriteFont Font { get => label.Font; set => label.Font = value; }
        public Color TextColor { get => label.TextColor; set => label.TextColor = value; }
        public HorizontalAlignment HorizontalTextAlignment { get => label.HorizontalTextAlignment; set => label.HorizontalTextAlignment = value; }
        public VerticalAlignment VerticalTextAlignment { get => label.VerticalTextAlignment; set => label.VerticalTextAlignment = value; }
        public bool WordWrap { get => label.WordWrap; set => label.WordWrap = value; }

        /// <summary>
        /// Erzeugt eine neue Instanz der Textbox-Klasse
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="style"></param>
        public Textbox(BaseScreenComponent manager, string style = "")
            : base(manager, style)
        {
            label = new Label(manager, style)
            {
                HorizontalTextAlignment = HorizontalAlignment.Left,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Padding = Border.All(0)
            };

            scrollContainer = new ScrollContainer(manager)
            {
                HorizontalScrollbarVisible = false,
                VerticalScrollbarVisible = false,
                HorizontalScrollbarEnabled = true,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                CanFocus = false,
                //VerticalAlignment = VerticalAlignment.Stretch,
                Content = label
            };
            Content = scrollContainer;

            Padding = Border.All(5);
            TextColor = Color.Black;
            TabStop = true;
            CanFocus = true;

            ApplySkin(typeof(Textbox));
        }

        protected override void OnPreDraw(GameTime gameTime)
        {
            base.OnPreDraw(gameTime);
        }

        /// <summary>
        /// Malt den Content des Controls
        /// </summary>
        /// <param name="batch">Spritebatch</param>
        /// <param name="area">Bereich für den Content in absoluten Koordinaten</param>
        /// <param name="gameTime">GameTime</param>
        /// <param name="alpha">Die Transparenz des Controls.</param>
        protected override void OnDrawContent(SpriteBatch batch, Rectangle area, GameTime gameTime, float alpha)
        {

            if (CursorPosition > Text.Length)
                CursorPosition = Text.Length;
            if (SelectionStart > Text.Length)
                SelectionStart = CursorPosition;

            // Selektion
            if (SelectionStart != CursorPosition)
            {
                int from = Math.Min(SelectionStart, CursorPosition);
                int to = Math.Max(SelectionStart, CursorPosition);
                var selectFrom = Font.MeasureString(Text.Substring(0, from));
                var selectTo = Font.MeasureString(Text.Substring(from, to - from));
                batch.Draw(Skin.Pix, new Rectangle(area.X + (int)selectFrom.X - scrollContainer.HorizontalScrollPosition, area.Y, (int)selectTo.X - scrollContainer.HorizontalScrollPosition, (int)selectTo.Y), Color.LightBlue);
            }

            base.OnDrawContent(batch, area, gameTime, alpha);

            // Cursor (wenn Fokus)
            if (Focused == TreeState.Active)
            {
                if ((int)gameTime.TotalGameTime.TotalSeconds % 2 == 0)
                {
                    var selectionSize = Font.MeasureString(Text.Substring(0, CursorPosition));
                    batch.Draw(Skin.Pix, new Rectangle(area.X + (int)selectionSize.X - scrollContainer.HorizontalScrollPosition, area.Y, 1, Font.LineSpacing), TextColor);
                }
            }
        }

        protected override void OnKeyTextPress(KeyTextEventArgs args)
        {
            // Ignorieren, wenn kein Fokus
            if (Focused != TreeState.Active) return;

            // Steuerzeichen ignorieren
            if (args.Character == '\b' || args.Character == '\t' || args.Character == '\n' || args.Character == '\r')
                return;

            // Strg-Kombinationen (A, X, C, V) ignorieren
            if (args.Character == '\u0001' ||
                args.Character == '\u0003' ||
                args.Character == '\u0016' ||
                args.Character == '\u0018')
                return;

            //Escape ignorieren
            if (args.Character == '\u001b')
                return;

            if (SelectionStart != CursorPosition)
            {
                int from = Math.Min(SelectionStart, CursorPosition);
                int to = Math.Max(SelectionStart, CursorPosition);
                Text = Text.Substring(0, from) + Text.Substring(to);
                CursorPosition = from;
                SelectionStart = from;
            }

            Text = Text.Substring(0, CursorPosition) + args.Character + Text.Substring(CursorPosition);
            CursorPosition++;
            SelectionStart++;
            args.Handled = true;
        }

        /// <summary>
        /// Wird aufgerufen, wenn eine Taste gedrückt ist.
        /// </summary>
        /// <param name="args">Zusätzliche Daten zum Event.</param>
        protected override void OnKeyPress(KeyEventArgs args)
        {
            // Ignorieren, wenn kein Fokus
            if (Focused != TreeState.Active) return;

            // Linke Pfeiltaste
            if (args.Key == Keys.Left)
            {
                if (args.Ctrl)
                {
                    int x = Math.Min(CursorPosition - 1, Text.Length - 1);
                    while (x > 0 && Text[x] != ' ') x--;
                    CursorPosition = x;
                }
                else
                {
                    CursorPosition--;
                    CursorPosition = Math.Max(CursorPosition, 0);
                }

                if (!args.Shift)
                    SelectionStart = CursorPosition;
                args.Handled = true;
            }

            // Rechte Pfeiltaste
            if (args.Key == Keys.Right)
            {
                if (args.Ctrl)
                {
                    int x = CursorPosition;
                    while (x < Text.Length && Text[x] != ' ') x++;
                    CursorPosition = Math.Min(x + 1, Text.Length);
                }
                else
                {
                    CursorPosition++;
                    CursorPosition = Math.Min(Text.Length, CursorPosition);
                }
                if (!args.Shift)
                    SelectionStart = CursorPosition;
                args.Handled = true;
            }

            // Pos1-Taste
            if (args.Key == Keys.Home)
            {
                CursorPosition = 0;
                if (!args.Shift)
                    SelectionStart = CursorPosition;
                args.Handled = true;
            }

            // Ende-Taste
            if (args.Key == Keys.End)
            {
                CursorPosition = Text.Length;
                if (!args.Shift)
                    SelectionStart = CursorPosition;
                args.Handled = true;
            }

            // Backspace
            if (args.Key == Keys.Back)
            {
                if (SelectionStart != CursorPosition)
                {
                    int from = Math.Min(SelectionStart, CursorPosition);
                    int to = Math.Max(SelectionStart, CursorPosition);
                    Text = Text.Substring(0, from) + Text.Substring(to);
                    CursorPosition = from;
                    SelectionStart = from;
                }
                else if (CursorPosition > 0)
                {
                    Text = Text.Substring(0, CursorPosition - 1) + Text.Substring(CursorPosition);
                    CursorPosition--;
                    SelectionStart--;
                }
                args.Handled = true;
            }

            // Entfernen
            if (args.Key == Keys.Delete)
            {
                if (SelectionStart != CursorPosition)
                {
                    int from = Math.Min(SelectionStart, CursorPosition);
                    int to = Math.Max(SelectionStart, CursorPosition);
                    Text = Text.Substring(0, from) + Text.Substring(to);
                    CursorPosition = from;
                    SelectionStart = from;
                }
                else if (CursorPosition < Text.Length)
                {
                    Text = Text.Substring(0, CursorPosition) + Text.Substring(CursorPosition + 1);
                }
                args.Handled = true;
            }

            // Ctrl+A (Select all)
            if (args.Key == Keys.A && args.Ctrl)
            {
                // Alles markieren
                SelectionStart = 0;
                CursorPosition = Text.Length;

                args.Handled = true;
            }

            // Ctrl+C (Copy)
            if (args.Key == Keys.C && args.Ctrl)
            {
                int from = Math.Min(SelectionStart, CursorPosition);
                int to = Math.Max(SelectionStart, CursorPosition);

                // Selektion kopieren
                if (from == to) SystemSpecific.ClearClipboard();
                else SystemSpecific.SetClipboardText(Text.Substring(from, to - from));

                args.Handled = true;
            }

            // Ctrl+X (Cut)
            if (args.Key == Keys.X && args.Ctrl)
            {
                int from = Math.Min(SelectionStart, CursorPosition);
                int to = Math.Max(SelectionStart, CursorPosition);

                // Selektion ausschneiden
                if (from == to) SystemSpecific.ClearClipboard();
                else SystemSpecific.SetClipboardText(Text.Substring(from, to - from));

                CursorPosition = from;
                SelectionStart = from;
                Text = Text.Substring(0, from) + Text.Substring(to);

                args.Handled = true;
            }

            // Ctrl+V (Paste)
            if (args.Key == Keys.V && args.Ctrl)
            {
                // Selektierten Text löschen
                if (SelectionStart != CursorPosition)
                {
                    int from = Math.Min(SelectionStart, CursorPosition);
                    int to = Math.Max(SelectionStart, CursorPosition);
                    Text = Text.Substring(0, from) + Text.Substring(to);
                    CursorPosition = from;
                    SelectionStart = from;
                }

                // Text einfügen und Cursor ans Ende setzen
                string paste = SystemSpecific.GetClipboardText();
                Text = Text.Substring(0, CursorPosition) + paste + Text.Substring(CursorPosition);
                CursorPosition += paste.Length;
                SelectionStart = CursorPosition;

                args.Handled = true;
            }

            args.Handled = true;

            //Manche Keys weitergeben
            if (ignoreKeys.Contains<Keys>(args.Key))
                args.Handled = false;

            base.OnKeyPress(args);
        }

        protected override void OnLeftMouseDown(MouseEventArgs args)
        {
            base.OnLeftMouseDown(args);
            
            for(int i = 0; i < Text.Length; i++)
            {
                //Font.MeasureString
            }
        }

        protected override void OnMouseMove(MouseEventArgs args)
        {
            base.OnMouseMove(args);
        }

        Keys[] ignoreKeys =
        {
            Keys.Escape,
            Keys.Tab,
            Keys.F1,
            Keys.F2,
            Keys.F3,
            Keys.F4,
            Keys.F5,
            Keys.F6,
            Keys.F7,
            Keys.F8,
            Keys.F9,
            Keys.F10,
            Keys.F11,
            Keys.F12,
            Keys.End,
            Keys.Pause
        };
    }


}
