﻿// <copyright file="DefaultSlider.cs" company="LeagueSharp">
//    Copyright (c) 2015 LeagueSharp.
// 
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
// 
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
// 
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see http://www.gnu.org/licenses/
// </copyright>

namespace LeagueSharp.SDK.UI.Skins.Default
{
    using Core.Utils;
    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.Utils;
    using SharpDX;
    using SharpDX.Direct3D9;
    using System;
    using System.Globalization;

    /// <summary>
    ///     A default implementation of an <see cref="ADrawable{MenuSlider}" />
    /// </summary>
    public class DefaultSlider : ADrawable<MenuSlider>
    {
        #region Static Fields

        /// <summary>
        ///     The line.
        /// </summary>
        private static readonly Line Line = new Line(Drawing.Direct3DDevice) { GLLines = true };

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultSlider" /> class.
        /// </summary>
        /// <param name="component">
        ///     The menu component
        /// </param>
        public DefaultSlider(MenuSlider component)
            : base(component)
        {
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the additional boundaries.
        /// </summary>
        /// <param name="component">The <see cref="MenuSlider" /></param>
        /// <returns>The <see cref="Rectangle" /></returns>
        public Rectangle AdditionalBoundries(MenuSlider component)
        {
            return DefaultUtilities.GetContainerRectangle(component);
        }

        /// <summary>
        ///     Gets the boundaries
        /// </summary>
        /// <param name="component">The <see cref="MenuSlider" /></param>
        /// <returns>The <see cref="Rectangle" /></returns>
        public Rectangle Bounding(MenuSlider component)
        {
            return DefaultUtilities.GetContainerRectangle(component);
        }

        /// <summary>
        ///     Disposes any resources used in this handler.
        /// </summary>
        public override void Dispose()
        {
            // Do nothing.
        }

        /// <summary>
        ///     Draws a <see cref="MenuSlider" />
        /// </summary>
        public override void Draw()
        {
            var position = this.Component.Position;
            var centeredY =
                (int)
                DefaultUtilities.GetContainerRectangle(this.Component)
                    .GetCenteredText(null, MenuSettings.Font, MultiLanguage.Translation(this.Component.DisplayName), CenteredFlags.VerticalCenter)
                    .Y;
            var percent = (this.Component.Value - this.Component.MinValue)
                          / (float)(this.Component.MaxValue - this.Component.MinValue);
            var x = position.X + (percent * this.Component.MenuWidth);

            Line.Width = 2;
            Line.Begin();
            Line.Draw(
                new[] { new Vector2(x, position.Y + 1), new Vector2(x, position.Y + MenuSettings.ContainerHeight) },
                this.Component.Interacting ? new ColorBGRA(255, 0, 0, 255) : new ColorBGRA(50, 154, 205, 255));
            Line.End();

            MenuSettings.Font.DrawText(
                MenuManager.Instance.Sprite,
                MultiLanguage.Translation(this.Component.DisplayName),
                (int)(position.X + MenuSettings.ContainerTextOffset),
                centeredY,
                MenuSettings.TextColor);

            var measureText = MenuSettings.Font.MeasureText(
                null,
                this.Component.Value.ToString(CultureInfo.InvariantCulture),
                0);
            MenuSettings.Font.DrawText(
                MenuManager.Instance.Sprite,
                this.Component.Value.ToString(CultureInfo.InvariantCulture),
                (int)(position.X + this.Component.MenuWidth - 5 - measureText.Width),
                centeredY,
                MenuSettings.TextColor);

            Line.Width = MenuSettings.ContainerHeight;
            Line.Begin();
            Line.Draw(
                new[]
                    {
                        new Vector2(position.X, position.Y + (MenuSettings.ContainerHeight / 2f)),
                        new Vector2(x, position.Y + (MenuSettings.ContainerHeight / 2f))
                    },
                MenuSettings.HoverColor);
            Line.End();
        }

        /// <summary>
        ///     Processes windows messages
        /// </summary>
        /// <param name="args">event data</param>
        public override void OnWndProc(WindowsKeys args)
        {
            if (!this.Component.Visible)
            {
                return;
            }

            if (args.Msg == WindowsMessages.MOUSEMOVE && this.Component.Interacting)
            {
                this.CalculateNewValue(this.Component, args);
            }
            else if (args.Msg == WindowsMessages.LBUTTONDOWN && !this.Component.Interacting)
            {
                var container = this.Bounding(this.Component);

                if (args.Cursor.IsUnderRectangle(container.X, container.Y, container.Width, container.Height))
                {
                    this.Component.Interacting = true;
                    this.CalculateNewValue(this.Component, args);
                }
            }
            else if (args.Msg == WindowsMessages.LBUTTONUP)
            {
                this.Component.Interacting = false;
            }
        }

        /// <summary>
        ///     Calculates the width of this component
        /// </summary>
        /// <returns>
        ///     The width.
        /// </returns>
        public override int Width()
        {
            return DefaultUtilities.CalcWidthItem(this.Component) + 100;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Calculate the new value based onto the cursor position.
        /// </summary>
        /// <param name="component">menu component</param>
        /// <param name="args">
        ///     <see cref="WindowsKeys" /> data
        /// </param>
        private void CalculateNewValue(MenuSlider component, WindowsKeys args)
        {
            var newValue =
                (int)
                Math.Round(
                    component.MinValue
                    + (((args.Cursor.X - component.Position.X) * (component.MaxValue - component.MinValue))
                       / component.MenuWidth));
            if (newValue < component.MinValue)
            {
                newValue = component.MinValue;
            }
            else if (newValue > component.MaxValue)
            {
                newValue = component.MaxValue;
            }

            if (newValue != component.Value)
            {
                component.Value = newValue;
                component.FireEvent();
            }
        }

        #endregion
    }
}