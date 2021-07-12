using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Entities.DataTransferObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

namespace WideWorldImportersWebApi.Utility
{
    public class CsvOutputFormatter : TextOutputFormatter
    {
        public CsvOutputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/csv"));
            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
        }

        protected override bool CanWriteType(Type type)
        {
            if (typeof(SupplierCategoryDto).IsAssignableFrom(type) || typeof(IEnumerable<SupplierCategoryDto>).IsAssignableFrom(type))
            {
                return base.CanWriteType(type);
            }

            return false;
        }

        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            var response = context.HttpContext.Response;
            var buffer = new StringBuilder();

            if (context.Object is IEnumerable<SupplierCategoryDto>)
            {
                foreach (var supplierCategories in (IEnumerable<SupplierCategoryDto>) context.Object)
                {
                    FormatCsv(buffer, supplierCategories);
                }
            }
            else
            {
                FormatCsv(buffer, (SupplierCategoryDto) context.Object);
            }

            await response.WriteAsync(buffer.ToString());
        }

        private static void FormatCsv(StringBuilder buffer, SupplierCategoryDto supplierCategory) { buffer.AppendLine($@"{supplierCategory.SupplierCategoryId},{supplierCategory.SupplierCategoryName},{supplierCategory.LastEditedBy},{supplierCategory.ValidFrom},{supplierCategory.ValidTo}\"); }
    }
}