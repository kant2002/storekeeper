// -----------------------------------------------------------------------
// <copyright file="Generator.cs" company="Andrii Kurdiumov">
// Copyright (c) Andrii Kurdiumov. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace StoreKeeper;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

/// <summary>
/// Generator for IOC storage.
/// </summary>
[Generator]
public class Generator : ISourceGenerator
{
    /// <inheritdoc/>
    public void Initialize(GeneratorInitializationContext context)
    {
        // context.RegisterForPostInitialization((pi) => pi.AddSource("SqlMarshalAttribute.cs", AttributeSource));
        context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
    }

    /// <inheritdoc/>
    public void Execute(GeneratorExecutionContext context)
    {
        // Retrieve the populated receiver
        if (!(context.SyntaxContextReceiver is SyntaxReceiver receiver))
        {
            return;
        }

        var hasNullableAnnotations = context.Compilation.Options.NullableContextOptions != NullableContextOptions.Disable;

        var sourceCode = string.Empty;
        foreach (var i in receiver.Methods)
        {
            var name = i.Name;
            if (name is GenericNameSyntax genericNameSyntax)
            {
                var identifier = genericNameSyntax.Identifier;
                var typesList = genericNameSyntax.TypeArgumentList.Arguments;
                var interfaceType = typesList[0];
                var implementationType = typesList.ElementAtOrDefault(1) ?? interfaceType;
                sourceCode += $"// {identifier.ToString().Substring(3)} {interfaceType} {implementationType}";
            }
            else
            {
                sourceCode += "// Unknown!!! " + name.ToFullString() + Environment.NewLine;
            }
        }

        context.AddSource($"ioc_constructor.cs", SourceText.From(sourceCode, Encoding.UTF8));
    }

    internal class SyntaxReceiver : ISyntaxContextReceiver
    {
        public List<MemberAccessExpressionSyntax> Methods { get; } = new List<MemberAccessExpressionSyntax>();

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            // any field with at least one attribute is a candidate for property generation
            if (context.Node is MemberAccessExpressionSyntax invocationExpressionSyntax)
            {
                var x = invocationExpressionSyntax.Name.ToString();
                if (x.StartsWith("AddScoped"))
                {
                    // Get the symbol being declared by the field, and keep it if its annotated
                    this.Methods.Add(invocationExpressionSyntax);
                }
            }
        }
    }
}
