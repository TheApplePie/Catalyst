﻿namespace Catalyst.ImGui
{
    public unsafe partial struct ImDrawDataPtr
    {
        public RangePtrAccessor<ImDrawListPtr> CmdListsRange => new RangePtrAccessor<ImDrawListPtr>(CmdLists.ToPointer(), CmdListsCount);
    }
}
