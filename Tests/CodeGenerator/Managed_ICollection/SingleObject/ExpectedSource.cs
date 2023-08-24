﻿using Microsoft.CodeAnalysis.Text;

namespace Tests.CodeGenerator.Managed_ICollection.SingleObject;

static public class ExpectedSource
{
    static public (String filename, SourceText source) Handler
    {
        get
        {
            return (HANDLER_FILENAME, s_HandlerSource!);
        }
    }

    static public readonly SourceText s_HandlerSource = SourceText.From(text: HANDLER_SOURCE,
                                                                        encoding: Encoding.UTF8);

    public const String HANDLER_FILENAME = @"Narumikazuchi.Generators.ByteSerialization.Core\Narumikazuchi.Generators.ByteSerialization.Generators.SerializableGenerator\Narumikazuchi.Generated.Internals.ByteSerialization.Handler.System.Collections.Generic.List`1[Person].g.cs";

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

public partial interface IAssemblyHandler_TestProject : Narumikazuchi.Generators.ByteSerialization.ISerializationHandler<System.Collections.Generic.List<Person>>
{
    [CompilerGenerated]
    UInt32 Narumikazuchi.Generators.ByteSerialization.ISerializationHandler<System.Collections.Generic.List<Person>>.Deserialize(ReadOnlySpan<Byte> buffer, out System.Collections.Generic.List<Person> result)
    {
        var pointer = sizeof(Int32);
        result = default(System.Collections.Generic.List<Person>);
        if (buffer[pointer++] == 0x1)
        {
            var _var0 = Unsafe.As<Byte, Narumikazuchi.Generators.ByteSerialization.TypeIdentifier>(ref MemoryMarshal.GetReference(buffer[pointer..]));
            pointer += Unsafe.SizeOf<Narumikazuchi.Generators.ByteSerialization.TypeIdentifier>();
            var _var1 = Unsafe.As<Byte, Int32>(ref MemoryMarshal.GetReference(buffer[pointer..]));
            pointer += Unsafe.SizeOf<Int32>();
            result = new System.Collections.Generic.List<Person>()
            {
                Capacity = _var1,
            };
            var _var2 = Unsafe.As<Byte, Int32>(ref MemoryMarshal.GetReference(buffer[pointer..]));
            pointer += sizeof(Int32);
            for (var _var3 = 0; _var3 < _var2; _var3++)
            {
                var _var4 = default(Person);
                if (buffer[pointer++] == 0x1)
                {
                    var _var5 = Unsafe.As<Byte, Narumikazuchi.Generators.ByteSerialization.TypeIdentifier>(ref MemoryMarshal.GetReference(buffer[pointer..]));
                    pointer += Unsafe.SizeOf<Narumikazuchi.Generators.ByteSerialization.TypeIdentifier>();
                    var _var6 = default(String);
                    if (buffer[pointer++] == 0x1)
                    {
                        var _var7 = Unsafe.As<Byte, Narumikazuchi.Generators.ByteSerialization.TypeIdentifier>(ref MemoryMarshal.GetReference(buffer[pointer..]));
                        pointer += Unsafe.SizeOf<Narumikazuchi.Generators.ByteSerialization.TypeIdentifier>();
                        var _var8 = Unsafe.As<Byte, Int32>(ref MemoryMarshal.GetReference(buffer[pointer..]));
                        pointer += Unsafe.SizeOf<Int32>();
                        _var6 = new String(MemoryMarshal.Cast<Byte, Char>(buffer[pointer..(pointer + _var8)]));
                        pointer += _var8;
                    }
                    else
                    {
                        pointer += Unsafe.SizeOf<Narumikazuchi.Generators.ByteSerialization.TypeIdentifier>();
                    }
                    var _var9 = default(String);
                    if (buffer[pointer++] == 0x1)
                    {
                        var _var10 = Unsafe.As<Byte, Narumikazuchi.Generators.ByteSerialization.TypeIdentifier>(ref MemoryMarshal.GetReference(buffer[pointer..]));
                        pointer += Unsafe.SizeOf<Narumikazuchi.Generators.ByteSerialization.TypeIdentifier>();
                        var _var11 = Unsafe.As<Byte, Int32>(ref MemoryMarshal.GetReference(buffer[pointer..]));
                        pointer += Unsafe.SizeOf<Int32>();
                        _var9 = new String(MemoryMarshal.Cast<Byte, Char>(buffer[pointer..(pointer + _var11)]));
                        pointer += _var11;
                    }
                    else
                    {
                        pointer += Unsafe.SizeOf<Narumikazuchi.Generators.ByteSerialization.TypeIdentifier>();
                    }
                    var _var12 = Unsafe.As<Byte, DateOnly>(ref MemoryMarshal.GetReference(buffer[pointer..]));
                    pointer += Unsafe.SizeOf<DateOnly>();
                    var _var13 = Unsafe.As<Byte, Guid>(ref MemoryMarshal.GetReference(buffer[pointer..]));
                    pointer += Unsafe.SizeOf<Guid>();
                    _var4 = new Person()
                    {
                        FirstName = _var6,
                        LastName = _var9,
                        DateOfBirth = _var12,
                        CustomerId = _var13,
                    };
                }
                else
                {
                    pointer += Unsafe.SizeOf<Narumikazuchi.Generators.ByteSerialization.TypeIdentifier>();
                }
                result.Add(_var4);
            }
        }
        else
        {
            pointer += Unsafe.SizeOf<Narumikazuchi.Generators.ByteSerialization.TypeIdentifier>();
        }
        return (UInt32)pointer;
    }

    [CompilerGenerated]
    Int32 Narumikazuchi.Generators.ByteSerialization.ISerializationHandler<System.Collections.Generic.List<Person>>.GetExpectedArraySize(System.Collections.Generic.List<Person> value)
    {
        var size = sizeof(Int64) + Unsafe.SizeOf<Narumikazuchi.Generators.ByteSerialization.TypeIdentifier>();
        size += sizeof(Byte) + Unsafe.SizeOf<Narumikazuchi.Generators.ByteSerialization.TypeIdentifier>();
        if (value is not null)
        {
            size += Unsafe.SizeOf<Int32>();
            foreach (var _var0 in value)
            {
                size += sizeof(Byte) + Unsafe.SizeOf<Narumikazuchi.Generators.ByteSerialization.TypeIdentifier>();
                if (_var0 is not null)
                {
                    size += sizeof(Byte) + Unsafe.SizeOf<Narumikazuchi.Generators.ByteSerialization.TypeIdentifier>();
                    size += 4 * _var0.FirstName.Length;
                    size += sizeof(Byte) + Unsafe.SizeOf<Narumikazuchi.Generators.ByteSerialization.TypeIdentifier>();
                    size += 4 * _var0.LastName.Length;
                    size += Unsafe.SizeOf<DateOnly>();
                    size += Unsafe.SizeOf<Guid>();
                }
            }
        }
        return size;
    }

    [CompilerGenerated]
    UInt32 Narumikazuchi.Generators.ByteSerialization.ISerializationHandler<System.Collections.Generic.List<Person>>.Serialize(Span<Byte> buffer, System.Collections.Generic.List<Person> value)
    {
        var pointer = sizeof(Int32);
        if (value is null)
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
            Unsafe.As<Byte, Int32>(ref buffer[pointer]) = value.Capacity;
            pointer += Unsafe.SizeOf<Int32>();
            Unsafe.As<Byte, Int32>(ref buffer[pointer]) = value.Count;
            pointer += sizeof(Int32);
            foreach (var _var0 in value)
            {
                if (_var0 is null)
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
                    if (_var0.FirstName is null)
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
                        var _var1 = MemoryMarshal.AsBytes(_var0.FirstName.AsSpan());
                        Unsafe.As<Byte, Int32>(ref buffer[pointer]) = _var1.Length;
                        pointer += sizeof(Int32);
                        _var1.CopyTo(buffer[pointer..]);
                        pointer += _var1.Length;
                    }
                    if (_var0.LastName is null)
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
                        var _var2 = MemoryMarshal.AsBytes(_var0.LastName.AsSpan());
                        Unsafe.As<Byte, Int32>(ref buffer[pointer]) = _var2.Length;
                        pointer += sizeof(Int32);
                        _var2.CopyTo(buffer[pointer..]);
                        pointer += _var2.Length;
                    }
                    Unsafe.As<Byte, DateOnly>(ref buffer[pointer]) = _var0.DateOfBirth;
                    pointer += Unsafe.SizeOf<DateOnly>();
                    Unsafe.As<Byte, Guid>(ref buffer[pointer]) = _var0.CustomerId;
                    pointer += Unsafe.SizeOf<Guid>();
                }
            }
        }
        Unsafe.As<Byte, Int32>(ref buffer[0]) = pointer;
        return (UInt32)pointer;
    }
}";
}