using System;
using Microsoft.AspNetCore.Mvc;

namespace Application.Helpers
{
    public class PageUtility<T>
    {
        public static Pagination CreateResourcePageUrl(ResourceParameter parameters, string name, PagedList<T> pageData, IUrlHelper helper)
        {

            var prevLink = pageData.HasPrevious
                ? CreateResourceUri(parameters, name, ResourceUriType.PreviousPage, helper)
                : null;
            var nextLink = pageData.HasNext
                ? CreateResourceUri(parameters, name, ResourceUriType.NextPage, helper)
                : null;

            var pagination = new Pagination
            {
                NextPage = nextLink,
                PreviousPage = prevLink,
                Total = pageData.TotalCount,
                PageSize = pageData.PageSize,
                TotalPages = pageData.TotalPages
            };

            return pagination;
        }

        private static string CreateResourceUri(ResourceParameter parameter, string name, ResourceUriType type, IUrlHelper url)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return url.Link(name,
                        new
                        {
                            PageNumber = parameter.PageNumber - 1,
                            parameter.PageSize,
                            Search = parameter.Search
                        });

                case ResourceUriType.NextPage:
                    return url.Link(name,
                        new
                        {
                            PageNumber = parameter.PageNumber + 1,
                            parameter.PageSize,
                            Search = parameter.Search
                        });

                default:
                    return url.Link(name,
                        new
                        {
                            parameter.PageNumber,
                            parameter.PageSize,
                            Search = parameter.Search,
                        });
            }

        }
    }

}