using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using SemBackDotNet10.baseDomain.repository;
using PagingRequest = Common.utils.PageRequest;

namespace SemBackDotNet10.Base.model.dto;

public sealed class PageableResponse<R>
{
    [JsonPropertyName("content")] public R? Content { get; }
    [JsonPropertyName("totalElements")] public long TotalElements { get; }
    [JsonPropertyName("totalPages")] public int TotalPages { get; }
    [JsonPropertyName("currentPage")] public int CurrentPage { get; }
    [JsonPropertyName("pageSize")] public int PageSize { get; }
    public PageableResponse() { }
    public PageableResponse(R content, long totalElements, int totalPages, int currentPage, int pageSize)
        => (Content, TotalElements, TotalPages, CurrentPage, PageSize) =
            (content, totalElements, totalPages, currentPage, pageSize);
}

public static class PageableResponse
{
    public static PageBuilder Builder() => new();
    public sealed class PageBuilder
    {
        public MapBuilder<I> Page<I>(PageResult<I> page)
            => new(page ?? throw new ArgumentNullException(nameof(page)));
    }
    public sealed class MapBuilder<I>(PageResult<I> page)
    {
        public ContentBuilder<I, R> Flat<R>(Func<List<I>, R> converter)
        {
            ArgumentNullException.ThrowIfNull(converter);
            return new(page, values => converter(values.ToList()));
        }
        // Unlike Java's unchecked R cast, Map returns a list of converted items.
        public ContentBuilder<I, List<R>> Map<R>(Func<I, R> converter)
        {
            ArgumentNullException.ThrowIfNull(converter);
            return new(page, values => values.Select(converter).ToList());
        }
        public FinalBuilder<I, List<I>> PageableAsList(PagingRequest request)
            => new(page, values => values.ToList(), request);
    }
    public sealed class ContentBuilder<I, R>(PageResult<I> page, Func<IReadOnlyList<I>, R> converter)
    {
        public FinalBuilder<I, R> Pageable(PagingRequest request) => new(page, converter, request);
    }
    public sealed class FinalBuilder<I, R>
    {
        private readonly PageResult<I> _page;
        private readonly Func<IReadOnlyList<I>, R> _converter;
        private readonly PagingRequest _request;
        internal FinalBuilder(PageResult<I> page, Func<IReadOnlyList<I>, R> converter, PagingRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);
            if (request.Page < 0 || request.Size < 1) throw new ArgumentOutOfRangeException(nameof(request));
            _page=page; _converter=converter; _request=request;
        }
        public PageableResponse<R> Build() => new(_converter(_page.Content), _page.TotalElements,
            checked((int)_page.TotalPages), _request.Page, _request.Size);
    }
}
