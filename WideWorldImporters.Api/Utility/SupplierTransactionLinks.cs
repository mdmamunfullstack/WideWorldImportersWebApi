using System;
using System.Collections.Generic;
using System.Linq;
using Contracts;
using Entities.DataTransferObjects;
using Entities.LinkModels;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Net.Http.Headers;
using WideWorldImportersWebApi.Controllers;

namespace WideWorldImportersWebApi.Utility
{
    public class SupplierTransactionLinks
    {
        private readonly IDataShaper<SupplierTransactionDto> _dataShaper;
        private readonly LinkGenerator _linkGenerator;

        public SupplierTransactionLinks(LinkGenerator linkGenerator, IDataShaper<SupplierTransactionDto> dataShaper)
        {
            _linkGenerator = linkGenerator;
            _dataShaper = dataShaper;
        }

        public LinkResponse TryGenerateLinks(IEnumerable<SupplierTransactionDto> transactionsDto, string fields, int supplierId, HttpContext httpContext)
        {
            var shapedTransactions = ShapeData(transactionsDto, fields);

            if (ShouldGenerateLinks(httpContext))
            {
                return ReturnLinkedTransactions(transactionsDto, fields, supplierId, httpContext, shapedTransactions);
            }

            return ReturnShapedTransactions(shapedTransactions);
        }

        private List<Entity> ShapeData(IEnumerable<SupplierTransactionDto> transactionsDto, string fields)
        {
            return _dataShaper.ShapeData(transactionsDto, fields)
                              .Select(e => e.Entity)
                              .ToList();
        }

        private bool ShouldGenerateLinks(HttpContext httpContext)
        {
            var mediaType = (MediaTypeHeaderValue) httpContext.Items["AcceptHeaderMediaType"];

            return mediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);
        }

        private LinkResponse ReturnShapedTransactions(List<Entity> shapedTransactions) { return new LinkResponse {ShapedEntities = shapedTransactions}; }

        private LinkResponse ReturnLinkedTransactions(IEnumerable<SupplierTransactionDto> transactionsDto, string fields, int supplierId, HttpContext httpContext, List<Entity> shapedTransactions)
        {
            var transactionDtoList = transactionsDto.ToList();

            for (var index = 0; index < transactionDtoList.Count(); index++)
            {
                var transactionLinks = CreateLinksForTransaction(httpContext, supplierId, transactionDtoList[index].SupplierTransactionId, fields);
                shapedTransactions[index].Add("Links", transactionLinks);
            }

            var transactionCollection = new LinkCollectionWrapper<Entity>(shapedTransactions);
            var linkedTransactions = CreateLinksForTransactions(httpContext, transactionCollection);

            return new LinkResponse {HasLinks = true, LinkedEntities = linkedTransactions};
        }

        private List<Link> CreateLinksForTransaction(HttpContext httpContext, int supplierId, int id, string fields = "")
        {
            var links = new List<Link>
                        {
                            // https://resharper-support.jetbrains.com/hc/en-us/community/posts/360010825819-Cannot-resolve-action-in-ASP-NET-Core-controller-when-calling-Url-Action-
                            // cannot resolve action for "GetTransactionForSupplier" is a ReSharper bug
                            new Link(_linkGenerator.GetUriByAction(httpContext, "GetTransactionForSupplier", values: new {supplierId, id}),
                                     "self",
                                     "GET"),
                            new Link(_linkGenerator.GetUriByAction(httpContext, "DeleteSupplierTransaction", values: new {supplierId, id}),
                                     "delete_transaction",
                                     "DELETE"),
                            new Link(_linkGenerator.GetUriByAction(httpContext, "UpdateSupplierTransaction", values: new {supplierId, id}),
                                     "update_transaction",
                                     "PUT"),
                            new Link(_linkGenerator.GetUriByAction(httpContext, "PartiallyUpdateSupplierTransaction", values: new {supplierId, id}),
                                     "partially_update_transaction",
                                     "PATCH")
                        };

            return links;
        }

        private LinkCollectionWrapper<Entity> CreateLinksForTransactions(HttpContext httpContext, LinkCollectionWrapper<Entity> transactionsWrapper)
        {
            transactionsWrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(httpContext, "GetSupplierTransactions", values: new { }),
                                                   "self",
                                                   "GET"));

            return transactionsWrapper;
        }
    }
}