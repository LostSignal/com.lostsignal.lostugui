//-----------------------------------------------------------------------
// <copyright file="StringInputResult.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Lost
{
    public struct StringInputResult
    {
        public InputResult Result;
        public string Text;

        public enum InputResult
        {
            Cancel,
            Ok,
        }
    }
}

#endif
