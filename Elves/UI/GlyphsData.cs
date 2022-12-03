using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elves.UI {
    public sealed partial class UVSpriteFont {

        private readonly Dictionary<char,Rectangle> Glpyhs = new Dictionary<char,Rectangle>() {
            { 'A', new Rectangle(2,0,21,38) },
            { 'B', new Rectangle(25,0,18,38) },
            { 'C', new Rectangle(46,0,18,38) },
            { 'D', new Rectangle(66,0,18,38) },
            { 'E', new Rectangle(86,0,14,38) },
            { 'F', new Rectangle(102,0,14,38) },
            { 'G', new Rectangle(118,0,21,38) },
            { 'H', new Rectangle(141,0,18,38) },
            { 'I', new Rectangle(161,0,10,38) },
            { 'J', new Rectangle(173,0,15,38) },
            { 'K', new Rectangle(190,0,15,38) },
            { 'L', new Rectangle(207,0,10,38) },
            { 'M', new Rectangle(219,0,23,38) },
            { 'N', new Rectangle(244,0,18,38) },
            { 'O', new Rectangle(2,38,18,38) },
            { 'P', new Rectangle(22,38,16,38) },
            { 'Q', new Rectangle(40,38,18,38) },
            { 'R', new Rectangle(60,38,18,38) },
            { 'S', new Rectangle(80,38,18,38) },
            { 'T', new Rectangle(100,38,18,38) },
            { 'U', new Rectangle(120,38,20,38) },
            { 'V', new Rectangle(142,38,18,38) },
            { 'W', new Rectangle(162,38,28,38) },
            { 'X', new Rectangle(192,38,18,38) },
            { 'Y', new Rectangle(212,38,18,38) },
            { 'Z', new Rectangle(232,38,18,38) },
            { '1', new Rectangle(2,76,15,38) },
            { '2', new Rectangle(19,76,18,38) },
            { '3', new Rectangle(39,76,18,38) },
            { '4', new Rectangle(61,76,20,38) },
            { '5', new Rectangle(83,76,18,38) },
            { '6', new Rectangle(103,76,18,38) },
            { '7', new Rectangle(123,76,18,38) },
            { '8', new Rectangle(143,76,18,38) },
            { '9', new Rectangle(163,76,18,38) },
            { '0', new Rectangle(183,76,18,38) },
            { '/', new Rectangle(203,76,12,38) },
            { '\\', new Rectangle(217,76,12,38) },
            { '%', new Rectangle(231,76,12,38) },
            { '!', new Rectangle(2,114,6,38) },
            { '?', new Rectangle(10,114,18,38) },
            { '(', new Rectangle(30,114,10,38) },
            { ')', new Rectangle(42,114,10,38) },
            { ':', new Rectangle(54,114,4,38) },
            { ';', new Rectangle(60,114,5,38) },
            { '\'', new Rectangle(67,114,3,38) },
            { '"', new Rectangle(72,114,9,38) },
            { ',', new Rectangle(83,114,5,38) },
            { '.', new Rectangle(90,114,4,38) },
            { '[', new Rectangle(96,114,10,38) },
            { ']', new Rectangle(108,114,10,38) },
            { '+', new Rectangle(120,114,14,38) },
            { '-', new Rectangle(136,114,14,38) }, /* MINUS */
            { '=', new Rectangle(152,114,14,38) },
            { '<', new Rectangle(168,114,8,38) },
            { '>', new Rectangle(178,114,8,38) },
            { '&', new Rectangle(188,114,12,38) }, /* DIVIDE */
            { '*', new Rectangle(202,114,11,38) }, /* MULTIPLY */
            { '^', new Rectangle(215,114,8,38) },  /* HYPHEN */
        };
    }
}
