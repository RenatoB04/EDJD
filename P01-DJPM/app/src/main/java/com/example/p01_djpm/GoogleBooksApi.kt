package com.example.p01_djpm

import retrofit2.Call
import retrofit2.http.GET
import retrofit2.http.Query

data class BookItem(
    val id: String,
    val volumeInfo: VolumeInfo
)

data class VolumeInfo(
    val title: String,
    val authors: List<String>?,
    val description: String?,
    val imageLinks: ImageLinks?
)

data class ImageLinks(
    val thumbnail: String
)

data class BooksResponse(
    val items: List<BookItem>
)

interface GoogleBooksApi {
    @GET("volumes")
    fun searchBooks(
        @Query("q") query: String,
        @Query("key") apiKey: String
    ): Call<BooksResponse>
}