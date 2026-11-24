using System;
using System.Collections;
using System.Text.Json.Serialization;

namespace SemBackDotNet10.Base.model.dto;

public sealed class SearchDto
{
    [JsonPropertyName("content")] public IList? Content { get; }
    [JsonPropertyName("totalElements")] public long TotalElements { get; }
    [JsonPropertyName("totalPages")] public int TotalPages { get; }
    [JsonPropertyName("currentPage")] public int CurrentPage { get; }
    [JsonPropertyName("pageSize")] public int PageSize { get; }
    [JsonPropertyName("last")] public bool IsLast => TotalElements <= ((long)CurrentPage + 1) * PageSize;

    private SearchDto(IList? content, long totalElements, int totalPages, int currentPage, int pageSize)
        => (Content, TotalElements, TotalPages, CurrentPage, PageSize) =
            (content, totalElements, totalPages, currentPage, pageSize);

    // Java-style builder names retained to support translated service callers.
    public sealed class Builder
    {
        private IList? _content;
        private long _totalElements;
        private int _totalPages, _currentPage, _pageSize;
        public Builder content(IEnumerable? value) { _content = value is null ? null : value as IList ?? value.Cast<object?>().ToList(); return this; }
        public Builder totalElements(long value) { _totalElements=value; return this; }
        public Builder totalPages(int value) { _totalPages=value; return this; }
        public Builder currentPage(int value) { _currentPage=value; return this; }
        public Builder pageSize(int value) { _pageSize=value; return this; }
        public Builder Content(IEnumerable? value) => content(value);
        public Builder TotalElements(long value) => totalElements(value);
        public Builder TotalPages(int value) => totalPages(value);
        public Builder CurrentPage(int value) => currentPage(value);
        public Builder PageSize(int value) => pageSize(value);
        public SearchDto Build() => build();
        public SearchDto build() => new(_content, _totalElements, _totalPages, _currentPage, _pageSize);
    }
}
