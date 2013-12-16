// ***********************************************************************
// Assembly         : BoonieBear.DockUnit
// Author           : ThinkPad
// Created          : 12-10-2013
//
// Last Modified By : ThinkPad
// Last Modified On : 12-10-2013
// ***********************************************************************
// <copyright file="DataItem.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace BoonieBear.DockUnit.Model
{
    /// <summary>
    /// Class DataItem.
    /// </summary>
    public class DataItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataItem"/> class.
        /// </summary>
        /// <param name="title">The title.</param>
        public DataItem(string title)
        {
            Title = title;
        }

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title
        {
            get;
            private set;
        }
    }
}
