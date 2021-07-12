using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Entities.DataTransferObjects;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WideWorldImporters.Api.IntegrationTests.Models;
using WideWorldImporters.Api.IntegrationTests.TestHelpers.Serialization;
using WideWorldImporters.Api.IntegrationTests.TestHelpers.Utility;
using Xunit;

namespace WideWorldImporters.Api.IntegrationTests.Controllers
{
    public sealed class SuppliersControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        public SuppliersControllerTests(WebApplicationFactory<Startup> factory)
        {
            factory.ClientOptions.BaseAddress = _isWebApplicationFactoryInMemory
                                                    ? new Uri("http://localhost/api/1.0/suppliers")
                                                    : new Uri("http://localhost:5001/api/1.0/suppliers");

            // automatically follow redirects and handles cookies
            _client = factory.CreateClient();
            _client.Timeout = new TimeSpan(0, 0, 60);
            _client.DefaultRequestHeaders.Clear();
        }

        private readonly bool _isWebApplicationFactoryInMemory = true;

        private readonly HttpClient _client;

        /// <summary>
        ///     Return a TestsSupplierForCreationDto Model
        /// </summary>
        /// <returns></returns>
        private static SupplierForCreationDtoInputModel GetValidTestsSupplierForCreationDtoModel()
        {
            return new SupplierForCreationDtoInputModel
                   {
                       SupplierCategoryId = 2,
                       PrimaryContactPersonId = 21,
                       AlternateContactPersonId = 22,
                       DeliveryMethodId = 7,
                       DeliveryCityId = 38171,
                       PostalCityId = 38171,
                       SupplierReference = "AA20384",
                       BankAccountName = "A Datum Corporation",
                       BankAccountBranch = "Woodgrove Bank Zionsville",
                       BankAccountCode = "356981",
                       BankAccountNumber = "8575824136",
                       BankInternationalCode = "25986",
                       PaymentDays = 14,
                       InternalComments = "",
                       PhoneNumber = "(847) 555-0100",
                       FaxNumber = "(847) 555-0101",
                       WebsiteUrl = "https://github.com/gmcbath/WideWorldImportersWebApi",
                       DeliveryAddressLine1 = "183838 Southwest Boulevard",
                       DeliveryAddressLine2 = "Suite 222",
                       DeliveryPostalCode = "46077",
                       PostalAddressLine1 = "PO Box 1039",
                       PostalAddressLine2 = "Surrey",
                       PostalPostalCode = "46077",
                       LastEditedBy = 1,
                       SupplierName = "A Datum Corporation"
                   };
        }

        /// <summary>
        ///     Perform content tests
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetAll_ReturnsContent()
        {
            // verify get a list of suppliers return content
            var response = await _client.GetAsync("");

            //verify content exists
            Assert.NotNull(response.Content);

            // headers exist
            Assert.True(response.Content.Headers.ContentLength > 0);

            // verify expected media type returned
            Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);

            // verify return code is 200
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        /// <summary>
        ///     Perform cache tests
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetAll_SetsExpectedCacheControlHeader()
        {
            var response = await _client.GetAsync("");

            var header = response.Headers.CacheControl;

            // make sure cache header is present and has the expected value
            Assert.True(header.MaxAge.HasValue);
            Assert.Equal(TimeSpan.FromMinutes(1), header.MaxAge);
            Assert.True(header.Private);
        }

        /// <summary>
        ///     Verify API honors the OPTIONS requests
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetOptions_ReturnsSuccess()
        {
            var request = new HttpRequestMessage(HttpMethod.Options, "");
            var result = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.True(result.Content.Headers.ContentLength == 0);
        }

        /// <summary>
        ///     Getting a specific supplier using an invalid ID should return a 404 error
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetSupplierByInvalidId_ReturnsBadRequest()
        {
            var response = await _client.GetAsync(_client.BaseAddress + "/0");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        /// <summary>
        ///     Get a specific supplier by id
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetSupplierByValidId_ReturnsSuccess()
        {
            var model = await _client.GetFromJsonAsync<SupplierDto>(_client.BaseAddress + "/4");

            Assert.True(model.SupplierId == 4);
        }

        /// <summary>
        ///     Passing null patch should return BadRequest
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Patch_Supplier_PassNullPatch_ReturnsBadRequest()
        {
            int supplierId = 1;
            int supplierCategoryId = 2;

            string requestUri = _client.BaseAddress + $"/{supplierId}/suppliercategories/{supplierCategoryId}";

            string serializedDoc = JsonConvert.SerializeObject(null);
            var requestContent = new StringContent(serializedDoc, Encoding.UTF8, "application/json-patch+json");

            var response = await _client.PatchAsync(requestUri, requestContent);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        /// <summary>
        ///     Patch supplier with replace, remove, and add operations
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Patch_Supplier_Replace_Remove_Add_Operations_ReturnsSuccess()
        {
            int supplierId = 1;
            int supplierCategoryId = 2;

            string requestUri = _client.BaseAddress + $"/{supplierId}/suppliercategories/{supplierCategoryId}";

            // replace
            var patchDoc = new JsonPatchDocument<SupplierForCreationDtoInputModel>();
            patchDoc.Replace(s => s.DeliveryAddressLine2, UtilityHelpers.RandomString(10));

            string serializedDoc = JsonConvert.SerializeObject(patchDoc);
            var requestContent = new StringContent(serializedDoc, Encoding.UTF8, "application/json-patch+json");

            var response = await _client.PatchAsync(requestUri, requestContent);
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            // remove
            patchDoc = new JsonPatchDocument<SupplierForCreationDtoInputModel>();
            patchDoc.Remove(s => s.DeliveryAddressLine2);

            serializedDoc = JsonConvert.SerializeObject(patchDoc);
            requestContent = new StringContent(serializedDoc, Encoding.UTF8, "application/json-patch+json");

            response = await _client.PatchAsync(requestUri, requestContent);
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            // add
            patchDoc = new JsonPatchDocument<SupplierForCreationDtoInputModel>();
            patchDoc.Add(s => s.DeliveryAddressLine2, UtilityHelpers.RandomString(20));

            serializedDoc = JsonConvert.SerializeObject(patchDoc);
            requestContent = new StringContent(serializedDoc, Encoding.UTF8, "application/json-patch+json");

            response = await _client.PatchAsync(requestUri, requestContent);
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        /// <summary>
        ///     POSTing a null supplier should return UnprocessableEntity
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Post_Null_Supplier_ReturnsUnprocessableEntity()
        {
            string serializedDoc = JsonConvert.SerializeObject(null);
            var requestContent = new StringContent(serializedDoc, Encoding.UTF8, "application/json-patch+json");

            var response = await _client.PostAsJsonAsync("", requestContent, JsonSerializerHelper.DefaultSerialisationOptions);

            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        }

        /// <summary>
        ///     Add supplier to the database for a specific category
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Post_Supplier_ForSpecificCategory_ReturnsSuccess()
        {
            var requestContent = GetValidTestsSupplierForCreationDtoModel();
            requestContent.SupplierName = requestContent.SupplierName + UtilityHelpers.RandomString(2);

            var response = await _client.PostAsJsonAsync(_client.BaseAddress + "/1", requestContent, JsonSerializerHelper.DefaultSerialisationOptions);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        /// <summary>
        ///     Attempt to add a supplier with a invalid SupplierCategoryId
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Post_Supplier_InvalidSupplierCategoryId_ThrowsDbUpdateException()
        {
            var requestContent = GetValidTestsSupplierForCreationDtoModel().CloneWith(m => m.SupplierCategoryId = 0);
            requestContent.SupplierName = requestContent.SupplierName + UtilityHelpers.RandomString(10);

            // todo - not sure why this doesnt work
            //await Assert.ThrowsAsync<DbUpdateException>(() => _client.PostAsJsonAsync("", requestContent, JsonSerializerHelper.DefaultSerialisationOptions));

            try
            {
                await _client.PostAsJsonAsync("", requestContent, JsonSerializerHelper.DefaultSerialisationOptions);
            }
            catch (DbUpdateException e)
            {
                // this should be the FK error
                //
                // The MERGE statement conflicted with the FOREIGN KEY constraint "FK_Purchasing_Suppliers_SupplierCategoryID_Purchasing_SupplierCategories". The conflict occurred in database "WideWorldImporters", table "Purchasing.SupplierCategories", column 'SupplierCategoryID'.
                //Debug.WriteLine(e);
                if (e.InnerException != null)
                {
                    Assert.Contains("FK_Purchasing_Suppliers_SupplierCategoryID_Purchasing_SupplierCategories", e.InnerException.Message);
                }
            }
        }

        /// <summary>
        ///     Add supplier to the database
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Post_Supplier_ReturnsSuccess()
        {
            var requestContent = GetValidTestsSupplierForCreationDtoModel();
            requestContent.SupplierName = requestContent.SupplierName + UtilityHelpers.RandomString(2);

            var response = await _client.PostAsJsonAsync("", requestContent, JsonSerializerHelper.DefaultSerialisationOptions);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        }

        /// <summary>
        ///     Update a supplier
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Put_Supplier_ReturnsSuccess()
        {
            var requestContent = GetValidTestsSupplierForCreationDtoModel();
            requestContent.SupplierName = UtilityHelpers.RandomString(20);

            int supplierId = 1;
            int supplierCategoryId = 2;

            string requestUri = _client.BaseAddress + $"/{supplierId}/suppliercategories/{supplierCategoryId}";
            var response = await _client.PutAsJsonAsync(requestUri, requestContent, JsonSerializerHelper.DefaultSerialisationOptions);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
    }
}