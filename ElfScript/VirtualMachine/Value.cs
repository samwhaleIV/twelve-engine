﻿using ElfScript.VirtualMachine.Memory;

namespace ElfScript.VirtualMachine {
    internal readonly struct Value {
        public readonly Type Type { get; private init; }
        public readonly Address Address { get; private init; }
        public Value(Address address,Type type) { Address = address; Type = type; }
        public static readonly Value Null = new(Address.Null,Type.Integer);
    }
}
