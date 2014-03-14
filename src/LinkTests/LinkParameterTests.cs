using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tavis;
using Xunit;

namespace LinkTests
{
    public class LinkParameterTests
    {
        [Fact]
        public void Add_parameters_to_uri_without_query_string()
        {
            var link = new Link()
            {
                Target = new Uri("http://example/customer")
            };

            link.SetParameter("id",99);

            link.AddParametersAsTemplate();

            var resolvedTarget = link.GetResolvedTarget().OriginalString;
            Assert.Equal("http://example/customer?id=99", resolvedTarget);
        }


        [Fact]
        public void Add_parameters_to_uri_with_query_string()
        {
            var link = new Link()
            {
                Target = new Uri("http://example/customer?view=true")
            };

            link.SetParameter("id", 99);

            link.AddParametersAsTemplate();

            var resolvedTarget = link.GetResolvedTarget().OriginalString;
            Assert.Equal("http://example/customer?view=true&id=99", resolvedTarget);
        }


        // TODO Not sure how to resolve this.  How do I know not to create a query string param with the id?
        // I could regex into the path, but that's just ugly.
        [Fact]
        public void Add_parameters_to_uri_with_query_string_ignoring_path_parameter()
        {
            var link = new Link()
            {
                Target = new Uri("http://example/customer/{id}?view=true")
            };

            link.SetParameter("id", 99);
            link.SetParameter("context", "detail");

            link.AddParametersAsTemplate();

            var resolvedTarget = link.GetResolvedTarget().OriginalString;
            Assert.Equal("http://example/customer/99?view=true&context=detail", resolvedTarget);
        }

        [Fact]
        public void Update_existing_parameters_in_query_string_automatically()
        {
            var link = new Link()
            {
                Target = new Uri("http://example/customer?view=true"),
                AddNonTemplatedParametersToQueryString = true
            };

            link.SetParameter("view", false);


            var resolvedTarget = link.GetResolvedTarget().OriginalString;
            Assert.Equal("http://example/customer?view=False", resolvedTarget);
        }


        [Fact]
        public void Update_existing_parameters_in_query_string()
        {
            var link = new Link()
            {
                Target = new Uri("http://example/customer?view=true")
            };

            link.SetParameter("view", false);

            link.AddParametersAsTemplate();

            var resolvedTarget = link.GetResolvedTarget().OriginalString;
            Assert.Equal("http://example/customer?view=False", resolvedTarget);
        }

        [Fact]
        public void Add_multiple_parameters_to_uri()
        {
            var link = new Link()
            {
                Target = new Uri("http://example/customer")
            };

            link.SetParameter("id", 99);
            link.SetParameter("view", false);

            link.AddParametersAsTemplate();

            var resolvedTarget = link.GetResolvedTarget().OriginalString;
            Assert.Equal("http://example/customer?id=99&view=False", resolvedTarget);
        }


        [Fact]
        public void Add_no_parameters_to_uri()
        {
            var link = new Link()
            {
                Target = new Uri("http://example/customer")
            };


            link.AddParametersAsTemplate();

            var resolvedTarget = link.GetResolvedTarget().OriginalString;
            Assert.Equal("http://example/customer", resolvedTarget);
        }

        [Fact]
        public void Change_an_existing_parameter()
        {
            var link = new Link()
            {
                Target = new Uri("http://example/customer?view=False")
            };

            link.SetParameter("view",true);

            link.AddParametersAsTemplate(true);

            var resolvedTarget = link.GetResolvedTarget().OriginalString;
            Assert.Equal("http://example/customer?view=True", resolvedTarget);
        }


        [Fact]
        public void Change_an_existing_parameter_within_multiple()
        {
            var link = new Link()
            {
                Target = new Uri("http://example/customer?view=False&foo=bar")
            };

            link.CreateParametersFromQueryString();

            link.SetParameter("view", true);

            link.AddParametersAsTemplate(true);

            var resolvedTarget = link.GetResolvedTarget().OriginalString;
            Assert.Equal("http://example/customer?view=True&foo=bar", resolvedTarget);
        }
    }
}
