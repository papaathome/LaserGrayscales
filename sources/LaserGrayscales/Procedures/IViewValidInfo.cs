using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace As.Applications.Procedures
{
    public delegate void IsValidViewChanged(object sender, EventArgs e);

    public interface IViewValidInfo
    {
        /// <summary>
        /// True: all checked values in the view are valid; False: at least one checked value in the view is not valid
        /// </summary>
        bool IsValidView { get; }

        /// <summary>
        /// Triggered direct after IsValidView is changed
        /// </summary>
        event IsValidViewChanged? OnIsValidViewChanged;
    }
}
