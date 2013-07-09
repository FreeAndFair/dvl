// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VoterCardGenerator.cs" company="DVL">
//   Author: Niels Søholm (nm@9la.dk)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Central.Central.Utility
{
    using System;

    /// <summary>
    /// Tiny hashing utility class for anonymizing CPR numbers when printing barcodes.
    /// </summary>
    public class BarCodeHashing
    {
        /// <summary>
        /// Secret anonymization hash of CPR so that voter cards can be safely mailed.
        /// (Should probably be more complicated in reality).
        /// </summary>
        /// <param name="cprno">The CPR number to be hashed.</param>
        /// <returns>The resulting barcode.</returns>
        public static long Hash(uint cprno)
        {
            return Convert.ToInt64(cprno) * 101;
        }

        /// <summary>
        /// UnHash barcode from voter card to CPR.
        /// </summary>
        /// <param name="barcode">The barcode to be unhashed.</param>
        /// <returns>The resulting CPR number.</returns>
        public static uint UnHash(string barcode)
        {
            long bc = Convert.ToInt64(barcode);
            return Convert.ToUInt32(bc / 101);
        }
    }
}
