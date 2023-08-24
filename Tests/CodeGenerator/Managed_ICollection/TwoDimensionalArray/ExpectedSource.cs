﻿using Microsoft.CodeAnalysis.Text;

namespace Tests.CodeGenerator.Managed_ICollection.TwoDimensionalArray;

static public class ExpectedSource
{
    static public (String, SourceText) Handler
    {
        get
        {
            return (HANDLER_FILENAME, s_HandlerSource!);
        }
    }

    static public readonly SourceText s_HandlerSource = SourceText.From(text: HANDLER_SOURCE,
                                                                        encoding: Encoding.UTF8);

    public const String HANDLER_FILENAME = @"Narumikazuchi.Generators.ByteSerialization.Core\Narumikazuchi.Generators.ByteSerialization.Generators.SerializableGenerator\Narumikazuchi.Generated.Internals.ByteSerialization.Handler.Array[System.Collections.Generic.List`1[Person]+2].g.cs";

    public const String HANDLER_SOURCE = @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
#pragma warning disable
#nullable disable

using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace Narumikazuchi.Generated.Internals.ByteSerialization;

public partial interface IAssemblyHandler_TestProject : Narumikazuchi.Generators.ByteSerialization.ISerializationHandler<System.Collections.Generic.List<Person>[,]>
{
    [CompilerGenerated]
    UInt32 Narumikazuchi.Generators.ByteSerialization.ISerializationHandler<System.Collections.Generic.List<Person>[,]>.Deserialize(ReadOnlySpan<Byte> buffer, out System.Collections.Generic.List<Person>[,] result)
    {
        var pointer = sizeof(Int32);
        result = default(System.Collections.Generic.List<Person>[,]);
        if (buffer[pointer++] == 0x1)
        {
            var _var0 = Unsafe.As<Byte, Narumikazuchi.Generators.ByteSerialization.TypeIdentifier>(ref MemoryMarshal.GetReference(buffer[pointer..]));
            pointer += Unsafe.SizeOf<Narumikazuchi.Generators.ByteSerialization.TypeIdentifier>();
            var _var1 = Unsafe.As<Byte, Int32>(ref MemoryMarshal.GetReference(buffer[pointer..]));
            pointer += sizeof(Int32);
            var _var2 = Unsafe.As<Byte, Int32>(ref MemoryMarshal.GetReference(buffer[pointer..]));
            pointer += sizeof(Int32);
            result = new System.Collections.Generic.List<Person>[_var1, _var2];
            for (var _var3 = 0; _var3 < _var1; _var3++)
            {
                for (var _var4 = 0; _var4 < _var2; _var4++)
                {
                    var _var5 = default(System.Collections.Generic.List<Person>);
                    if (buffer[pointer++] == 0x1)
                    {
                        var _var6 = Unsafe.As<Byte, Narumikazuchi.Generators.ByteSerialization.TypeIdentifier>(ref MemoryMarshal.GetReference(buffer[pointer..]));
                        pointer += Unsafe.SizeOf<Narumikazuchi.Generators.ByteSerialization.TypeIdentifier>();
                        var _var7 = Unsafe.As<Byte, Int32>(ref MemoryMarshal.GetReference(buffer[pointer..]));
                        pointer += Unsafe.SizeOf<Int32>();
                        _var5 = new System.Collections.Generic.List<Person>()
                        {
                            Capacity = _var7,
                        };
                        var _var8 = Unsafe.As<Byte, Int32>(ref MemoryMarshal.GetReference(buffer[pointer..]));
                        pointer += sizeof(Int32);
                        for (var _var9 = 0; _var9 < _var8; _var9++)
                        {
                            var _var10 = default(Person);
                            if (buffer[pointer++] == 0x1)
                            {
                                var _var11 = Unsafe.As<Byte, Narumikazuchi.Generators.ByteSerialization.TypeIdentifier>(ref MemoryMarshal.GetReference(buffer[pointer..]));
                                pointer += Unsafe.SizeOf<Narumikazuchi.Generators.ByteSerialization.TypeIdentifier>();
                                var _var12 = default(String);
                                if (buffer[pointer++] == 0x1)
                                {
                                    var _var13 = Unsafe.As<Byte, Narumikazuchi.Generators.ByteSerialization.TypeIdentifier>(ref MemoryMarshal.GetReference(buffer[pointer..]));
                                    pointer += Unsafe.SizeOf<Narumikazuchi.Generators.ByteSerialization.TypeIdentifier>();
                                    var _var14 = Unsafe.As<Byte, Int32>(ref MemoryMarshal.GetReference(buffer[pointer..]));
                                    pointer += Unsafe.SizeOf<Int32>();
                                    _var12 = new String(MemoryMarshal.Cast<Byte, Char>(buffer[pointer..(pointer + _var14)]));
                                    pointer += _var14;
                                }
                                else
                                {
                                    pointer += Unsafe.SizeOf<Narumikazuchi.Generators.ByteSerialization.TypeIdentifier>();
                                }
                                var _var15 = default(String);
                                if (buffer[pointer++] == 0x1)
                                {
                                    var _var16 = Unsafe.As<Byte, Narumikazuchi.Generators.ByteSerialization.TypeIdentifier>(ref MemoryMarshal.GetReference(buffer[pointer..]));
                                    pointer += Unsafe.SizeOf<Narumikazuchi.Generators.ByteSerialization.TypeIdentifier>();
                                    var _var17 = Unsafe.As<Byte, Int32>(ref MemoryMarshal.GetReference(buffer[pointer..]));
                                    pointer += Unsafe.SizeOf<Int32>();
                                    _var15 = new String(MemoryMarshal.Cast<Byte, Char>(buffer[pointer..(pointer + _var17)]));
                                    pointer += _var17;
                                }
                                else
                                {
                                    pointer += Unsafe.SizeOf<Narumikazuchi.Generators.ByteSerialization.TypeIdentifier>();
                                }
                                var _var18 = Unsafe.As<Byte, DateOnly>(ref MemoryMarshal.GetReference(buffer[pointer..]));
                                pointer += Unsafe.SizeOf<DateOnly>();
                                var _var19 = Unsafe.As<Byte, Guid>(ref MemoryMarshal.GetReference(buffer[pointer..]));
                                pointer += Unsafe.SizeOf<Guid>();
                                _var10 = new Person()
                                {
                                    FirstName = _var12,
                                    LastName = _var15,
                                    DateOfBirth = _var18,
                                    CustomerId = _var19,
                                };
                            }
                            else
                            {
                                pointer += Unsafe.SizeOf<Narumikazuchi.Generators.ByteSerialization.TypeIdentifier>();
                            }
                            _var5.Add(_var10);
                        }
                    }
                    else
                    {
                        pointer += Unsafe.SizeOf<Narumikazuchi.Generators.ByteSerialization.TypeIdentifier>();
                    }
                    result[_var3, _var4] = _var5;
                }
            }
        }
        else
        {
            Unsafe.SizeOf<Narumikazuchi.Generators.ByteSerialization.TypeIdentifier>();
        }
        return (UInt32)pointer;
    }

    [CompilerGenerated]
    Int32 Narumikazuchi.Generators.ByteSerialization.ISerializationHandler<System.Collections.Generic.List<Person>[,]>.GetExpectedArraySize(System.Collections.Generic.List<Person>[,] value)
    {
        var size = sizeof(Int64) + Unsafe.SizeOf<Narumikazuchi.Generators.ByteSerialization.TypeIdentifier>();
        if (value is not null)
        {
            size += sizeof(Int32) * 2;
            foreach (var _var0 in value)
            {
                size += sizeof(Byte) + Unsafe.SizeOf<Narumikazuchi.Generators.ByteSerialization.TypeIdentifier>();
                if (_var0 is not null)
                {
                    size += Unsafe.SizeOf<Int32>();
                    foreach (var _var1 in _var0)
                    {
                        size += sizeof(Byte) + Unsafe.SizeOf<Narumikazuchi.Generators.ByteSerialization.TypeIdentifier>();
                        if (_var1 is not null)
                        {
                            size += sizeof(Byte) + Unsafe.SizeOf<Narumikazuchi.Generators.ByteSerialization.TypeIdentifier>();
                            size += 4 * _var1.FirstName.Length;
                            size += sizeof(Byte) + Unsafe.SizeOf<Narumikazuchi.Generators.ByteSerialization.TypeIdentifier>();
                            size += 4 * _var1.LastName.Length;
                            size += Unsafe.SizeOf<DateOnly>();
                            size += Unsafe.SizeOf<Guid>();
                        }
                    }
                }
            }
        }
        return size;
    }

    [CompilerGenerated]
    UInt32 Narumikazuchi.Generators.ByteSerialization.ISerializationHandler<System.Collections.Generic.List<Person>[,]>.Serialize(Span<Byte> buffer, System.Collections.Generic.List<Person>[,] value)
    {
        var pointer = sizeof(Int32);
        if (value is null)
        {
            buffer[pointer++] = 0x0;
            Unsafe.As<Byte, Narumikazuchi.Generators.ByteSerialization.TypeIdentifier>(ref buffer[pointer]) = Narumikazuchi.Generators.ByteSerialization.TypeIdentifier.CreateFrom(typeof(System.Collections.Generic.List<Person>[,]));
            pointer += Unsafe.SizeOf<Narumikazuchi.Generators.ByteSerialization.TypeIdentifier>();
        }
        else
        {
            buffer[pointer++] = 0x1;
            Unsafe.As<Byte, Narumikazuchi.Generators.ByteSerialization.TypeIdentifier>(ref buffer[pointer]) = Narumikazuchi.Generators.ByteSerialization.TypeIdentifier.CreateFrom(typeof(System.Collections.Generic.List<Person>[,]));
            pointer += Unsafe.SizeOf<Narumikazuchi.Generators.ByteSerialization.TypeIdentifier>();
            Unsafe.As<Byte, Int32>(ref buffer[pointer]) = value.GetLength(0);
            pointer += sizeof(Int32);
            Unsafe.As<Byte, Int32>(ref buffer[pointer]) = value.GetLength(1);
            pointer += sizeof(Int32);
            foreach (var _var0 in value)
            {
                if (_var0 is null)
                {
                    buffer[pointer++] = 0x0;
                    Unsafe.As<Byte, Narumikazuchi.Generators.ByteSerialization.TypeIdentifier>(ref buffer[pointer]) = Narumikazuchi.Generators.ByteSerialization.TypeIdentifier.CreateFrom(typeof(System.Collections.Generic.List<Person>));
                    pointer += Unsafe.SizeOf<Narumikazuchi.Generators.ByteSerialization.TypeIdentifier>();
                }
                else
                {
                    buffer[pointer++] = 0x1;
                    Unsafe.As<Byte, Narumikazuchi.Generators.ByteSerialization.TypeIdentifier>(ref buffer[pointer]) = Narumikazuchi.Generators.ByteSerialization.TypeIdentifier.CreateFrom(typeof(System.Collections.Generic.List<Person>));
                    pointer += Unsafe.SizeOf<Narumikazuchi.Generators.ByteSerialization.TypeIdentifier>();
                    Unsafe.As<Byte, Int32>(ref buffer[pointer]) = _var0.Capacity;
                    pointer += Unsafe.SizeOf<Int32>();
                    Unsafe.As<Byte, Int32>(ref buffer[pointer]) = _var0.Count;
                    pointer += sizeof(Int32);
                    foreach (var _var1 in _var0)
                    {
                        if (_var1 is null)
                        {
                            buffer[pointer++] = 0x0;
                            Unsafe.As<Byte, Narumikazuchi.Generators.ByteSerialization.TypeIdentifier>(ref buffer[pointer]) = Narumikazuchi.Generators.ByteSerialization.TypeIdentifier.CreateFrom(typeof(Person));
                            pointer += Unsafe.SizeOf<Narumikazuchi.Generators.ByteSerialization.TypeIdentifier>();
                        }
                        else
                        {
                            buffer[pointer++] = 0x1;
                            Unsafe.As<Byte, Narumikazuchi.Generators.ByteSerialization.TypeIdentifier>(ref buffer[pointer]) = Narumikazuchi.Generators.ByteSerialization.TypeIdentifier.CreateFrom(typeof(Person));
                            pointer += Unsafe.SizeOf<Narumikazuchi.Generators.ByteSerialization.TypeIdentifier>();
                            if (_var1.FirstName is null)
                            {
                                buffer[pointer++] = 0x0;
                                Unsafe.As<Byte, Narumikazuchi.Generators.ByteSerialization.TypeIdentifier>(ref buffer[pointer]) = Narumikazuchi.Generators.ByteSerialization.TypeIdentifier.CreateFrom(typeof(String));
                                pointer += Unsafe.SizeOf<Narumikazuchi.Generators.ByteSerialization.TypeIdentifier>();
                            }
                            else
                            {
                                buffer[pointer++] = 0x1;
                                Unsafe.As<Byte, Narumikazuchi.Generators.ByteSerialization.TypeIdentifier>(ref buffer[pointer]) = Narumikazuchi.Generators.ByteSerialization.TypeIdentifier.CreateFrom(typeof(String));
                                pointer += Unsafe.SizeOf<Narumikazuchi.Generators.ByteSerialization.TypeIdentifier>();
                                var _var2 = MemoryMarshal.AsBytes(_var1.FirstName.AsSpan());
                                Unsafe.As<Byte, Int32>(ref buffer[pointer]) = _var2.Length;
                                pointer += sizeof(Int32);
                                _var2.CopyTo(buffer[pointer..]);
                                pointer += _var2.Length;
                            }
                            if (_var1.LastName is null)
                            {
                                buffer[pointer++] = 0x0;
                                Unsafe.As<Byte, Narumikazuchi.Generators.ByteSerialization.TypeIdentifier>(ref buffer[pointer]) = Narumikazuchi.Generators.ByteSerialization.TypeIdentifier.CreateFrom(typeof(String));
                                pointer += Unsafe.SizeOf<Narumikazuchi.Generators.ByteSerialization.TypeIdentifier>();
                            }
                            else
                            {
                                buffer[pointer++] = 0x1;
                                Unsafe.As<Byte, Narumikazuchi.Generators.ByteSerialization.TypeIdentifier>(ref buffer[pointer]) = Narumikazuchi.Generators.ByteSerialization.TypeIdentifier.CreateFrom(typeof(String));
                                pointer += Unsafe.SizeOf<Narumikazuchi.Generators.ByteSerialization.TypeIdentifier>();
                                var _var3 = MemoryMarshal.AsBytes(_var1.LastName.AsSpan());
                                Unsafe.As<Byte, Int32>(ref buffer[pointer]) = _var3.Length;
                                pointer += sizeof(Int32);
                                _var3.CopyTo(buffer[pointer..]);
                                pointer += _var3.Length;
                            }
                            Unsafe.As<Byte, DateOnly>(ref buffer[pointer]) = _var1.DateOfBirth;
                            pointer += Unsafe.SizeOf<DateOnly>();
                            Unsafe.As<Byte, Guid>(ref buffer[pointer]) = _var1.CustomerId;
                            pointer += Unsafe.SizeOf<Guid>();
                        }
                    }
                }
            }
        }
        Unsafe.As<Byte, Int32>(ref buffer[0]) = pointer;
        return (UInt32)pointer;
    }
}";
}