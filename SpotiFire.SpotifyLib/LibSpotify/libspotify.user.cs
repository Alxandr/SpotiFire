using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SpotiFire
{
    /// <summary>
    /// User relation type.
    /// </summary>
    public enum RelationType
    {
        /// <summary>
        /// Not yet known.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// No relation.
        /// </summary>
        None = 1,

        /// <summary>
        /// The currently logged in user is following this user.
        /// </summary>
        Unidirectional = 2,

        /// <summary>
        /// Bidirectional friendship established.
        /// </summary>
        Bidirectional = 3,
    }

}
