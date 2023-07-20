﻿using Childrens_Social_Care_CPD.Constants;
using Childrens_Social_Care_CPD.Contentful;
using Childrens_Social_Care_CPD.Contentful.Models;
using Childrens_Social_Care_CPD.Interfaces;
using Childrens_Social_Care_CPD_Tests.Contentful;
using Contentful.Core.Models;
using Contentful.Core.Search;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Childrens_Social_Care_CPD_Tests.Controllers;

public class ContentControllerServerTests
{
    private CpdTestServerApplication _application;
    private HttpClient _httpClient;
    private static string ContentUrl = "/content";

    [SetUp]
    public void SetUp()
    {
        _application = new CpdTestServerApplication();
        _httpClient = _application.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Test]
    public async Task Content_Will_Contain_Warning_If_Data_Is_Self_Referential()
    {
        // arrange
        var content = new Content();
        content.Items = new List<IContent> { content };
        var contentCollection = new ContentfulCollection<Content>() { Items = new List<Content>() { content } };
        _application.CpdContentfulClient.GetEntries(Arg.Any<QueryBuilder<Content>>(), default).Returns(contentCollection);

        // act
        var response = await _httpClient.GetAsync(ContentUrl);
        var responseContent = await response.Content.ReadAsStringAsync();
        
        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        responseContent.Should().Contain("You have a circular link in your content");
    }

    [Test]
    public async Task Content_Will_Contain_Warning_If_Data_Has_An_Unknown_Content_Type()
    {
        // arrange
        var content = new Content();
        content.Items = new List<IContent> { new TestingContentItem() };
        var contentCollection = new ContentfulCollection<Content>() { Items = new List<Content>() { content } };
        _application.CpdContentfulClient.GetEntries(Arg.Any<QueryBuilder<Content>>(), default).Returns(contentCollection);

        // act
        var response = await _httpClient.GetAsync(ContentUrl);
        var responseContent = await response.Content.ReadAsStringAsync();

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        responseContent.Should().Contain("You have used an unkown content type");
    }
}
