﻿using Microsoft.CodeAnalysis.Text;

namespace Tests.CodeGenerator;

static public class AssemblySource
{
    static public (String, SourceText) ExpectedSource
    {
        get
        {
            return (ASSEMBLY_FILENAME, s_ExpectedAssemblySource!);
        }
    }

    static public readonly SourceText s_ExpectedAssemblySource = SourceText.From(text: ASSEMBLY_SOURCE,
                                                                                 encoding: Encoding.UTF8);

    public const String ASSEMBLY_FILENAME = @"Narumikazuchi.Generators.ByteSerialization.Core\Narumikazuchi.Generators.ByteSerialization.Generators.SerializableGenerator\Narumikazuchi.Generated.Internals.ByteSerialization.AssemblyHandler_TestProject.g.cs";

    public const String ASSEMBLY_SOURCE = @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
#pragma warning disable
#nullable enable

using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Narumikazuchi.Generated.Internals.ByteSerialization;

[EditorBrowsable(EditorBrowsableState.Never)]
[CompilerGenerated]
public partial interface IAssemblyHandler_TestProject :
    Narumikazuchi.Generators.ByteSerialization.IByteSerializer
{
    UInt32 Narumikazuchi.Generators.ByteSerialization.IByteSerializer.Variant
    {
        get
        {
            return 0;
        }
    }
}

[EditorBrowsable(EditorBrowsableState.Never)]
[CompilerGenerated]
public sealed class AssemblyHandler_TestProject : IAssemblyHandler_TestProject
{
    public AssemblyHandler_TestProject()
    { }
}";
}