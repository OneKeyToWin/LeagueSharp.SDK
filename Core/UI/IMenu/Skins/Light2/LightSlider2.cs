﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LightSlider2.cs" company="LeagueSharp">
//   Copyright (C) 2015 LeagueSharp
//   
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//   
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details.
//   
//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// <summary>
//   A custom implementation of an <see cref="ADrawable{MenuSlider}" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LeagueSharp.SDK.UI.Skins.Light2
{
    using Core.Utils;
    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.UI.Skins.Light;
    using SharpDX;
    using SharpDX.Direct3D9;
    using System.Globalization;

    /// <summary>
    ///     A default implementation of an <see cref="ADrawable{MenuSlider}" />
    /// </summary>
    public class LightSlider2 : LightSlider
    {
        #region Static Fields

        /// <summary>
        ///     The line.
        /// </summary>
        private static readonly Line Line = new Line(Drawing.Direct3DDevice) { GLLines = true };

        /// <summary>
        ///     Offset.
        /// </summary>
        private static readonly int Offset = 15;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="LightSlider2" /> class.
        /// </summary>
        /// <param name="component">
        ///     The menu component
        /// </param>
        public LightSlider2(MenuSlider component)
            : base(component)
        {
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Draws a <see cref="MenuSlider" />
        /// </summary>
        public override void Draw()
        {
            var position = this.Component.Position;
            var centeredY =
                (int)
                LightUtilities.GetContainerRectangle(this.Component)
                    .GetCenteredText(null, MenuSettings.Font, MultiLanguage.Translation(this.Component.DisplayName), CenteredFlags.VerticalCenter)
                    .Y;
            var percent = (this.Component.Value - this.Component.MinValue)
                          / (float)(this.Component.MaxValue - this.Component.MinValue);
            var x = position.X + Offset + (percent * (this.Component.MenuWidth - Offset * 2));

            Line.Width = 3;
            Line.Begin();
            Line.Draw(
                new[] { new Vector2(x, position.Y + 1), new Vector2(x, position.Y + MenuSettings.ContainerHeight) },
                this.Component.Interacting ? new ColorBGRA(210, 210, 210, 255) : new ColorBGRA(170, 170, 170, 255));
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
                MultiLanguage.Translation(this.Component.Value.ToString(CultureInfo.InvariantCulture)),
                (int)(position.X + this.Component.MenuWidth - 5 - measureText.Width),
                centeredY,
                MenuSettings.TextColor);

            Line.Width = MenuSettings.ContainerHeight;
            Line.Begin();
            Line.Draw(
                new[]
                    {
                        new Vector2(position.X + Offset, position.Y + MenuSettings.ContainerHeight / 2f),
                        new Vector2(x, position.Y + MenuSettings.ContainerHeight / 2f)
                    },
                new ColorBGRA(229, 229, 229, 255));
            Line.End();
        }

        #endregion
    }
}