﻿using Childrens_Social_Care_CPD.Contentful.Renderers;
using Contentful.Core.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.WebEncoders.Testing;
using NSubstitute;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace Childrens_Social_Care_CPD_Tests.Contentful.Renderers;

public class Heading5RendererTests
{
    private IRenderer<Text> _textRenderer;
    private IRenderer<Hyperlink> _hyperlinkRenderer;
    private Heading5Renderer _sut;

    [SetUp]
    public void Setup()
    {
        _textRenderer = Substitute.For<IRenderer<Text>>();
        _hyperlinkRenderer = Substitute.For<IRenderer<Hyperlink>>();
        _sut = new Heading5Renderer(_textRenderer, _hyperlinkRenderer);
    }

    [Test]
    public void Heading5_Renders()
    {
        // arrange
        var theText = new Text()
        {
            Value = "Foo",
            Marks = new List<Mark>()
        };
        var heading5 = new Heading5()
        {
            Content = new List<IContent> { theText }
        };

        _textRenderer.Render(theText).Returns(new HtmlString("AAA"));
        var stringWriter = new StringWriter();

        // act
        var htmlContent = _sut.Render(heading5);
        htmlContent.WriteTo(stringWriter, new HtmlTestEncoder());
        var actual = stringWriter.ToString();

        // assert
        actual.Should().Be("<h5>AAA</h5>");
    }

    [Test]
    public void Heading5_Renders_Hyperlink()
    {
        // arrange
        var stringWriter = new StringWriter();
        var foo = new Hyperlink()
        {
            Data = new HyperlinkData
            {
                Title = "foo",
                Uri = "bar"
            }
        };
        var heading5 = new Heading5()
        {
            Content = new List<IContent> { foo }
        };

        _hyperlinkRenderer.Render(foo).Returns(new HtmlString("AAA"));

        // act
        var htmlContent = _sut.Render(heading5);
        htmlContent.WriteTo(stringWriter, new HtmlTestEncoder());
        var actual = stringWriter.ToString();

        // assert
        actual.Should().Be("<h5>AAA</h5>");
    }
}
